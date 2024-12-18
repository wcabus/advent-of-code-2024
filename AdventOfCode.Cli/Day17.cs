using System.Collections.Concurrent;
using System.Numerics;

namespace AdventOfCode.Cli;

public class Day17
{
    private int _registerA;
    private int _registerB;
    private int _registerC;
    private int[] _program = [];

    class Computer(int[] program, long registerA, long registerB, long registerC)
    {
        private long _registerA = registerA;
        private long _registerB = registerB;
        private long _registerC = registerC;
        private int _instructionPointer;

        private readonly List<int> _output = [];
        
        // instructions
        private const int ADV = 0;
        private const int BXL = 1;
        private const int BST = 2;
        private const int JNZ = 3;
        private const int BXC = 4;
        private const int OUT = 5;
        private const int BDV = 6;
        private const int CDV = 7;

        private long GetComboOperand(int operand)
        {
            return operand switch
            {
                0 => operand,
                1 => operand,
                2 => operand,
                3 => operand,
                4 => _registerA,
                5 => _registerB,
                6 => _registerC,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public void RunCpu(ParallelLoopState? state = null)
        {
            while (_instructionPointer < program.Length && state is not { ShouldExitCurrentIteration: true })
            {
                var instruction = program[_instructionPointer];
                
                switch (instruction)
                {
                    case ADV:
                    case BDV:
                    case CDV:
                        // Divide A register value by combo operand
                        long value = _registerA >> (int)GetComboOperand(program[_instructionPointer + 1]);

                        // Store in A, B or C depending on the opcode
                        switch (instruction)
                        {
                            
                            case ADV:
                                _registerA = value;
                                break;
                            case BDV:
                                _registerB = value;
                                break;
                            default:
                                _registerC = value;
                                break;
                        }
                        
                        _instructionPointer += 2;
                        break;

                    case BXL:
                    case BXC:
                        // Perform bitwise XOR between register B and the literal operand or register C (depending on the opcode)
                        var operand = instruction == BXC ? _registerC : program[_instructionPointer + 1];
                        // Store in B
                        _registerB ^= operand; 
                        _instructionPointer += 2;
                        
                        break;

                    case BST:
                        // Combo operand MOD 8 => B
                        _registerB = GetComboOperand(program[_instructionPointer + 1]) & 7;
                        _instructionPointer += 2;
                        break;

                    case JNZ:
                        // Jump to the literal operand if A is not zero
                        if (_registerA != 0)
                        {
                            _instructionPointer = program[_instructionPointer + 1];
                            break;
                        }
                        
                        _instructionPointer += 2;
                        break;
                    
                    case OUT:
                        // Combo operand MOD 8 => output comma separated
                        _output.Add((int)(GetComboOperand(program[_instructionPointer + 1]) & 7));
                        _instructionPointer += 2;
                        break;
                    
                    default:
                        return; // halt
                }
            }
        }

        public IReadOnlyCollection<int> Output => _output;
    }

    public async ValueTask ParseDataAsync(string path)
    {
        var lines = await Helpers.GetAllLinesAsync(path);
        _registerA = int.Parse(lines[0][12..]);
        _registerB = int.Parse(lines[1][12..]);
        _registerC = int.Parse(lines[2][12..]);
        
        _program = lines[4][9..].Split(',').Select(int.Parse).ToArray();
    }

    public ValueTask Task1()
    {
        var computer = new Computer(_program, _registerA, _registerB, _registerC);
        computer.RunCpu();
        Console.WriteLine(string.Join(',', computer.Output));
        return ValueTask.CompletedTask;
    }

    public ValueTask Task2()
    {
        var shiftPerCycle = 0;
        for (var i = 0; i < _program.Length; i +=2 )
        {
            if (_program[i] == 0)
            {
                shiftPerCycle = _program[i + 1];
            }
        }
        var bitsToCheck = shiftPerCycle + 8;
        
        var dictionary = new ConcurrentDictionary<(long, int), HashSet<long>>();
        var solutions = FindA(0, 0).ToList();
        Console.WriteLine(solutions.Min());
        
        HashSet<long> FindA(long a, int programDigit)
        {
            if (dictionary.TryGetValue((a, programDigit), out var memory))
            {
                return memory;
            }
            
            if (programDigit >= _program.Length)
            {
                dictionary[(a, programDigit)] = [0];
                return [0];
            }
            
            var candidates = new HashSet<long>();
            var offset = (long)BitOperations.RoundUpToPowerOf2((ulong)a + 1);
            for (var i = a; i < 1 << bitsToCheck; i += offset)
            {
                var computer = new Computer(_program, i, _registerB, _registerC);
                computer.RunCpu();
                var output = computer.Output.First();
                if (output == _program[programDigit])
                {
                    var nextCandidates = FindA(i >> shiftPerCycle, programDigit + 1);
                    foreach (var c in nextCandidates)
                    {
                        var concatCandI = (c << shiftPerCycle) | i;
                        var secondComputer = new Computer(_program, concatCandI, _registerB, _registerC);
                        secondComputer.RunCpu();
                        var secondOutput = secondComputer.Output;
                        if (secondOutput.Zip(_program.Skip(programDigit), (o, p) => o == p).All(x => x))
                        {
                            candidates.Add(concatCandI);
                        }
                    }
                }
            }
            
            dictionary[(a, programDigit)] = candidates;
            return candidates;
        }
        
        return ValueTask.CompletedTask;
    }
}