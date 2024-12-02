namespace AdventOfCode.Cli;

public class Day2
{
    private class Report
    {
        private readonly int[] _levels;

        public Report(string line)
        {
            _levels = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }

        public bool IsSafe()
        {
            return IsSafe(_levels);
        }
        
        public bool IsSafeUsingDampener()
        {
            if (IsSafe(_levels))
            {
                return true;
            }

            var i = 0;
            while (i < _levels.Length)
            {
                int[] levels = [.. _levels.Take(i), .. _levels.Skip(i + 1)];
                if (IsSafe(levels))
                {
                    return true;
                }

                i++;
            }

            return false;
        }

        private bool IsSafe(IReadOnlyList<int> levels)
        {
            var direction = levels[0] < levels[1] ? -1 : 1;
            var diffBetween1And3 = levels
                .Zip(levels.Skip(1))
                .Select(x => x.First - x.Second)
                .All(x => x is <= -1 and >= -3 or >= 1 and <= 3);
            
            var sameDirection = levels
                .Zip(levels.Skip(1))
                .Select(x => x.First < x.Second ? -1 : 1)
                .All(x => x == direction);
            
            return sameDirection && diffBetween1And3;
        }
    }
    
    public async ValueTask Task1()
    {
        var safe = 0;
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day2-input.txt"))
        {
            var report = new Report(line);
            if (report.IsSafe())
            {
                safe++;
            }
        }
        
        Console.WriteLine(safe);
    }
    
    public async ValueTask Task2()
    {
        var safe = 0;
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day2-input.txt"))
        {
            var report = new Report(line);
            if (report.IsSafeUsingDampener())
            {
                safe++;
            }
        }
        
        Console.WriteLine(safe);
    }
}