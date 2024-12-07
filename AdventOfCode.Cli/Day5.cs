namespace AdventOfCode.Cli;

public class Day5
{
    private readonly List<(int a, int b)> _pageOrderingRules = new();
    private readonly List<List<int>> _updates = new();
    
    private async ValueTask ParseDataAsync()
    {
        var parseUpdates = false;
        await foreach (var line in Helpers.GetInput(@"C:\temp\aoc\day5-input.txt"))
        {
            if (string.IsNullOrEmpty(line))
            {
                parseUpdates = true;
                continue;
            }

            if (parseUpdates)
            {
                var update = new List<int>();
                update.AddRange(line.Split(',').Select(int.Parse));
                _updates.Add(update);
            }
            else
            {
                var pages = line.Split('|').Select(int.Parse).ToArray();
                _pageOrderingRules.Add((pages[0], pages[1]));
            }
        }
    }

    private bool UpdateIsValid(IList<int> pages, int index, bool swapWhenInvalid = false)
    {
        var currentPage = pages[index];
        if (index == 0)
        {
            if (_pageOrderingRules
                .Where(x => x.b == currentPage)
                .Any(x => pages.Contains(x.a)))
            {
                if (!swapWhenInvalid)
                {
                    return false;
                }

                var rule = _pageOrderingRules
                        .Where(x => x.b == currentPage)
                        .First(x => pages.Contains(x.a));
                    
                    var indexOfSecond = pages.IndexOf(rule.a);
                    pages[index] = rule.a;
                    pages[indexOfSecond] = rule.b;

                return false;
            }
        }
        else if (index == _pageOrderingRules.Count - 1)
        {
            if (_pageOrderingRules
                .Where(x => x.a == currentPage)
                .Any(x => pages.Contains(x.b)))
            {
                if (!swapWhenInvalid)
                {
                    return false;
                }

                var rule = _pageOrderingRules
                    .Where(x => x.a == currentPage)
                    .First(x => pages.Contains(x.b));
                    
                var indexOfSecond = pages.IndexOf(rule.b);
                pages[index] = rule.b;
                pages[indexOfSecond] = rule.a;
            }
        }
        else
        {
            if (_pageOrderingRules
                .Where(x => x.b == currentPage)
                .Any(x => pages.IndexOf(x.a) > index))
            {
                if (!swapWhenInvalid)
                {
                    return false;
                }

                var rule = _pageOrderingRules
                    .Where(x => x.b == currentPage)
                    .First(x => pages.IndexOf(x.a) > index);
                    
                var indexOfSecond = pages.IndexOf(rule.a);
                pages[index] = rule.a;
                pages[indexOfSecond] = rule.b;
                
                return false;
            }
            if (_pageOrderingRules
                .Where(x => x.a == currentPage)
                .Any(x => pages.IndexOf(x.b) != -1 && pages.IndexOf(x.b) < index))
            {
                if (!swapWhenInvalid)
                {
                    return false;
                }

                var rule = _pageOrderingRules
                    .Where(x => x.a == currentPage)
                    .First(x => pages.IndexOf(x.b) != -1 && pages.IndexOf(x.b) < index);
                    
                var indexOfSecond = pages.IndexOf(rule.b);
                pages[index] = rule.b;
                pages[indexOfSecond] = rule.a;
                
                return false;
            }
        }
        
        return true;
    }
    
    public async ValueTask Task1()
    {
        await ParseDataAsync();
        
        var correctlyOrderedUpdates = new List<List<int>>();
        foreach (var update in _updates)
        {
            var i = 0;
            var updatesAreValid = true;
            while (i < update.Count)
            {
                if (UpdateIsValid(update, i++)) continue;
                updatesAreValid = false;
                break;
            }

            if (updatesAreValid)
            {
                correctlyOrderedUpdates.Add(update);
            }
        }
        
        var middleOnes = 0;
        foreach (var update in correctlyOrderedUpdates)
        {
            middleOnes += update[(int)Math.Ceiling(update.Count / 2f) - 1];
        }
        Console.WriteLine(middleOnes);
    }
    
    public ValueTask Task2()
    {
        var incorrectlyOrderedUpdates = new List<List<int>>();
        foreach (var update in _updates)
        {
            var i = 0;
            while (i < update.Count)
            {
                if (UpdateIsValid(update, i++)) continue;
                incorrectlyOrderedUpdates.Add(update);
                break;
            }
        }
        
        // fix the incorrectly ordered updates...
        foreach (var update in incorrectlyOrderedUpdates)
        {
            var i = 0;
            while (i < update.Count)
            {
                if (UpdateIsValid(update, i++, true)) continue;
                
                i = 0;
            }
        }
        
        var middleOnes = 0;
        foreach (var update in incorrectlyOrderedUpdates)
        {
            middleOnes += update[(int)Math.Ceiling(update.Count / 2f) - 1];
        }
        Console.WriteLine(middleOnes);
        
        return ValueTask.CompletedTask;
    }
}