namespace AdventOfCode.Cli;

public static class Helpers
{
    public static async IAsyncEnumerable<string> GetInput(string filename)
    {
        using var reader = new StreamReader(filename);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line is null)
            {
                yield break;
            }

            yield return line;
        }
    }
    
    public static async Task<string[]> GetAllLinesAsync(string filename)
    {
        return await File.ReadAllLinesAsync(filename).ConfigureAwait(false);
    }
}