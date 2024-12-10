using System.Collections.Immutable;

namespace AdventOfCode.Cli;

public class Day10
{
    readonly record struct Point(int X, int Y)
    {
        public static readonly Point Up = new(0, 1);
        public static readonly Point Down = new(0, -1);
        public static readonly Point Left = new(-1, 0);
        public static readonly Point Right = new(1, 0);
        
        public static Point operator +(Point a, Point b) 
            => new(a.X + b.X, a.Y + b.Y);
    }

    private async ValueTask<ImmutableDictionary<Point, char>> ParseDataAsync()
    {
        var lines = await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day10-input.txt");
        return 
        (
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Point, char>(new Point(x, y), lines[y][x])
        ).ToImmutableDictionary();
    }

    private Dictionary<Point, List<Point>> GetTrails(ImmutableDictionary<Point, char> map)
    {
        return GetTrailHeads(map).ToDictionary(x => x, trailHead => GetTrailsFrom(map, trailHead));
    }
    
    private IEnumerable<Point> GetTrailHeads(ImmutableDictionary<Point, char> map) 
        => map.Keys.Where(x => map[x] == '0');
    
    // Find all the trails leading up to a '9' using flood fill.
    private List<Point> GetTrailsFrom(ImmutableDictionary<Point, char> map, Point trailHead) {
        var points = new Queue<Point>();
        points.Enqueue(trailHead);
        
        var trails = new List<Point>();
        while (points.Count != 0) 
        {
            var point = points.Dequeue();
            if (map[point] == '9') 
            {
                trails.Add(point);
            }
            else 
            {
                foreach (var dir in new[] { Point.Up, Point.Down, Point.Left, Point.Right }) 
                {
                    if (map.GetValueOrDefault(point + dir) == map[point] + 1) 
                    {
                        points.Enqueue(point + dir);
                    }
                }
            }
        }
        return trails;
    }
    
    public async ValueTask Task1()
    {
        var map = await ParseDataAsync();

        var trails = GetTrails(map);
        
        Console.WriteLine(trails.Sum(x => x.Value.Distinct().Count()));
    }
    
    public async ValueTask Task2()
    {
        var map = await ParseDataAsync();

        var trails = GetTrails(map);
        
        Console.WriteLine(trails.Sum(x => x.Value.Count));
    }
}