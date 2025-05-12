using Microsoft.Extensions.AI;
using OllamaSharp;

public static class TagTransformer
{
    public static void Execute(IList<string> args)
    {
        var fileNames = GetFiles(args[0]);

        var model = args.Count >= 2 ? args[1] : "";
        var promptFileName = args.Count >= 3 ? args[2] : "";

        TransformFiles(fileNames, model, promptFileName);
    }

    static string[] GetFiles(string path)
    {
        try
        {
        var fileNames = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(path, "*.npz", SearchOption.AllDirectories);
        return fileNames;
        }
        catch (DirectoryNotFoundException)
        {
            return [];
        }
    }

    static void TransformFiles(string[] fileNames, string model, string promptFileName)
    {
        foreach (var fileName in fileNames)
            TransformFile(fileName, model, promptFileName);
    }

    static void TransformFile(string fileName, string model, string promptFileName)
    {
        var client = new OllamaApiClient("http://localhost:11434", model);

        try
        {
            var lines = string.Join(Environment.NewLine, File.ReadAllLines(fileName).ToList());
            var prompt = string.Join(Environment.NewLine, File.ReadAllLines(promptFileName).ToList());
            prompt += lines;
            var r = Task.Run(() => client.GetResponseAsync(prompt)).GetAwaiter().GetResult();
            File.WriteAllLines(fileName, [r.Text]);
        }
        catch (HttpRequestException)
        {

        }
    }    
}