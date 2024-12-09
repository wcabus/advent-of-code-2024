namespace AdventOfCode.Cli;

public class Day9
{
    private Disk? _disk;
    private Disk2? _disk2;

    class Disk
    {
        private int _fileId = -1;
        private readonly List<Sector> _storage = new();

        private void AddFile(int blocks)
        {
            var fileId = ++_fileId;
            _storage.AddRange(Enumerable.Repeat(new Sector(fileId), blocks));
        }

        private void AddFreeSpace(int blocks)
        {
            _storage.AddRange(Enumerable.Repeat(new Sector(-1), blocks));
        }

        public void Defragment()
        {
            var end = _storage.Count - 1;
            var pos = 0;
            while (pos < _storage.Count)
            {
                // skip until pos reaches free space
                if (_storage[pos].FileId != -1)
                {
                    pos++;
                    continue;
                }

                // find the first non-free space from the end
                while (_storage[end].FileId == -1 && end >= pos)
                {
                    end--;
                }

                if (end <= pos)
                {
                    break;
                }

                // swap free space with file data
                (_storage[pos], _storage[end]) = (_storage[end], _storage[pos]);
            }
        }
        
        public long Checksum()
        {
            var pos = 0;
            var checksum = 0L;
            while (pos < _storage.Count)
            {
                if (_storage[pos].FileId == -1)
                {
                    pos++;
                    continue;
                }
                
                checksum += _storage[pos].FileId * pos++;
            }
            
            return checksum;
        }
        
        public static Disk FromLine(string line)
        {
            var isFile = true;
            var disk = new Disk();
            foreach (var character in line)
            {
                var blocks = character - '0';
                if (isFile)
                {
                    disk.AddFile(blocks);
                }
                else
                {
                    disk.AddFreeSpace(blocks);
                }
                
                isFile = !isFile;
            }
            
            return disk;
        }
    }

    readonly record struct Sector(long FileId);
    readonly record struct Block(long FileId, int Size);

    class Disk2
    {
        private int _fileId = -1;
        private List<Block> _storage = new();
        
        private void AddFile(int size)
        {
            _storage.Add(new Block(++_fileId, size));
        }
        
        private void AddFreeSpace(int size)
        {
            _storage.Add(new Block(-1, size));
        }
        
        public void Defragment()
        {
            var fileIds = _storage.Where(x => x.FileId != -1).Select(x => x.FileId).Reverse().ToArray();
            foreach (var fileId in fileIds)
            {
                var file = _storage.Last(x => x.FileId == fileId);
                var indexOfFile = _storage.IndexOf(file);
                var indexOfFreeSpace = _storage.FindIndex(x => x.FileId == -1 && x.Size >= file.Size);
                if (indexOfFreeSpace == -1 || indexOfFreeSpace >= indexOfFile)
                {
                    continue; // attempt to move the next file
                }

                var freeSpace = _storage[indexOfFreeSpace];
                
                
                _storage[indexOfFreeSpace] = file;
                if (file.Size == freeSpace.Size)
                {
                    // easy swap
                    _storage[indexOfFile] = freeSpace;
                    continue;
                }

                // Insert free space behind the moved file which contains the remaining number of free space
                _storage.Insert(indexOfFreeSpace + 1, new Block(-1, freeSpace.Size - file.Size));
                // Insert the remaining free space where the file used to be (offset by 1)
                _storage[indexOfFile + 1] = file with { FileId = -1 };
            }
        }
        
        public long Checksum()
        {
            var pos = 0;
            var checksum = 0L;
            foreach (var block in _storage)
            {
                if (block.FileId == -1)
                {
                    pos += block.Size;
                    continue;
                }

                for (var i = 0; i < block.Size; i++)
                {
                    checksum += block.FileId * pos++;
                }
            }
            
            return checksum;
        }
        
        public static Disk2 FromLine(string line)
        {
            var isFile = true;
            var disk = new Disk2();
            foreach (var character in line)
            {
                var blocks = character - '0';
                if (isFile)
                {
                    disk.AddFile(blocks);
                }
                else
                {
                    disk.AddFreeSpace(blocks);
                }
                
                isFile = !isFile;
            }
            
            return disk;
        }
    }
    
    private async ValueTask ParseDataAsync()
    {
        var line = (await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day9-input.txt"))[0];
        _disk = Disk.FromLine(line);
    }
    
    private async ValueTask ParseDataAsync2()
    {
        var line = (await Helpers.GetAllLinesAsync(@"C:\temp\aoc\day9-input.txt"))[0];
        _disk2 = Disk2.FromLine(line);
    }
    
    public async ValueTask Task1()
    {
        await ParseDataAsync();

        _disk?.Defragment(); 

        Console.WriteLine(_disk?.Checksum() ?? 0);
    }

    public async ValueTask Task2()
    {
        await ParseDataAsync2();

        _disk2?.Defragment(); 

        Console.WriteLine(_disk2?.Checksum() ?? 0);
    }
}