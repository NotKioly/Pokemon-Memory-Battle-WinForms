namespace PokemonMemoryBattle;

public sealed class ResourceManager : IDisposable
{
    public string Root
    {
        get;
    }
    private readonly Action<string> warn;
    private readonly Dictionary<string, Image> images = [];
    private readonly HashSet<string> warned = [];
    public ResourceManager(string root, Action<string> warning)
    {
        Root = root;
        warn = warning;
    }
    public void Warn(string message)
    {
        if (warned.Add(message))
            warn(message);
    }
    public void CheckDirectories()
    {
        foreach (var folder in new[] { "img", "bg" })
        if (!Directory.Exists(Path.Combine(Root, folder)))
            Warn($"Missing {folder}/ folder. Built-in card designs will be used; unavailable sound is skipped.");
    }
    public Image? LoadImage(string relative)
    {
        if (images.TryGetValue(relative, out var value))
            return value;
        try
        {
            using var stream = File.OpenRead(Path.Combine(Root, relative));
            using var original = Image.FromStream(stream);
            var copy = new Bitmap(original);
            images.Add(relative, copy);
            return copy;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or OutOfMemoryException) { Warn($"Cannot load {relative}. A drawn placeholder will be used. {ex.Message}"); return null; }
    }
    public void Dispose()
    {
        foreach (var image in images.Values)
            image.Dispose();
        images.Clear();
    }
}
