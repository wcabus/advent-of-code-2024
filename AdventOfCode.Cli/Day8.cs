using System.Drawing;
using System.Text;

namespace AdventOfCode.Cli;

public class Day8
{
    private readonly List<Antenna> _antennae = new();
    private readonly List<Antinode> _antinodes = new();
    private readonly Lock _antinodesLock = new();
    
    private Rectangle _bounds = Rectangle.Empty;
    record Antenna(int Row, int Col, char Frequency);
    record struct Antinode(int Row, int Col);
    
    private async ValueTask ParseDataAsync()
    {
        var lines = await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day8-input.txt");
        
        _antennae.Clear();
        _antinodes.Clear();
        var row = 0;
        foreach (var line in lines)
        {
            for (var col = 0; col < line.Length; col++)
            {
                if (line[col] == '.')
                {
                    continue;
                }
                _antennae.Add(new Antenna(row, col, line[col]));
            }

            row++;
        }
        
        _bounds = new Rectangle(0, 0, lines[0].Length, lines.Length);
    }
    
    public async ValueTask Task1()
    {
        await ParseDataAsync();

        await Parallel.ForEachAsync(_antennae, async (antenna, _) =>
        {
            await CreateAntinodes(antenna);
        });

        Console.WriteLine(_antinodes.Count);
    }

    private async ValueTask CreateAntinodes(Antenna origin)
    {
        var candidates = _antennae.Where(x => x != origin && x.Frequency == origin.Frequency);
        
        await Parallel.ForEachAsync(candidates, (antenna, _) =>
        {
            var antinodeRow = origin.Row + (antenna.Row - origin.Row) * 2;
            var antinodeCol = origin.Col + (antenna.Col - origin.Col) * 2;
            if (_bounds.Contains(antinodeCol, antinodeRow))
            {
                lock (_antinodesLock)
                {
                    if (_antinodes.Any(x => x.Row == antinodeRow && x.Col == antinodeCol))
                    {
                        return ValueTask.CompletedTask;
                    }
                    _antinodes.Add(new Antinode(antinodeRow, antinodeCol));
                }
            }
            
            return ValueTask.CompletedTask;
        });
    }

    public async ValueTask Task2()
    {
        await ParseDataAsync();

        await Parallel.ForEachAsync(_antennae, async (antenna, _) =>
        {
            await CreateResonantAntinodes(antenna);
        });

        Console.WriteLine(_antinodes.Count);
    }

    private void Visualize()
    {
        var output = new StringBuilder();
        for (var row = 0; row < _bounds.Height; row++)
        {
            for (var col = 0; col < _bounds.Width; col++)
            {
                var antenna = _antennae.FirstOrDefault(x => x.Row == row && x.Col == col);
                if (antenna is not null)
                {
                    output.Append(antenna.Frequency);    
                }
                else if (_antinodes.Any(x => x.Row == row && x.Col == col))
                {
                    output.Append('#');
                }
                else
                {
                    output.Append('.');
                }
            }
            
            output.AppendLine();
        }
        
        Console.WriteLine(output.ToString());
    }

    private async ValueTask CreateResonantAntinodes(Antenna origin)
    {
        var candidates = _antennae.Where(x => x != origin && x.Frequency == origin.Frequency);
        
        await Parallel.ForEachAsync(candidates, (antenna, _) =>
        {
            var rowOffset = antenna.Row - origin.Row;
            var colOffset = antenna.Col - origin.Col;
            
            var antinodeRow = origin.Row + rowOffset;
            var antinodeCol = origin.Col + colOffset;
            while (_bounds.Contains(antinodeCol, antinodeRow))
            {
                lock (_antinodesLock)
                {
                    if (!_antinodes.Any(x => x.Row == antinodeRow && x.Col == antinodeCol))
                    {
                        _antinodes.Add(new Antinode(antinodeRow, antinodeCol));
                    }
                }
                
                antinodeRow += rowOffset;
                antinodeCol += colOffset;
            }
            
            return ValueTask.CompletedTask;
        });
    }
}