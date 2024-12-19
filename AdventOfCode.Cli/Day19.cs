namespace AdventOfCode.Cli;

public class Day19
{
    private List<string> _availablePatterns = new();
    private List<string> _designs = new();

    public async ValueTask ParseDataAsync(string path)
    {
        if (_designs.Count > 0)
        {
            return;
        }

        var lines = await Helpers.GetAllLinesAsync(path);
        _availablePatterns.AddRange(lines[0].Split([',', ' '], StringSplitOptions.RemoveEmptyEntries));

        _designs.AddRange(lines[2..]);
    }

    private long IsDesignPossible(Dictionary<string, long> cache, string design)
    {
        if (design == "")
        {
            return 1;
        }

        if (cache.TryGetValue(design, out var count))
        {
            return count;
        }
        
        count = _availablePatterns
            .Where(pattern => design.StartsWith(pattern, StringComparison.Ordinal))
            .Sum(pattern => IsDesignPossible(cache, design[pattern.Length..]));
        
        cache.TryAdd(design, count);
        return count;
    }

    public ValueTask Task1()
    {
        var cache = new Dictionary<string, long>();
        var count = _designs.Count(x => IsDesignPossible(cache, x) > 0);
        Console.WriteLine(count);

        return ValueTask.CompletedTask;
    }

    public ValueTask Task2()
    {
        var cache = new Dictionary<string, long>();
        var count = _designs.Sum(x => IsDesignPossible(cache, x));
        Console.WriteLine(count);

        return ValueTask.CompletedTask;
    }
}