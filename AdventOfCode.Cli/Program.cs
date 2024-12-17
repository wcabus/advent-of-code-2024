using System.Diagnostics;
using AdventOfCode.Cli;

// const string path = @"C:\temp\aoc\day17-testinput.txt";
// const string path = @"C:\temp\aoc\day17-testinput2.txt";
const string path = @"C:\temp\aoc\day17-input.txt";

var day = new Day17();
await day.ParseDataAsync(path);

var sw = Stopwatch.StartNew();
// await day.Task1();
// sw.Stop();
// Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");
//
// await day.ParseDataAsync(path);

sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");