namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that orders tags in files based on their frequency and position.
/// </summary>
public class TagOrderer : TagBase
{
    /// <summary>
    /// Executes the tag ordering process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        var tokens = new Dictionary<string, (int Count, float Position)>();
        CountTagsInFiles(fileNames, tokens);

        Order(fileNames, tokens);
    }

    /// <summary>
    /// Counts the tags in the specified files and updates the token dictionary.
    /// </summary>
    /// <param name="fileNames">An array of file names to process.</param>
    /// <param name="tokens">A dictionary to store the count and position of each tag.</param>
    static void CountTagsInFiles(string[] fileNames, Dictionary<string, (int Count, float Position)> tokens)
    {
        foreach (var fileName in fileNames)
            CountTagsInFile(fileName, tokens);
    }

    /// <summary>
    /// Counts the tags in the specified file and updates the token dictionary.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <param name="tokens">A dictionary to store the count and position of each tag.</param>
    static void CountTagsInFile(string fileName, IDictionary<string, (int Count, float Position)> tokens)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var items = TagHelper.SplitIntoTags(line);
            for (var j = 0; j < items.Count; j++)
            {
                var item = items[j];
                var currentPosition = j * (j / (float)items.Count);
                if (tokens.TryGetValue(item, out var counter))
                {
                    var count = counter.Count + 1;
                    var newPosition = (currentPosition / count) + (counter.Position * ((count - 1) / ((float)count)));
                    tokens[item] = (count, newPosition);
                }
                else
                {
                    tokens[item] = (1, currentPosition);
                }
            }
        }
    }

    /// <summary>
    /// Orders the tags in the specified files based on their frequency and position.
    /// </summary>
    /// <param name="fileNames">An array of file names to process.</param>
    /// <param name="tokens">A dictionary containing the count and position of each tag.</param>
    static void Order(string[] fileNames, Dictionary<string, (int Count, float Position)> tokens)
    {
        foreach (var fileName in fileNames)
        {
            var lines = File.ReadAllLines(fileName).ToList();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var items = TagHelper.SplitIntoTags(line);
                var newTokes = new List<(string Token, float Position)>();
                for (var j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    if (!tokens.TryGetValue(item, out var counter))
                        continue;
                    newTokes.Add((item, counter.Position));
                }
                newTokes = newTokes.OrderBy(x => x.Position).ToList();
                line = string.Join(", ", newTokes.Select(x => x.Token));
                lines[i] = line;
            }
            File.WriteAllLines(fileName, lines);
        }
    }
}