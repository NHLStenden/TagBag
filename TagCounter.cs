
namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that counts the occurrences of tags in files.
/// </summary>
public class TagCounter : TagBase
{
    /// <summary>
    /// Executes the tag counting process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        var tokens = new Dictionary<string, int>();
        CountTagsInFiles(fileNames, tokens);

        foreach (var token in tokens.OrderByDescending(x => x.Value))
            Console.WriteLine(token.Key + ": " + token.Value);
    }

    /// <summary>
    /// Counts the tags in the specified files and updates the token dictionary.
    /// </summary>
    /// <param name="fileNames">An array of file names to process.</param>
    /// <param name="tokens">A dictionary to store the count of each tag.</param>
    static void CountTagsInFiles(string[] fileNames, Dictionary<string, int> tokens)
    {
        foreach (var fileName in fileNames)
            CountTagsInFile(fileName, tokens);
    }

    /// <summary>
    /// Counts the tags in the specified file and updates the token dictionary.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <param name="tokens">A dictionary to store the count of each tag.</param>
    static void CountTagsInFile(string fileName, IDictionary<string, int> tokens)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var items = TagHelper.SplitIntoTags(line);
            for (var j = 0; j < items.Count; j++)
            {
                var item = items[j];
                if (tokens.TryGetValue(item, out var counter))
                {
                    tokens[item] = counter + 1;
                }
                else
                {
                    tokens[item] = 1;
                }
            }
        }
    }    
}