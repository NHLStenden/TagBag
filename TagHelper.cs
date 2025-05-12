public static class TagHelper
{
    public static IList<string> SplitIntoTags(string text)
    {
        return text.Split(',').Select(x => x.Trim()).ToList();
    }
}