using Microsoft.Extensions.AI;
using OllamaSharp;

namespace NhlStenden.TagBag;

/// <summary>
/// Provides methods to transform tag files using AI models.
/// </summary>
public class TagTransformer : TagBase
{
    /// <summary>
    /// Executes the transformation process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path, the second argument is the model name, and the third argument is the prompt file name.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);

        var model = args.Count >= 2 ? args[1] : "";
        var promptFileName = args.Count >= 3 ? args[2] : "";

        TransformFiles(fileNames, model, promptFileName);
    }

    /// <summary>
    /// Transforms the specified files using the given model and prompt file.
    /// </summary>
    /// <param name="fileNames">An array of file names to process.</param>
    /// <param name="model">The name of the AI model to use for transformation.</param>
    /// <param name="promptFileName">The name of the prompt file to use for transformation.</param>
    static void TransformFiles(string[] fileNames, string model, string promptFileName)
    {
        foreach (var fileName in fileNames)
            TransformFile(fileName, model, promptFileName);
    }

    /// <summary>
    /// Transforms the specified file using the given model and prompt file.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <param name="model">The name of the AI model to use for transformation.</param>
    /// <param name="promptFileName">The name of the prompt file to use for transformation.</param>
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