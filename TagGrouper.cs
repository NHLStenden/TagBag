public static class TagGrouper
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        var groups = args.Count >= 2 ? new List<string>(args.ToArray()[1..]) : new List<string>();

        foreach (var fileName in fileNames)
            ProcessFile(fileName, groups);
    }

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

    static IList<int> IndexOf<T>(IList<T> haystack, IList<T> needles)
    {
        var result = new List<int>();
        foreach (var needle in needles)
        {
            result.Add(haystack.IndexOf(needle));
        }
        return result;
    }

    static bool ValidIndices(IList<int> indices)
    {
        foreach (var index in indices)
            if (index < 0) return false;

        return true;
    }
}
