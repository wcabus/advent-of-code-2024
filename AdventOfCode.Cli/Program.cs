using System.Diagnostics;
using AdventOfCode.Cli;

var day = new Day14();
await day.ParseDataAsync(@"C:\temp\aoc\day14-input.txt");

var sw = Stopwatch.StartNew();
await day.Task1();
sw.Stop();
Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");

await day.ParseDataAsync(@"C:\temp\aoc\day14-input.txt");

sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");