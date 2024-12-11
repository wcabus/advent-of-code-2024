namespace AdventOfCode.Cli;

public class Day11
{
    private static Dictionary<long, long> _workInstance = new();
    
    private static long BlinkTimes(Dictionary<long, long> stones, int times)
    {
        while (times > 0)
        {
            _workInstance.Clear();
            var newStones = _workInstance;
            
            foreach (var stone in stones)
            {
                // Every time we'd encounter a 0 in the original dictionary, add the number of 0-value stones to the 1-value stones instead
                if (stone.Key == 0)
                {
                    newStones.TryAdd(1, 0);
                    newStones[1] += stone.Value;
                    continue;
                }

                // Every time we encounter the same even-length stone, split it in half and add the number of stones to each half
                var numberOfDigits = CountNumberOfDigits(stone.Key);
                if (numberOfDigits % 2 == 0)
                {
                    var factor = 10;
                    numberOfDigits /= 2;
                    while (--numberOfDigits > 0)
                    {
                        factor *= 10;
                    }
                    
                    var leftStone = stone.Key / factor;
                    var rightStone = stone.Key % factor;

                    newStones.TryAdd(leftStone, 0);
                    newStones[leftStone] += stone.Value;

                    newStones.TryAdd(rightStone, 0);
                    newStones[rightStone] += stone.Value;

                    continue;
                }

                // same for the rest, just add the number of stones to the new dictionary for the new value
                newStones.TryAdd(stone.Key * 2024, 0);
                newStones[stone.Key * 2024] += stone.Value;
            }
            
            (_workInstance, stones) = (stones, newStones);
            times--;
        }

        return stones.Sum(x => x.Value);
    }

    private static int CountNumberOfDigits(long value)
    {
        var digits = 1;
        while (value >= 10)
        {
            digits++;
            value /= 10;
        }

        return digits;
    }


    private async ValueTask<Dictionary<long, long>> ParseDataAsync()
    {
        return (await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day11-input.txt"))[0]
            .Split(' ')
            .Select(long.Parse)
            .Select(x => new KeyValuePair<long, long>(x, 1))
            .ToDictionary();
    }

    public async ValueTask Task1()
    {
        var stones = await ParseDataAsync();
        
        Console.WriteLine(BlinkTimes(stones, 25));
    }
    
    public async ValueTask Task2()
    {
        var stones = await ParseDataAsync();

        Console.WriteLine(BlinkTimes(stones, 75));
    }
}