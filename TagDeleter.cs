public static class TagDeleter
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        var removeWordsFileName = args.Count >= 2 ? args[1] : "";
        var removeWords = !string.IsNullOrEmpty(removeWordsFileName) ? File.ReadAllLines(removeWordsFileName).ToList() : new List<string>();

        foreach (var fileName in fileNames)
            ProcessFile(fileName, removeWords);
    }

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
