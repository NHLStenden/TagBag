public static class DoubleTagDeleter
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        foreach (var fileName in fileNames)
        {
            ProcessFile(fileName);
        }
    }
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