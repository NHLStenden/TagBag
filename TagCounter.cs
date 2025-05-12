public static class TagCounter
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        var tokens = new Dictionary<string, int>();
        CountTagsInFiles(fileNames, tokens);

        foreach (var token in tokens.OrderByDescending(x => x.Value))
            Console.WriteLine(token.Key + ": " + token.Value);
    }

    static void CountTagsInFiles(string[] fileNames, Dictionary<string, int> tokens)
    {
        foreach (var fileName in fileNames)
            CountTagsInFile(fileName, tokens);
    }

    static void CountTagsInFile(string fileName, IDictionary<string, int> tokens)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var items = TagHelper.SplitIntoTags(line);
            for (var j = 0; j < items.Count; j++)
            {
                var item = items[j];
                if (tokens.TryGetValue(item, out var counter))
                {
                    tokens[item] = counter + 1;
                }
                else
                {
                    tokens[item] = 1;
                }
            }
        }
    }    
}