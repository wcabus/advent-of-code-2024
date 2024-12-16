namespace AdventOfCode.Cli;

public class Day16
{
    private Node[,] _map = new Node[0,0];
    private Node? _start;
    private Node? _goal;
    
    public async ValueTask ParseDataAsync(string path)
    {
        Array.Clear(_map);
        
        var lines = await Helpers.GetAllLinesAsync(path);
        _map = new Node[lines[0].Length, lines.Length];
        for (var row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (var col = 0; col < line.Length; col++)
            {
                _map[col, row] = new Node((col, row)) { Char = line[col] };
                if (line[col] == 'S')
                {
                    _start = _map[col, row];
                }
                
                if (line[col] == 'E')
                {
                    _goal = _map[col, row] ;
                }
            }
        }
    }

    record Node((int X, int Y) Position)
    {
        public char Char { get; set; }
        public long Cost { get; set; }
    }

    record struct NodeState(Node Current, int Direction, HashSet<Node> Visited);

    private long AStar(Node start, Node goal, bool isPartOne)
    {
        var atGoal = false;
        var openSet = new PriorityQueue<NodeState, long>();
        var minimumCost = long.MaxValue;
        var maxDistance = GetManhattanDistance(start, goal);
        var seats = new HashSet<(int, int)>();
        var minimumPathLength = int.MaxValue;
        
        openSet.Enqueue(new NodeState(start, 0, []), maxDistance);
        
        while (openSet.TryDequeue(out var state, out _))
        {
            var (current, direction, visited) = state;
            visited.Add(current);

            var neighbours = GetNeighbours(current, x => !visited.Contains(x) && x.Char is '.' or 'E').ToList();
            if (neighbours.Count == 0)
            {
                continue; // no neighbours, dead end
            }

            // keep moving in the same direction as long as we can
            while (neighbours.Count == 1 && GetDirection(current, neighbours[0]) == direction)
            {
                neighbours[0].Cost = current.Cost + 1;
                current = neighbours[0];
                visited.Add(current);

                if (current.Position == goal.Position) // we found the goal
                {
                    atGoal = true;
                    break;
                }

                neighbours = GetNeighbours(current, x => !visited.Contains(x) && x.Char is '.' or 'E').ToList();
            }

            if (atGoal)
            {
                if (current.Cost < minimumCost)
                {
                    minimumCost = current.Cost;
                    seats.Clear();
                }
                
                if (current.Cost == minimumCost && visited.Count < minimumPathLength)
                {
                    minimumPathLength = visited.Count;
                    seats.Clear();
                }

                if (current.Cost == minimumCost && visited.Count == minimumPathLength)
                {
                    foreach (var seat in visited.Select(c => c.Position))
                    {
                        seats.Add(seat);
                    }
                }

                atGoal = false;
                continue;
            }
            
            neighbours.ForEach(x =>
            {
                var newDirection = GetDirection(current, x);
                var newCost = newDirection == direction ? current.Cost + 1 : current.Cost + 1001;

                if (newCost > x.Cost && x.Cost != 0)
                {
                    return;
                }
                
                x.Cost = newCost;
                openSet.Enqueue(new NodeState(x, newDirection, [..visited]), newCost - maxDistance - GetManhattanDistance(x, goal));
            });
        }
        
        return isPartOne ? minimumCost : seats.Count;
    }

    private int GetManhattanDistance(Node a, Node b) => 
        Math.Abs(a.Position.X - b.Position.X) + Math.Abs(a.Position.Y - b.Position.Y);

    private IEnumerable<Node> GetNeighbours(Node node, Func<Node, bool> predicate)
    {
        var col = node.Position.X;
        var row = node.Position.Y;
        var directions = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
        foreach (var (dx, dy) in directions)
        {
            var x = col + dx;
            var y = row + dy;
            if (x < 0 || x >= _map.GetLength(0) || y < 0 || y >= _map.GetLength(1))
            {
                continue;
            }
            var neighbour = _map[x, y];
            if (predicate(neighbour))
            {
                yield return neighbour;
            }
        }
    }

    private int GetDirection(Node current, Node neighbour)
    {
        var diff = (current.Position.X - neighbour.Position.X, current.Position.Y - neighbour.Position.Y);
        return diff switch
        {
            (1, _) => 0, // E
            (_, -1) => 1, // S
            (-1, _) => 2, // W
            (_, 1) => 3, // N
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public ValueTask Task1()
    {
        Console.WriteLine(AStar(_start!, _goal!, true));
        return ValueTask.CompletedTask;
    }

    public ValueTask Task2()
    {
        Console.WriteLine(AStar(_start!, _goal!, false));
        return ValueTask.CompletedTask;
    }
}