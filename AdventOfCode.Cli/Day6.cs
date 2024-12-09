namespace AdventOfCode.Cli;

public class Day6
{
    private static char[,]? _grid;
    private static char[,]? _tempGrid;
    
    private async ValueTask ParseDataAsync()
    {
        var lines = await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day6-input.txt");
        _grid = new char[lines.Length, lines[0].Length];
        _tempGrid = new char[lines.Length, lines[0].Length];

        var row = 0;
        foreach (var line in lines)
        {
            for (var col = 0; col < line.Length; col++)
            {
                _grid[row, col] = line[col];
            }

            row++;
        }
    }
    
    public async ValueTask Task1()
    {
        await ParseDataAsync();

        var guard = FindGuard();
        if (guard.Direction == ' ')
        {
            Console.WriteLine("Guard not found!");
            return;
        }

        guard.StartMoving();
        
        Console.WriteLine(guard.PositionsHeld);
    }

    private Guard FindGuard()
    {
        for (var row = 0; row < _grid!.GetLength(0); row++)
        {
            for (var col = 0; col < _grid.GetLength(1); col++)
            {
                if (_grid[row, col] == '#' || _grid[row, col] == '.') continue;
                return new Guard(row, col, _grid[row, col]);
            }
        }
        
        return new Guard(-1, -1, ' ');
    }

    public async ValueTask Task2()
    {
        await ParseDataAsync();

        var guard = FindGuard();
        if (guard.Direction == ' ')
        {
            Console.WriteLine("Guard not found!");
            return;
        }

        guard.StartMoving();
        
        // grid now contains all the positions we've been in
        var numberOfLoops = guard.LookForLoops();
        Console.WriteLine(numberOfLoops);
    }

    record struct Guard(int Row, int Col, char Direction)
    {
        public int PositionsHeld { get; private set; }
        public bool LoopDetected { get; private set; }
        
        public void StartMoving()
        {
            PositionsHeld = 1;
            var gridPositions = new Dictionary<Point, GridPosition>();
            
            var row = Row;
            var col = Col;
            var direction = Direction;
            _grid![row, col] = 'Y'; // we've been here and started here
            gridPositions.Add(new Point(row, col), new GridPosition(direction));

            var maxRow = _grid!.GetLength(0);
            var maxCol = _grid!.GetLength(1);

            while (true)
            {
                var nextRow = row;
                var nextCol = col;
                
                switch (direction)
                {
                    case '^':
                        nextRow = row - 1;
                        break;
                    case 'v':
                        nextRow = row + 1;
                        break;
                    case '>':
                        nextCol = col + 1;
                        break;
                    case '<':
                        nextCol = col - 1;
                        break;
                }

                if (nextRow < 0 || nextRow >= maxRow || nextCol < 0 || nextCol >= maxCol)
                {
                    break;
                }

                if (_grid[nextRow, nextCol] == '.' || _grid[nextRow, nextCol] == 'X' || _grid[nextRow, nextCol] == 'Y')
                {
                    if (gridPositions.TryGetValue(new Point(nextRow, nextCol), out var gridPosition))
                    {
                        gridPosition.Visit(direction);
                        if (gridPosition.DuplicateVisits)
                        {
                            LoopDetected = true;
                            break;
                        }
                    }
                    else
                    {
                        gridPositions.Add(new Point(nextRow, nextCol), new GridPosition(direction));
                    }
                    
                    if (_grid[nextRow, nextCol] == '.')
                    {
                        _grid[nextRow, nextCol] = 'X';
                        PositionsHeld++;
                    }

                    row = nextRow;
                    col = nextCol;
                    continue;
                }
                
                // obstruction hit
                direction = direction switch
                {
                    '^' => '>',
                    '>' => 'v',
                    'v' => '<',
                    '<' => '^',
                    _ => ' '
                };
            }
        }

        public int LookForLoops()
        {
            InitializeTempGrid();
            var potentialLoops = GetPotentialLoopPositions().ToArray();
            var numberOfLoops = 0;
            
            foreach (var potentialLoop in potentialLoops)
            {
                _grid![potentialLoop.Row, potentialLoop.Col] = 'O';
                StartMoving();
                if (LoopDetected)
                {
                    numberOfLoops++;
                }
                
                ResetGrid();
                LoopDetected = false;
            }
            
            return numberOfLoops;
        }

        private IEnumerable<(int Row, int Col)> GetPotentialLoopPositions()
        {
            for (var row = 0; row < _tempGrid!.GetLength(0); row++)
            {
                for (var col = 0; col < _tempGrid.GetLength(1); col++)
                {
                    if (_tempGrid[row, col] == 'X')
                    {
                        yield return (row, col);
                    }
                }
            }
        }

        private void InitializeTempGrid()
        {
            for (var row = 0; row < _grid!.GetLength(0); row++)
            {
                for (var col = 0; col < _grid.GetLength(1); col++)
                {
                    _tempGrid![row, col] = _grid[row, col];
                }
            }
        }
        
        private void ResetGrid()
        {
            for (var row = 0; row < _grid!.GetLength(0); row++)
            {
                for (var col = 0; col < _grid.GetLength(1); col++)
                {
                    _grid[row, col] = _tempGrid![row, col];
                }
            }
        }
    }

    readonly record struct Point(int Row, int Col); 
    
    class GridPosition
    {
        public GridPosition(char initialDirection)
        {
            Visit(initialDirection);
        }

        public void Visit(char direction)
        {
            switch (direction)
            {
                case '^': 
                    Up++;
                    break;
                case 'v': 
                    Down++;
                    break;
                case '>': 
                    Right++;
                    break;
                case '<': 
                    Left++;
                    break;
            }
        }
        
        public bool DuplicateVisits => Up > 1 || Down > 1 || Right > 1 || Left > 1;
        
        public int Up { get; private set; }
        public int Down { get; private set; }
        public int Left { get; private set; }
        public int Right { get; private set; }
    }
}