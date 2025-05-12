public static class TagToJsonlConverter
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        var lines = new List<String>();
        foreach (var fileName in fileNames)
        {
            var line = ProcessFile(fileName);
            if (string.IsNullOrEmpty(line))
                continue;
            lines.Add(line);
        }
        File.WriteAllLines(Path.Join(args[0], "train.jsonl"), lines);
    }

    static string ProcessFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName).ToList();
        if (lines.Count == 0) return "";
        var imageFileName = GetImageFileName(fileName);
        if (string.IsNullOrEmpty(imageFileName)) return "";
        var line = string.Join(" ", lines);
        return "{\"image\": \"" + imageFileName + "\", \"prompt\": \"" + line + "\"}";
    }

    static string GetImageFileName(string textFileName)
    {
        var baseTextFileName = Path.GetFileNameWithoutExtension(textFileName);
        var directory = Path.GetDirectoryName(textFileName);
        if (directory == null) return "";
        var fileNames = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var fileName in fileNames)
        {
            var baseFileName = Path.GetFileNameWithoutExtension(fileName);
            if (string.Equals(baseFileName, baseTextFileName, StringComparison.Ordinal) && !string.Equals(fileName, textFileName, StringComparison.Ordinal))
                return fileName;
        }

        return "";
    }
}