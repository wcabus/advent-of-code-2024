using System.Drawing;
using Spectre.Console;
using Size = System.Drawing.Size;

namespace AdventOfCode.Cli;

public class Day15
{
    abstract class MapObject
    {
        protected MapObject(Point position)
        {
            Position = position;
        }

        public Point Position { get; set; }
        
        public abstract bool TryMove(Point direction);
    }

    class Wall(Point position, Point? position2) : MapObject(position)
    {
        public Point? Position2 { get; set; } = position2;
        
        public override bool TryMove(Point direction)
        {
            return false;
        }
    }

    class Box(Point position, Point? position2) : MapObject(position)
    {
        private static readonly Stack<Action> UndoActions = new();
        
        public Point? Position2 { get; set; } = position2;

        public override bool TryMove(Point direction)
        {
            var newPosition = new Point(Position.X + direction.X, Position.Y + direction.Y);
            var newPosition2 = Position2 is null ? (Point?)null : new Point(Position2.Value.X + direction.X, Position2.Value.Y + direction.Y);

            var hitMapObject = Map.TryGetValue(newPosition, out var mapObject);
            MapObject? mapObject2 = null;
            var hitMapObject2 = newPosition2 is not null && Map.TryGetValue(newPosition2.Value, out mapObject2);
            
            if (hitMapObject || hitMapObject2)
            {
                if (mapObject is Wall || mapObject2 is Wall)
                {
                    return false; // can't move into a wall
                }

                if (mapObject is Box box && mapObject2 is null && box != this)
                {
                    if (!box.TryMove(direction))
                    {
                        return false;
                    }
                    
                    // we moved the box, so we can move this box
                    PerformMove();
                    return true;
                }
                
                if (mapObject is null && mapObject2 is Box box2 && box2 != this)
                {
                    if (!box2.TryMove(direction))
                    {
                        return false;
                    }
                    
                    // we moved the box, so we can move this box
                    PerformMove();
                    return true;
                }
                
                if (mapObject is Box box3 && mapObject2 is Box box4)
                {
                    if (box3 == box4)
                    {
                        if (!box3.TryMove(direction))
                        {
                            return false;
                        }

                        // we moved the box, so we can move this box
                        PerformMove();
                        return true;
                    }

                    if (box3 == this)
                    {
                        if (!box4.TryMove(direction))
                        {
                            return false;
                        }

                        // we moved the box, so we can move this box
                        PerformMove();
                        return true;
                    }
                    
                    if (box4 == this)
                    {
                        if (!box3.TryMove(direction))
                        {
                            return false;
                        }

                        // we moved the box, so we can move this box
                        PerformMove();
                        return true;
                    }
                    
                    // two different boxes
                    var box3Moved = box3.TryMove(direction);
                    var box4Moved = box4.TryMove(direction);
                    if (box3Moved == false || box4Moved == false)
                    {
                        return false;
                    }
                    
                    PerformMove();
                    return true;
                }
            }
            
            // we didn't hit any map object, move the box
            PerformMove();
            return true;

            void PerformMove()
            {
                UndoActions.Push(() => UndoMove(direction));
                
                Map.Remove(Position);
                if (Position2.HasValue)
                {
                    Map.Remove(Position2.Value);
                }
                
                Map[newPosition] = this;
                Position = newPosition;
                
                if (Position2.HasValue)
                {
                    Position2 = new Point(Position2.Value.X + direction.X, Position2.Value.Y + direction.Y);
                    Map[Position2.Value] = this;
                }
            }
        }

        public void CommitMoves()
        {
            UndoActions.Clear();
        }

        public void UndoMoves()
        {
            while (UndoActions.TryPop(out var action))
            {
                action();
            }
            UndoActions.Clear();
        }
        
        private void UndoMove(Point direction)
        {
            Map.Remove(Position);
            if (Position2.HasValue)
            {
                Map.Remove(Position2.Value);
            }

            Position = new Point(Position.X - direction.X, Position.Y - direction.Y);
            Map[Position] = this;
                
            if (Position2.HasValue)
            {
                Position2 = new Point(Position2.Value.X - direction.X, Position2.Value.Y - direction.Y);
                Map[Position2.Value] = this;
            }
        }
    }

    class Robot(Point position) : MapObject(position)
    {
        public override bool TryMove(Point direction)
        {
            var newPosition = new Point(Position.X + direction.X, Position.Y + direction.Y);
            if (Map.TryGetValue(newPosition, out var mapObject))
            {
                if (mapObject is Wall)
                {
                    return false; // can't move into a wall
                }

                if (mapObject is Box box)
                {
                    if (!box.TryMove(direction))
                    {
                        box.UndoMoves();
                        return false;
                    }
                    box.CommitMoves();

                    // we moved the box, so we can move the robot
                    PerformMove();
                    return true; 
                }
            }
            
            // we didn't hit any map object, move the robot
            PerformMove();
            return true;

            void PerformMove()
            {
                var oldPosition = Position;
                Map.Remove(oldPosition);
                Map[newPosition] = this;
                Position = newPosition;
            }
        }
    }
    
    private static readonly Dictionary<Point, MapObject> Map = new();
    private readonly List<char> _moves = new();
    private Robot? _robot;
    
    public async ValueTask ParseDataAsync(string path, bool doubleWidth = false)
    {
        var readingMap = true;
        Map.Clear();
        _moves.Clear();
        
        var x = 0;
        var y = 0;
        
        await foreach (var line in Helpers.GetInput(path))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                readingMap = false;
                continue;
            }

            if (readingMap)
            {
                foreach (var c in line)
                {
                    var position = new Point(x, y);
                    MapObject? mapObject = c switch
                    {
                        '#' => new Wall(position, doubleWidth ? new Point(x + 1, y) : null),
                        'O' => new Box(position, doubleWidth ? new Point(x + 1, y) : null),
                        '@' => new Robot(position),
                        _ => null
                    };
                    
                    x++;
                    if (doubleWidth)
                    {
                        x++;
                    }
                    
                    if (mapObject is not null)
                    {
                        Map.Add(position, mapObject);
                        if (doubleWidth)
                        {
                            if (c != '@')
                            {
                                Map.Add(new Point(x - 1, y), mapObject);
                            }
                        }
                    }

                    if (mapObject is Robot robot)
                    {
                        _robot = robot;
                    }
                }

                y++;
                x = 0;
                continue;
            }
            
            // reading moves
            _moves.AddRange(line.Trim());
        }
    }

    public async ValueTask Task1()
    {
        if (_robot is null)
        {
            return;
        }

        DrawMap();
        foreach (var move in _moves)
        {
            _robot.TryMove(MoveToPoint(move));
            DrawMap();
        }
        
        Console.WriteLine(Map.Values.OfType<Box>().Sum(x => x.Position.Y * 100 + x.Position.X));
    }

    public async ValueTask Task2()
    {
        if (_robot is null)
        {
            return;
        }

        if (DrawEnabled)
        {
            AnsiConsole.Clear();
        }
        
        DrawMap();
        foreach (var move in _moves)
        {
            _robot.TryMove(MoveToPoint(move));
            DrawMap();
        }
        
        Console.WriteLine(Map.Values.OfType<Box>().Distinct().Sum(box =>
        {
            // var x = box.Position2.HasValue ? Math.Min(box.Position.X, box.Position2.Value.X) : box.Position.X;
            // var y = box.Position2.HasValue ? Math.Min(box.Position.Y, box.Position2.Value.Y) : box.Position.Y;
            //
            // return y * 100 + x;
            return box.Position.Y * 100 + box.Position.X;
        }));
    }

    private static void DrawMap()
    {
        if (!DrawEnabled)
        {
            return;
        }
        
        AnsiConsole.Cursor.SetPosition(0, 0);
        
        var maxX = Map.Values.Select(x => x.Position.X).Max();
        var maxY = Map.Values.Select(x => x.Position.Y).Max();
        var canvas = new Canvas(maxX + 2, maxY + 1);
        
        foreach (var mapObject in Map.Values)
        {
            canvas.SetPixel(mapObject.Position.X, mapObject.Position.Y, mapObject switch
            {
                Wall => Spectre.Console.Color.Red,
                Box => Spectre.Console.Color.White,
                Robot => Spectre.Console.Color.Green,
                _ => Spectre.Console.Color.Black
            });

            if (mapObject is Box { Position2: not null } box)
            {
                canvas.SetPixel(box.Position2.Value.X, box.Position2.Value.Y, Spectre.Console.Color.White);
            }
            
            if (mapObject is Wall { Position2: not null } wall)
            {
                canvas.SetPixel(wall.Position2.Value.X, wall.Position2.Value.Y, Spectre.Console.Color.Red);
            }
        }
        
        AnsiConsole.Write(canvas);
    }

    public static bool DrawEnabled = false;
    
    private static Point MoveToPoint(char move)
    {
        return move switch
        {
            '^' => new Point(0, -1),
            'v' => new Point(0, 1),
            '<' => new Point(-1, 0),
            '>' => new Point(1, 0),
            _ => Point.Empty
        };
    }
}