namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that groups specified tags in files.
/// </summary>
public class TagGrouper : TagBase
{
    /// <summary>
    /// Executes the tag grouping process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path and subsequent arguments are the groups of tags.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
        var groups = args.Count >= 2 ? [.. args.ToArray()[1..]] : new List<string>();

        foreach (var fileName in fileNames)
            ProcessFile(fileName, groups);
    }

    /// <summary>
    /// Processes the specified file to group specified tags.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <param name="groups">A list of groups of tags to be grouped in the file.</param>
    static void ProcessFile(string fileName, List<string> groups)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var newLine = new List<string>();

            var items = TagHelper.SplitIntoTags(line).ToList();
            foreach (var group in groups)
            {
                var groupItems = TagHelper.SplitIntoTags(group);
                var groupIndices = IndexOf(items, groupItems).OrderByDescending(x => x).ToList();
                if (ValidIndices(groupIndices))
                {
                    // regroup items into new version of items
                    foreach (var groupIndex in groupIndices)
                    {
                        items.RemoveAt(groupIndex);
                    }
                    items.InsertRange(groupIndices[^1], groupItems);
                }
            }

            newLine = items.ToList();
            line = string.Join(", ", newLine);
            lines[i] = line;
        }

        File.WriteAllLines(fileName, lines);
    }

    /// <summary>
    /// Finds the indices of the specified needles in the haystack.
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists.</typeparam>
    /// <param name="haystack">The list to search in.</param>
    /// <param name="needles">The list of elements to find.</param>
    /// <returns>A list of indices where the needles are found in the haystack.</returns>
    static IList<int> IndexOf<T>(IList<T> haystack, IList<T> needles)
    {
        var result = new List<int>();
        foreach (var needle in needles)
        {
            result.Add(haystack.IndexOf(needle));
        }
        return result;
    }

    /// <summary>
    /// Validates the indices to ensure they are all non-negative.
    /// </summary>
    /// <param name="indices">The list of indices to validate.</param>
    /// <returns>True if all indices are non-negative; otherwise, false.</returns>
    static bool ValidIndices(IList<int> indices)
    {
        foreach (var index in indices)
            if (index < 0) return false;

        return true;
    }
}
