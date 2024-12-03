using System.Text.RegularExpressions;

namespace AdventOfCode.Cli;

public partial class Day3
{
    public async ValueTask Task1()
    {
        var mul = 0;
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day3-input.txt"))
        {
            var matches = MulMatcher().EnumerateMatches(line);
            while (matches.MoveNext())
            {
                var mulOperands = line[(matches.Current.Index + 4)..(matches.Current.Index + matches.Current.Length - 1)].Split(',');
                mul += int.Parse(mulOperands[0]) * int.Parse(mulOperands[1]);
            }
        }
        
        Console.WriteLine(mul);
    }
    
    public async ValueTask Task2()
    {
        var mul = 0;
        var enabled = true;
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day3-input.txt"))
        {
            var matches = ExtendedMulMatcher().EnumerateMatches(line);
            while (matches.MoveNext())
            {
                var match = line[matches.Current.Index..(matches.Current.Index + matches.Current.Length)];
                if (match == "don't()")
                {
                    enabled = false;
                }
                else if (match == "do()")
                {
                    enabled = true;
                }
                else if (enabled)
                {
                    var mulOperands = line[(matches.Current.Index + 4)..(matches.Current.Index + matches.Current.Length - 1)].Split(',');
                    mul += int.Parse(mulOperands[0]) * int.Parse(mulOperands[1]);
                }
            }
        }
        
        Console.WriteLine(mul);
    }

    [GeneratedRegex(@"mul\(\d{1,3},\d{1,3}\)", RegexOptions.IgnoreCase)]
    private static partial Regex MulMatcher();
    
    [GeneratedRegex(@"(do\(\))|(don't\(\))|(mul\(\d{1,3},\d{1,3}\))", RegexOptions.IgnoreCase)]
    private static partial Regex ExtendedMulMatcher();
}