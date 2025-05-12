namespace NhlStenden.TagBag;

/// <summary>
/// Main program entry.
/// </summary>
static class Program
{
    /// <summary>
    /// Main method.
    /// </summary>
    /// <param name="args">Arguments.</param>
    static void Main(string[] args)
    {
        if (args.Length < 2)
            return;
        var dictionary = new Dictionary<string, Action<IList<string>>>
        {
            { "order-mean", TagOrderer.Execute },
            { "count", TagCounter.Execute },
            { "delete", TagDeleter.Execute },
            { "delete-doubles", DoubleTagDeleter.Execute },
            { "convert-to-jsonl", TagToJsonlConverter.Execute },
            { "group", TagGrouper.Execute },
            { "transform", TagTransformer.Execute }
        };

        var argsForExecutor = args.Where((x, i) => i != 0).ToList();
        if (!dictionary.TryGetValue(args[0], out var action))
            return;

        action(argsForExecutor);
    }
}
