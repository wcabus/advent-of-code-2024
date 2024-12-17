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

        private readonly List<long> _output = [];
        
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
                        var value = (int)Math.Floor(_registerA / Math.Pow(2, GetComboOperand(program[_instructionPointer + 1])));

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
                        _registerB ^= operand; // potentially (_registerB % 8) ^ (operand % 8) 
                        _instructionPointer += 2;
                        
                        break;

                    case BST:
                        // Combo operand MOD 8 => B
                        _registerB = GetComboOperand(program[_instructionPointer + 1]) % 8;
                        _instructionPointer += 2;
                        break;

                    case JNZ:
                        // Jump to the literal operand if A is not zero
                        if (_registerA != 0)
                        {
                            _instructionPointer = program[_instructionPointer + 1];
                            break;
                        }
                        
                        _instructionPointer += 2; // or 0!
                        break;
                    
                    case OUT:
                        // Combo operand MOD 8 => output comma separated
                        _output.Add(GetComboOperand(program[_instructionPointer + 1]) % 8);
                        _instructionPointer += 2;
                        break;
                    
                    default:
                        return; // halt
                }
            }
        }

        public IReadOnlyCollection<long> Output => _output;
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
        var result = -1L;
        var start = 4.526.000.000.000.000L;

        // for (var i = 2048; i <= 4096; i++)
        // {
        //     var computer = new Computer(_program, i, _registerB, _registerC);
        //     computer.RunCpu();
        //     Console.WriteLine($"A = {i}");
        //     Console.WriteLine($"Out = {string.Join(',', computer.Output)}");
        //     Console.WriteLine();
        // }
        
        Parallel.For<long>(start, long.MaxValue, () => 0, (i, state, _) =>
        {
            var computer = new Computer(_program, i, _registerB, _registerC);
            computer.RunCpu(state);
            if (computer.Output.Count == _program.Length && computer.Output.Zip(_program, (a, b) => a == b).All(x => x))
            {
                state.Break();
                return i;
            }
        
            return -1;
        },
        local =>
        {
            if (local != -1)
            {
                Interlocked.Exchange(ref result, local);
            }   
        });
        
        Console.WriteLine(result);
        return ValueTask.CompletedTask;
    }
}