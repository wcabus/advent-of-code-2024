namespace AdventOfCode.Cli;

public class Day20
{
    private Node[,] _map = new Node[0,0];
    private Node? _start;
    private Node? _goal;
    
    public async ValueTask ParseDataAsync(string path)
    {
        Array.Clear(_map);
        
        var lines = await Helpers.GetAllLinesAsync(path);
        _map = new Node[lines[0].Length, lines.Length];
        for (var row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (var col = 0; col < line.Length; col++)
            {
                _map[col, row] = new Node((col, row)) { Char = line[col] };
                if (line[col] == 'S')
                {
                    _start = _map[col, row];
                }
                
                if (line[col] == 'E')
                {
                    _goal = _map[col, row];
                }
            }
        }
    }

    record Node((int X, int Y) Position)
    {
        public char Char { get; set; }
    }

    private Dictionary<(int X, int Y), int> GetPathThroughMaze()
    {
        var current = _start!.Position;
        var path = new Dictionary<(int X, int Y), int>
        {
            [current] = 0
        };
        var time = 1;
        
        Span<(int x, int y)> directions = [(-1, 0), (0, 1), (1, 0), (0, -1)];
        while (current != _goal!.Position)
        {
            foreach (var direction in directions)
            {
                var newPosition = (X: current.X + direction.x, Y:current.Y + direction.y);
                if (newPosition.X < 0 ||
                    newPosition.X >= _map.GetLength(0) ||
                    newPosition.Y < 0 ||
                    newPosition.Y >= _map.GetLength(1) ||
                    _map[newPosition.X, newPosition.Y].Char == '#' ||
                    !path.TryAdd(newPosition, time))
                {
                    continue;
                }
                
                current = newPosition;
                time++;
                break;
            }
        }

        return path;
    }

    private int AttemptToCheat(Dictionary<(int X, int Y), int> path, bool testInput)
    {
        var cheatsPerTime = new Dictionary<int, int>();
        var result = 0;
        
        Span<(int x, int y)> jumps = [(-2, 0), (0, 2), (2, 0), (0, -2)];
        foreach (var kvp in path)
        {
            var cheatStart = kvp.Key;
            var cheatStartTime = kvp.Value;
            foreach (var (x, y) in jumps)
            {
                var cheatEndX = cheatStart.X + x;
                var cheatEndY = cheatStart.Y + y;
                    
                if (cheatEndX < 0 || cheatEndX >= _map.GetLength(0) || cheatEndY < 0 || cheatEndY >= _map.GetLength(1))
                {
                    continue;
                }
                
                var cheatEnd = (cheatEndX, cheatEndY);
                if (!path.TryGetValue(cheatEnd, out var cheatEndTime))
                {
                    continue;
                }

                var savedTime = cheatEndTime - cheatStartTime - 2;
                if (testInput && savedTime > 0)
                {
                    if (cheatsPerTime.TryGetValue(savedTime, out var value))
                    {
                        cheatsPerTime[savedTime] = ++value;
                    }
                    else
                    {
                        cheatsPerTime[savedTime] = 1;
                    }
                }
                
                if (savedTime >= 100)
                {
                    result++;
                }
            }
        }

        if (testInput)
        {
            foreach (var kvp in cheatsPerTime.OrderBy(x => x.Key))
            {
                Console.WriteLine($"There {(kvp.Value == 1 ? "is" : "are")} {kvp.Value} cheat{(kvp.Value == 1 ? "": "s")} that save{(kvp.Value == 1 ? "s": "")} {kvp.Key} picosecond{(kvp.Key == 1 ? "": "s")}.");
            }
        }
        
        return result;
    }
    
    private int AttemptToCheat2(Dictionary<(int X, int Y), int> path, bool testInput)
    {
        var cheatsPerTime = new Dictionary<int, int>();
        var result = 0;
        
        foreach (var kvp in path)
        {
            var cheatStart = kvp.Key;
            var cheatStartTime = kvp.Value;

            for (var x = -20; x <= 20; x++)
            {
                for (var y = -20 + Math.Abs(x); y <= 20 - + Math.Abs(x); y++)
                {
                    var cheatEndX = cheatStart.X + x;
                    var cheatEndY = cheatStart.Y + y;

                    if (cheatEndX < 0 || cheatEndX >= _map.GetLength(0) || cheatEndY < 0 || cheatEndY >= _map.GetLength(1))
                    {
                        continue;
                    }

                    var cheatEnd = (cheatEndX, cheatEndY);
                    if (!path.TryGetValue(cheatEnd, out var cheatEndTime))
                    {
                        continue;
                    }

                    var distance = Math.Abs(x) + Math.Abs(y);
                    var savedTime = cheatEndTime - cheatStartTime - distance;
                    if (testInput && savedTime >= 50)
                    {
                        if (cheatsPerTime.TryGetValue(savedTime, out var value))
                        {
                            cheatsPerTime[savedTime] = ++value;
                        }
                        else
                        {
                            cheatsPerTime[savedTime] = 1;
                        }
                    }

                    if (savedTime >= 100)
                    {
                        result++;
                    }
                }
            }
        }

        if (testInput)
        {
            foreach (var kvp in cheatsPerTime.OrderBy(x => x.Key))
            {
                Console.WriteLine($"There {(kvp.Value == 1 ? "is" : "are")} {kvp.Value} cheat{(kvp.Value == 1 ? "": "s")} that save{(kvp.Value == 1 ? "s": "")} {kvp.Key} picosecond{(kvp.Key == 1 ? "": "s")}.");
            }
        }
        
        return result;
    }
    
    public ValueTask Task1(bool testInput = false)
    {
        var path = GetPathThroughMaze();
        var result = AttemptToCheat(path, testInput);
        Console.WriteLine(result);
        return ValueTask.CompletedTask;
    }

    public ValueTask Task2(bool testInput = false)
    {
        var path = GetPathThroughMaze();
        var result = AttemptToCheat2(path, testInput);
        Console.WriteLine(result);
        return ValueTask.CompletedTask;
    }
}