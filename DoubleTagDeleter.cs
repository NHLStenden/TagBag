namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that deletes duplicate tags from files.
/// </summary>
public class DoubleTagDeleter : TagBase
{
    /// <summary>
    /// Executes the tag deletion process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        foreach (var fileName in fileNames)
        {
            ProcessFile(fileName);
        }
    }

    /// <summary>
    /// Processes the specified file to remove duplicate tags.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    static void ProcessFile(string fileName)
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
                if (!string.IsNullOrEmpty(item))
                    newLine.Add(item);
            }
            newLine = newLine.Distinct().ToList();
            line = string.Join(", ", newLine);
            lines[i] = line;
        }

        File.WriteAllLines(fileName, lines);
    }
}