
using Microsoft.Extensions.AI;
using Microsoft.VisualBasic;
using OllamaSharp;

namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that analyzes tags in files using AI models.
/// </summary>
public class TagAnalyzer : TagBase
{
    /// <summary>
    /// Executes the tag analyzing process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        var model = args.Count >= 2 ? args[1] : "";
        var promptFileName = args.Count >= 3 ? args[2] : "";

        var content = new List<string>();
        LoadTagsFromFiles(fileNames, content);

        Console.WriteLine("Tags in files:");
        foreach (var c in content)
            Console.WriteLine(c);

        var sum = Analyze(content, model, promptFileName);
        var lines = new List<string>(sum.Split(["\r\n", "\n", "\r"], StringSplitOptions.None));
        foreach (var line in lines)
            Console.WriteLine(line);
    }

    /// <summary>
    /// Reads all tags from multiple files into a list.
    /// </summary>
    /// <param name="fileNames">An array of file paths to process.</param>
    /// <param name="tokens">A list to collect the tags.</param>
    static void LoadTagsFromFiles(string[] fileNames, List<string> tokens)
    {
        foreach (var fileName in fileNames)
            tokens.AddRange(File.ReadAllLines(fileName));
    }

    /// <summary>
    /// Uses the specified AI model to analyze the given tags.
    /// </summary>
    /// <param name="content">The list of tags to process.</param>
    /// <param name="model">The name of the AI model to use for transformation.</param>
    /// <param name="promptFileName">The name of the prompt file to use for transformation.</param>
    /// <returns>The AI-generated analysis text, or an empty string if an error occurs.</returns>
    static string Analyze(List<string> content, string model, string promptFileName)
    {
        var client = new OllamaApiClient("http://localhost:11434", model);

        try
        {
            var lines = string.Join(Environment.NewLine, content);
            var prompt = string.Join(Environment.NewLine, File.ReadAllLines(promptFileName).ToList());
            prompt += lines;
            var r = Task.Run(() => client.GetResponseAsync(prompt)).GetAwaiter().GetResult();
            return r.Text;
        }
        catch (HttpRequestException)
        {
            return "";
        }
    }
}