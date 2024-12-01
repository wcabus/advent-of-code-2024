namespace AdventOfCode.Cli;

public class Day1
{
    public async ValueTask Task1()
    {
        var leftList = new List<int>();
        var rightList = new List<int>();
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day1-input.txt"))
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            leftList.Add(int.Parse(split[0]));
            rightList.Add(int.Parse(split[1]));
        }
        
        leftList.Sort();
        rightList.Sort();

        var distance = 0;
        for (var i = 0; i < leftList.Count; i++)
        {
            var left = leftList[i];
            var right = rightList[i];
            distance += left <= right ? right - left : left - right;
        }
        
        Console.WriteLine(distance);
    }
    
    public async ValueTask Task2()
    {
        var leftList = new List<int>();
        var rightList = new List<int>();
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day1-input.txt"))
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            leftList.Add(int.Parse(split[0]));
            rightList.Add(int.Parse(split[1]));
        }
        
        leftList.Sort();
        rightList.Sort();

        var similarity = 0;
        for (var i = 0; i < leftList.Count; i++)
        {
            var left = leftList[i];
            var right = rightList.Count(x => x == left);
            similarity += left * right;
        }
        
        Console.WriteLine(similarity);
    }
}