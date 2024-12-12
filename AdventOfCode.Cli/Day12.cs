namespace AdventOfCode.Cli;

public class Day12
{
    private char[,] _farm = new char[0, 0];
    private int[,] _regions = new int[0, 0];

    private int _currentRegion;

    private async ValueTask ParseDataAsync()
    {
        var fileData = await Helpers.GetFullTextAsync(@"C:\temp\aoc\day12-input.txt");
        var lines = fileData.Split("\r\n");
        
        var width = lines[0].Length;
        var height = lines.Length;
        _farm = new char[width, height];
        _regions = new int[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                _farm[x, y] = lines[y][x];
                _regions[x, y] = 0;
            }
        }
    }

    public async ValueTask Task1()
    {
        await ParseDataAsync();
        var regions = FindRegions();
        
        Console.WriteLine(regions.Values.Sum(x => x.Area * x.Perimeter));
    }

    public async ValueTask Task2()
    {
        await ParseDataAsync();
        var regions = FindRegions();

        Console.WriteLine(regions.Values.Sum(x => x.Area * x.Sides));
    }

    private Dictionary<int, Region> FindRegions()
    {
        _currentRegion = 1;
        var regions = new Dictionary<int, Region>();

        for (var y = 0; y < _farm.GetLength(1); y++)
        {
            for (var x = 0; x < _farm.GetLength(0); x++)
            {
                if (_regions[x, y] != 0)
                {
                    continue; // already visited
                }

                regions.Add(_currentRegion, ExploreRegion(_farm, _regions, (x, y), new Region(0, 0, 0)));
                _currentRegion++;
            }
        }

        return regions;
    }

    private static readonly (int x, int y)[] UpLeftRightDown = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    private Region ExploreRegion(char[,] farm, int[,] regions, (int x, int y) pos, Region region)
    {
        if (regions[pos.x, pos.y] == _currentRegion)
        {
            return region;
        }
        
        regions[pos.x, pos.y] = _currentRegion;

        var area = 1;
        var perimeter = 4;
        var sides = CountCorners(farm, regions, pos);

        foreach (var (x, y) in UpLeftRightDown)
        {
            (int x, int y) next = (pos.x + x, pos.y + y);
            if (next.x >= 0 && next.x < farm.GetLength(0) &&
                next.y >= 0 && next.y < farm.GetLength(1) &&
                farm[next.x, next.y] == farm[pos.x, pos.y])
            {
                var region2 = ExploreRegion(farm, regions, next, region);
                area += region2.Area;
                perimeter += region2.Perimeter;
                sides += region2.Sides;

                // Had neighbour in the region, so reduce perimeter by 1.
                perimeter--;
            }
        }

        return new Region(region.Area + area, region.Perimeter + perimeter, sides);
    }

    private int CountCorners(char[,] farm, int[,] regions, (int x, int y) pos)
    {
        var corners = 0;
        var plant = farm[pos.x, pos.y];
        var region = regions[pos.x, pos.y];

        for (var corner = 0; corner < 4; corner++)
        {
            (int x, int y) corner0 = corner switch
            {
                0 => (pos.x, pos.y - 1),
                1 => (pos.x + 1, pos.y),
                2 => (pos.x, pos.y + 1),
                3 => (pos.x - 1, pos.y),
                _ => throw new InvalidOperationException()
            };

            (int x, int y) corner1 = corner switch
            {
                0 => (pos.x + 1, pos.y - 1),
                1 => (pos.x + 1, pos.y + 1),
                2 => (pos.x - 1, pos.y + 1),
                3 => (pos.x - 1, pos.y - 1),
                _ => throw new InvalidOperationException()
            };

            (int x, int y) corner2 = corner switch
            {
                0 => (pos.x + 1, pos.y),
                1 => (pos.x, pos.y + 1),
                2 => (pos.x - 1, pos.y),
                3 => (pos.x, pos.y - 1),
                _ => throw new InvalidOperationException()
            };

            if (corner0.x >= 0 && corner0.x < farm.GetLength(0) &&
                corner0.y >= 0 && corner0.y < farm.GetLength(1) &&
                regions[corner0.x, corner0.y] == region)
            {
                continue; // already counted
            }

            if (corner2.x >= 0 && corner2.x < farm.GetLength(0) &&
                corner2.y >= 0 && corner2.y < farm.GetLength(1) &&
                regions[corner2.x, corner2.y] == region)
            {
                continue; // already counted
            }

            var bound0 = corner0.x < 0 || corner0.x >= farm.GetLength(0) || corner0.y < 0 || corner0.y >= farm.GetLength(1) || farm[corner0.x, corner0.y] != plant;
            var bound1 = corner1.x < 0 || corner1.x >= farm.GetLength(0) || corner1.y < 0 || corner1.y >= farm.GetLength(1) || farm[corner1.x, corner1.y] != plant;
            var bound2 = corner2.x < 0 || corner2.x >= farm.GetLength(0) || corner2.y < 0 || corner2.y >= farm.GetLength(1) || farm[corner2.x, corner2.y] != plant;

            // if bound0 and bound2 are true, then we have a corner pointing outwards.
            if (bound0 && bound2)
            {
                corners++;
            }
            else if (((bound0 && !bound1 && !bound2)
                      || (!bound0 && bound1 && !bound2)
                      || (!bound0 && !bound1 && bound2))
                     && regions[corner1.x, corner1.y] != region)
            {
                // inward corner
                corners++;
            }

        }

        return corners;
    }

    record Region(int Area, int Perimeter, int Sides);
}