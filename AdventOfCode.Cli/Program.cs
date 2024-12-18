using System.Diagnostics;
using AdventOfCode.Cli;

// const bool testinput = true;
// const string path = @"C:\temp\aoc\day18-testinput.txt";

const bool testinput = false;
const string path = @"C:\temp\aoc\day18-input.txt";

var day = new Day18();
await day.ParseDataAsync(path, testinput);

var sw = Stopwatch.StartNew();
await day.Task1();
sw.Stop();
Console.WriteLine($"Task 1: {sw.ElapsedMilliseconds}ms");

await day.ParseDataAsync(path, testinput);

sw.Restart();
await day.Task2();
sw.Stop();
Console.WriteLine($"Task 2: {sw.ElapsedMilliseconds}ms");