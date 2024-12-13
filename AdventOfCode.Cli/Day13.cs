namespace AdventOfCode.Cli;

public class Day13
{
    readonly record struct Point(int X, int Y);
    
    class ClawMachine
    {
        public Point ButtonA { get; set; }
        public Point ButtonB { get; set; }
        
        public Point Prize { get; set; }

        public long Solve(long prizeX, long prizeY, bool limit = false)
        {
            var bPresses = (prizeY * ButtonA.X - prizeX * ButtonA.Y) / (-ButtonA.Y * ButtonB.X + ButtonB.Y * ButtonA.X);
            var aPresses = (prizeX - ButtonB.X * bPresses) / ButtonA.X;

            if (limit && (aPresses > 100 || bPresses > 100))
            {
                return 0;
            }
            
            if (aPresses * ButtonA.X + bPresses * ButtonB.X == prizeX &&
                aPresses * ButtonA.Y + bPresses * ButtonB.Y == prizeY)
            {
                return aPresses * 3 + bPresses;
            }

            return 0;
        }
    }
    
    private async ValueTask<List<ClawMachine>> ParseDataAsync()
    {
        var clawMachine = new ClawMachine();
        var machines = new List<ClawMachine>();
        
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day13-input.txt"))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("Button A: "))
            {
                var xAndY = line[10..].Split([',', ' ', 'X', 'Y', '+'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                
                clawMachine.ButtonA = new Point(xAndY[0], xAndY[1]);
            }
            else if (line.StartsWith("Button B: "))
            {
                var xAndY = line[10..].Split([',', ' ', 'X', 'Y', '+'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                
                clawMachine.ButtonB = new Point(xAndY[0], xAndY[1]);
            }
            else
            {
                // Prize: X=123, Y=456
                var xAndY = line[7..].Split([',', ' ', 'X', 'Y', '='], StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                
                clawMachine.Prize = new Point(xAndY[0], xAndY[1]);
                machines.Add(clawMachine);
                
                clawMachine = new ClawMachine();
            }
        }

        return machines;
    }

    public async ValueTask Task1()
    {
        var machines = await ParseDataAsync();
        
        Console.WriteLine(machines.Sum(x => x.Solve(x.Prize.X, x.Prize.Y, limit: true)));
    }

    public async ValueTask Task2()
    {
        var machines = await ParseDataAsync();
        
        Console.WriteLine(machines.Sum(x => x.Solve(x.Prize.X + 10000000000000L, x.Prize.Y + 10000000000000L)));
    }
}