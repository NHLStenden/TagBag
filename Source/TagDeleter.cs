namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that deletes specified tags from files.
/// </summary>
public class TagDeleter : TagBase
{
    /// <summary>
    /// Executes the tag deletion process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path and the second argument is the file containing tags to remove.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        var removeWordsFileName = args.Count >= 2 ? args[1] : "";
        var removeWords = !string.IsNullOrEmpty(removeWordsFileName) ? File.ReadAllLines(removeWordsFileName).ToList() : new List<string>();

        foreach (var fileName in fileNames)
            ProcessFile(fileName, removeWords);
    }

    /// <summary>
    /// Processes the specified file to remove specified tags.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <param name="removeWords">A list of tags to remove from the file.</param>
    static void ProcessFile(string fileName, List<string> removeWords)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var newLine = new List<string>();

            var items = TagHelper.SplitIntoTags(line);
            for (var j = 0; j < items.Count; j++)
            {
                var item = items[j];
                item = item.Trim();
                if (!removeWords.Contains(item) && !string.IsNullOrEmpty(item))
                {
                    newLine.Add(item);
                }
            }
            newLine = newLine.ToList();
            line = string.Join(", ", newLine);
            lines[i] = line;
        }

        File.WriteAllLines(fileName, lines);
    }
}
