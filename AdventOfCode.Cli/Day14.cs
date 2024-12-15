using System.Drawing;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

namespace AdventOfCode.Cli;

public class Day14
{
    public Day14(bool sampleInput = false)
    {
        if (sampleInput)
        {
            Robot.MaxX = 11;
            Robot.MaxY = 7;
        }
    }
    
    private readonly List<Robot> _robots = new();
    
    readonly record struct Point(int X, int Y);

    record Robot(Point Origin, Point Velocity)
    {
        private Point _position = Origin;
        
        public static int MinX = 0;
        public static int MaxX = 101;
        public static int MinY = 0;
        public static int MaxY = 103;
        
        public Point Origin { get; init; } = Origin;
        public Point Position => _position;
        
        public void Move()
        {
            var newX = WrapAround(_position.X + Velocity.X, MinX, MaxX);
            var newY = WrapAround(_position.Y + Velocity.Y, MinY, MaxY);

            _position = new Point(newX, newY);
        }

        private int WrapAround(int value, int min, int max)
        {
            if (value >= min && value < max)
            {
                return value;
            }
            
            if (value < min)
            {
                return max + min + value;
            }
            
            return min + (value - max);
        }
    }
    
    public async ValueTask ParseDataAsync(string path)
    {
        _robots.Clear();
        
        await foreach (var line in Helpers.GetInput(path))
        {
            var positionData = line[2..line.IndexOf(' ')].Split(',').Select(int.Parse).ToArray();
            var position = new Point(positionData[0], positionData[1]);
            
            var velocityData = line[(line.IndexOf(' ') + 3)..].Split(',').Select(int.Parse).ToArray();
            var velocity = new Point(velocityData[0], velocityData[1]);
            _robots.Add(new Robot(position, velocity));
        }
    }

    private int CountRobotsPerQuadrant()
    {
        // 11, 7
        // halfx == 5
        // halfy == 3
        var halfX = (int)Math.Floor(Robot.MaxX / 2f);
        var halfY = (int)Math.Floor(Robot.MaxY / 2f);

        var quadrant1 = new Rectangle(0, 0, halfX, halfY);
        var quadrant2 = new Rectangle(halfX + 1, 0, halfX, halfY);
        var quadrant3 = new Rectangle(0, halfY + 1, halfX, halfY);
        var quadrant4 = new Rectangle(halfX + 1, halfY + 1, halfX, halfY);

        var q1c = 0;
        var q2c = 0;
        var q3c = 0;
        var q4c = 0;

        foreach (var robot in _robots)
        {
            if (quadrant1.Contains(robot.Position.X, robot.Position.Y))
            {
                q1c++;
                continue;
            }
            
            if (quadrant2.Contains(robot.Position.X, robot.Position.Y))
            {
                q2c++;
                continue;
            }
            
            if (quadrant3.Contains(robot.Position.X, robot.Position.Y))
            {
                q3c++;
                continue;
            }
            
            if (quadrant4.Contains(robot.Position.X, robot.Position.Y))
            {
                q4c++;
            }
        }

        return q1c * q2c * q3c * q4c;
    }

    public async ValueTask Task1()
    {
        await Parallel.ForEachAsync(_robots, (robot, _) =>
        {
            for (var i = 0; i < 100; i++)
            {
                robot.Move();
            }
            
            return ValueTask.CompletedTask;
        });

        Console.WriteLine(CountRobotsPerQuadrant());
    }

    public async ValueTask Task2()
    {
        var seconds = 0;
        var cts = new CancellationTokenSource();
       
        
        var safetyScores = new List<(int safety, int seconds)>();
        while (seconds < Robot.MaxX * Robot.MaxY)
        {
            try
            {
                seconds++;
                await Parallel.ForEachAsync(_robots, cts.Token, (robot, _) =>
                {
                    robot.Move();
                    return ValueTask.CompletedTask;
                });
                var safety = CountRobotsPerQuadrant();
                safetyScores.Add((safety, seconds));

                if (safety < 123128280)
                {
                    DrawRobots(seconds);
                }
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        foreach (var tuple in safetyScores.OrderBy(x => x.safety).ThenBy(x => x.seconds))
        {
            Console.WriteLine($"safety of {tuple.safety} after {tuple.seconds} seconds ");
        }
    }

    private void DrawRobots(int seconds)
    {
        using var bmp = new Bitmap(Robot.MaxX, Robot.MaxY);
        foreach (var robot in _robots)
        {
            bmp.SetPixel(robot.Position.X, robot.Position.Y, Color.Green);
        }
        
        bmp.Save(Path.Combine(@"C:\Temp\AOC\day14", $"frame-{seconds}.png"), ImageFormat.Png);
    }
}