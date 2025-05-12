namespace NhlStenden.TagBag;

/// <summary>
/// Represents a class that converts tag files to JSONL format.
/// </summary>
public class TagToJsonlConverter : TagBase
{
    /// <summary>
    /// Executes the conversion process on the specified files.
    /// </summary>
    /// <param name="args">A list of arguments where the first argument is the directory path.</param>
    public static void Execute(IList<string> args)
    {
        var fileNames = GetTagFiles(args[0]);
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

    /// <summary>
    /// Processes the specified file to convert its content to JSONL format.
    /// </summary>
    /// <param name="fileName">The name of the file to process.</param>
    /// <returns>A JSONL formatted string representing the file content.</returns>
    static string ProcessFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName).ToList();
        if (lines.Count == 0) return "";
        var imageFileName = GetImageFileName(fileName);
        if (string.IsNullOrEmpty(imageFileName)) return "";
        var line = string.Join(" ", lines);
        return "{\"image\": \"" + imageFileName + "\", \"prompt\": \"" + line + "\"}";
    }

    /// <summary>
    /// Retrieves the corresponding image file name for the given text file name.
    /// </summary>
    /// <param name="textFileName">The name of the text file.</param>
    /// <returns>The name of the corresponding image file, if found; otherwise, an empty string.</returns>
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