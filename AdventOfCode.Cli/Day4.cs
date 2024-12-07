namespace AdventOfCode.Cli;

public class Day4
{
    public async ValueTask Task1()
    {
        var lines = await File.ReadAllLinesAsync(@"C:\temp\aoc\day4-input.txt");
        
        Console.WriteLine(CountXmas(lines));
    }
    
    public async ValueTask Task2()
    {
        var lines = await File.ReadAllLinesAsync(@"C:\temp\aoc\day4-input.txt");
        
        Console.WriteLine(CountCrossMAS(lines));
    }

    private int CountXmas(string[] lines)
    {
        const string word = "XMAS";
        const string word2 = "SAMX";
        return CountOccurrence(Horizontal(lines), word) +
               CountOccurrence(Horizontal(lines), word2) +
               CountOccurrence(Vertical(lines), word) +
               CountOccurrence(Vertical(lines), word2) +
               CountOccurrence(DiagonallyTopToBottom(lines), word) +
               CountOccurrence(DiagonallyTopToBottom(lines), word2) +
               CountOccurrence(DiagonallyBottomToTop(lines), word) +
               CountOccurrence(DiagonallyBottomToTop(lines), word2);
    }

    private int CountCrossMAS(string[] lines)
    {
        var count = 0;

        for (var y = 1; y < lines.Length - 1; y++)
        for (var x = 1; x < lines[y].Length - 1; x++)
        {
            if (lines[y][x] != 'A') continue;

            count += (lines[y - 1][x - 1], lines[y + 1][x + 1],
                    lines[y + 1][x - 1], lines[y - 1][x + 1]) 
                switch
                {
                    ('S', 'M', 'S', 'M') => 1,
                    ('S', 'M', 'M', 'S') => 1,
                    ('M', 'S', 'S', 'M') => 1,
                    ('M', 'S', 'M', 'S') => 1,
                    _ => 0
                };
        }

        return count;
    }

    private IEnumerable<char> Horizontal(string[] lines)
    {
        foreach (var line in lines)
        {
            foreach (var c in line)
            {
                yield return c;
            }

            yield return ' ';
        }
    }
    
    private IEnumerable<char> Vertical(string[] lines)
    {
        for (var x = 0; x < lines[0].Length; x++)
        {
            foreach (var line in lines)
            {
                yield return line[x];
            }

            yield return ' ';
        }
    }

    private IEnumerable<char> DiagonallyTopToBottom(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            foreach (var c in TraverseLine(y, 0))
            {
                yield return c;
            }
        }

        for (var x = 1; x < lines[0].Length; x++)
        {
            foreach (var c in TraverseLine(0, x))
            {
                yield return c;
            }
        }

        yield break;
        
        IEnumerable<char> TraverseLine(int y, int x)
        {
            while (y < lines.Length && x < lines[y].Length)
            {
                yield return lines[y][x];

                y++;
                x++;
            }

            yield return ' ';
        }
    }

    private IEnumerable<char> DiagonallyBottomToTop(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            foreach (var c in TraverseLineReverse(y, lines[y].Length - 1))
            {
                yield return c;
            }
        }

        for (var x = lines[0].Length - 2; x >= 0; x--)
        {
            foreach (var c in TraverseLineReverse(0, x))
            {
                yield return c;
            }
        }
        
        yield break;

        IEnumerable<char> TraverseLineReverse(int y, int x)
        {
            while (y < lines.Length && x >= 0)
            {
                yield return lines[y][x];

                y++;
                x--;
            }

            yield return ' ';
        }
    }
    
    private int CountOccurrence(IEnumerable<char> line, string word)
    {
        var occurrences = 0;
        var index = 0;
        
        foreach (var c in line)
        {
            if (c == word[index])
            {
                index++;
                if (index == word.Length)
                {
                    occurrences++;
                    index = 0;
                }
            }
            else
            {
                index = 0;
                if (c == word[0])
                {
                    index++;
                }
            }
        }
        
        return occurrences;
    }
}