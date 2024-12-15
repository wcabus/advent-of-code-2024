using System.Diagnostics;
using AdventOfCode.Cli;

Day15.DrawEnabled = true;

var day = new Day15();
await day.ParseDataAsync(@"C:\temp\aoc\day15-input.txt", true);

// var sw = Stopwatch.StartNew();
// await day.Task1();
// sw.Stop();
// Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");
//
// await day.ParseDataAsync(@"C:\temp\aoc\day15-testinput-small.txt");

var sw = Stopwatch.StartNew();
//sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");