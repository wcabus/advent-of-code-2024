using System.Diagnostics;
using AdventOfCode.Cli;

// const string path = @"C:\temp\aoc\day16-testinput1.txt"; // 7036, 45
// const string path = @"C:\temp\aoc\day16-testinput2.txt"; // 11048, 64
const string path = @"C:\temp\aoc\day16-input.txt";

var day = new Day16();
await day.ParseDataAsync(path);

var sw = Stopwatch.StartNew();
await day.Task1();
sw.Stop();
Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");

await day.ParseDataAsync(path);

sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");