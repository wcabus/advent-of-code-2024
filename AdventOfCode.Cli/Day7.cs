namespace AdventOfCode.Cli;

public class Day7
{
    record struct Equation(long TargetResult, IReadOnlyList<int> Operands, bool EnableTernary = false)
    {
        public bool Resolves()
        {
            var result = Operands[0];
            if (Solve(result, 1, (a, b) => a + b))
            {
                return true;
            }
            
            if (Solve(result, 1, (a, b) => a * b))
            {
                return true;
            }
            
            if (EnableTernary && Solve(result, 1, (a, b) => long.Parse($"{a}{b}")))
            {
                return true;
            }

            return false;
        }

        private bool Solve(long result, int index, Func<long, long, long> solver)
        {
            result = solver(result, Operands[index]);
            if (index == Operands.Count - 1)
            {
                return result == TargetResult;
            }
            
            if (Solve(result, index + 1, (a, b) => a + b))
            {
                return true;
            }
            
            if (Solve(result, index + 1, (a, b) => a * b))
            {
                return true;
            }
            
            if (EnableTernary && Solve(result, index + 1, (a, b) => long.Parse($"{a}{b}")))
            {
                return true;
            }
            
            return false;
        }
    }
    
    private async IAsyncEnumerable<Equation> ParseDataAsync(bool enableTernary = false)
    {
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day7-input.txt"))
        {
            var targetResult = long.Parse(line[..(line.IndexOf(':'))]);
            var operands = line[(line.IndexOf(':') + 2)..].Split(' ').Select(int.Parse).ToArray();
            yield return new Equation(targetResult, operands, enableTernary);
        }
    }
    
    public async ValueTask Task1()
    {
        var sum = 0L;
        await foreach (var equation in ParseDataAsync())
        {
            if (equation.Resolves())
            {
                sum += equation.TargetResult;
            }
        }

        Console.WriteLine(sum);
    }

    public async ValueTask Task2()
    {
        var sum = 0L;
        await foreach (var equation in ParseDataAsync(true))
        {
            if (equation.Resolves())
            {
                sum += equation.TargetResult;
            }
        }

        Console.WriteLine(sum);
    }
}