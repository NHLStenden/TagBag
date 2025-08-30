namespace NhlStenden.TagBag;

/// <summary>
/// Provides helper methods for handling tags.
/// </summary>
public static class TagHelper
{
    /// <summary>
    /// Splits the input text into a list of tags.
    /// </summary>
    /// <param name="text">The input text to split into tags.</param>
    /// <returns>A list of tags extracted from the input text.</returns>
    public static IList<string> SplitIntoTags(string text)
    {
        return text.Split(',').Select(x => x.Trim()).ToList();
    }
}