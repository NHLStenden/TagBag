
namespace NhlStenden.TagBag;

/// <summary>
/// Represents the base class for handling tags.
/// </summary>
public class TagBase
{
    /// <summary>
    /// Retrieves the tag files from the specified directory path.
    /// </summary>
    /// <param name="path">The directory path to search for tag files.</param>
    /// <returns>An array of file names matching the tag file patterns.</returns>
    public static string[] GetTagFiles(string path)
    {
        try
        {
            var fileNames = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);
            if (fileNames.Length == 0)
                fileNames = Directory.GetFiles(path, "*.npz", SearchOption.AllDirectories);
            return fileNames;
        }
        catch (DirectoryNotFoundException)
        {
            return [];
        }
    }
}
