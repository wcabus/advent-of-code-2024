using System.Diagnostics;
using AdventOfCode.Cli;

var day = new Day12();

var sw = Stopwatch.StartNew();
await day.Task1();
sw.Stop();
Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");

sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");