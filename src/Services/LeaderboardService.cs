using System.Text.Json;
namespace PokemonMemoryBattle;

public sealed class LeaderboardService(string root, Action<string> warn, string? fallbackRoot = null)
{
    public string FilePath { get; private set; } = Path.Combine(root, "thanhtichs.txt");
    public static string ValidateName(string name)
    {
        name = (name ?? "").Trim();
        if (name.Length < 1 || name.Length > 24 || name.Any(char.IsControl))
            throw new ArgumentException("Enter a name of 1–24 characters without control characters.");
        return name;
    }
    private void EnsureFile()
    {
        if (!File.Exists(FilePath))
        {
            warn("Leaderboard file is missing. An empty thanhtichs.txt will be created.");
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, "");
        }
    }
    private void Fallback()
    {
        FilePath = Path.Combine(fallbackRoot ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PokemonMemoryBattle"), "thanhtichs.txt");
        warn($"Leaderboard access failed. Using writable location: {FilePath}");
    }
    public List<Result> Load()
    {
        try
        {
            EnsureFile();
            return Read();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { Fallback(); try { EnsureFile(); return Read(); } catch (Exception e) when (e is IOException or UnauthorizedAccessException) { warn($"Cannot read leaderboard: {e.Message}"); return []; } }
    }
    private List<Result> Read()
    {
        var list = new List<Result>();
        int invalid = 0;
        foreach (var line in File.ReadLines(FilePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            try
            {
                var r = JsonSerializer.Deserialize<Result>(line);
                if (r == null || r.Score < 0 || r.Moves < 0 || r.CompletionSeconds < 0 || r.Date == default || !Difficulty.All.Any(d => d.Name == r.Difficulty))
                    throw new FormatException();
                ValidateName(r.Name);
                list.Add(r);
            }
            catch (Exception e) when (e is JsonException or ArgumentException or FormatException) { invalid++; }
        }
        if (invalid > 0)
            warn($"Skipped {invalid} invalid leaderboard line(s).");
        return list.OrderByDescending(r => r.Score).ThenBy(r => r.Moves).ThenBy(r => r.CompletionSeconds).ThenBy(r => r.Date).ToList();
    }
    public bool Save(Result result)
    {
        try
        {
            ValidateName(result.Name);
            EnsureFile();
            File.AppendAllText(FilePath, JsonSerializer.Serialize(result) + Environment.NewLine);
            return true;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { Fallback(); try { EnsureFile(); File.AppendAllText(FilePath, JsonSerializer.Serialize(result) + Environment.NewLine); return true; } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { warn($"Result could not be saved: {ex.Message}"); return false; } }
    }
}
