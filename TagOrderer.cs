public static class TagOrderer
{
    public static void Execute(IList<string> args)
    {
        var fileNames = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
        if (fileNames.Length == 0)
            fileNames = Directory.GetFiles(args[0], "*.npz", SearchOption.AllDirectories);
        var tokens = new Dictionary<string, (int Count, float Position)>();
        CountTagsInFiles(fileNames, tokens);

        Order(fileNames, tokens);
    }

    static void CountTagsInFiles(string[] fileNames, Dictionary<string, (int Count, float Position)> tokens)
    {
        foreach (var fileName in fileNames)
            CountTagsInFile(fileName, tokens);
    }

    static void CountTagsInFile(string fileName, IDictionary<string, (int Count, float Position)> tokens)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var items = TagHelper.SplitIntoTags(line);
            for (var j = 0; j < items.Count; j++)
            {
                var item = items[j];
                var currentPosition = j * (j / (float)items.Count);
                if (tokens.TryGetValue(item, out var counter))
                {
                    var count = counter.Count + 1;
                    var newPosition = (currentPosition / count) + (counter.Position * ((count - 1) / ((float)count)));
                    tokens[item] = (count, newPosition);
                }
                else
                {
                    tokens[item] = (1, currentPosition);
                }
            }
        }
    }    
    static void Order(string[] fileNames, Dictionary<string, (int Count, float Position)> tokens)
    {
        foreach (var fileName in fileNames)
        {
            var lines = File.ReadAllLines(fileName).ToList();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var items = TagHelper.SplitIntoTags(line);
                var newTokes = new List<(string Token, float Position)>();
                for (var j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    if (!tokens.TryGetValue(item, out var counter))
                        continue;
                    newTokes.Add((item, counter.Position));
                }
                newTokes = newTokes.OrderBy(x => x.Position).ToList();
                line = string.Join(", ", newTokes.Select(x => x.Token));
                lines[i] = line;
            }
            File.WriteAllLines(fileName, lines);
        }
    }
}