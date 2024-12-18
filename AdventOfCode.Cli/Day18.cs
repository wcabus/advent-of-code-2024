namespace AdventOfCode.Cli;

public class Day18
{
    private int _size;
    private int _takeBytes;
    private HashSet<(int,int)> _fallenBytes = [];
    private List<(int, int)> _allBytes = [];

    public async ValueTask ParseDataAsync(string path, bool testInput)
    {
        _size = testInput ? 7 : 71;
        _takeBytes = testInput ? 12 : 1024;

        var i = 0;
        await foreach (var line in Helpers.GetInput(path))
        {
            var bytes = line.Split(',').Select(int.Parse).ToArray();
            
            _allBytes.Add((bytes[0], bytes[1]));
            i++;
            if (i >= _takeBytes) continue;
            
            _fallenBytes.Add((bytes[0], bytes[1]));
        }
    }

    private int Traverse((int x, int y) dimensions, HashSet<(int x, int y)> bytes)
    {
        var trailheads = new Queue<(int x, int y, int steps)>();
        trailheads.Enqueue(new ValueTuple<int, int, int>(0, 0, 1));

        var seen = new Dictionary<(int x, int y), int>();
        seen[(0, 0)] = 0;

        while (trailheads.Count > 0)
        {
            var trail = trailheads.Dequeue(); 
            Extend( 0, -1);
            Extend(+1,  0);
            Extend( 0, +1);
            Extend(-1,  0);

            void Extend(int dx, int dy)
            {
                var nx = trail.x + dx;
                var ny = trail.y + dy;
                var innerSteps = trail.steps + 1;

                if (nx < 0 || nx >= dimensions.x || ny < 0 || ny >= dimensions.y
                    || bytes.Contains((nx, ny))
                    || seen.TryGetValue((nx, ny), out var seenSteps) && seenSteps <= innerSteps)
                    return;

                seen[(nx, ny)] = innerSteps;

                if (nx == dimensions.x - 1 && ny == dimensions.y - 1)
                    return;
                    
                trailheads.Enqueue(new ValueTuple<int, int, int>(nx, ny, innerSteps));
            }
        }

        return seen.TryGetValue((dimensions.x-1, dimensions.y-1), out var steps) ? steps - 1 : int.MaxValue;
    }
    
    public ValueTask Task1()
    {
        var result = Traverse((_size, _size), _fallenBytes);
        Console.WriteLine(result);
        return ValueTask.CompletedTask;
    }

    public ValueTask Task2()
    {
        for (var i = 0; i < _allBytes.Count; i++)
        {
            if (Traverse((_size, _size), [.._allBytes[..i]]) != int.MaxValue)
            {
                continue;
            }
            
            Console.WriteLine($"{_allBytes[i - 1]}");
            return ValueTask.CompletedTask;
        }
        
        return ValueTask.CompletedTask;
    }
}