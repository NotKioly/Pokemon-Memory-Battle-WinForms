using System.Media;
namespace PokemonMemoryBattle;

public sealed class AudioManager : IDisposable
{
    private readonly Dictionary<string, SoundPlayer> players = [];
    private readonly System.Windows.Forms.Timer resume = new() { Interval = 550 };
    private readonly ResourceManager resources;
    private bool active = true;
    public bool Enabled { get; private set; } = true;
    public AudioManager(ResourceManager resources)
    {
        this.resources = resources;
        foreach (var file in new[] { "true.wav", "false.wav", "background.wav" })
        {
            try
            {
                var player = new SoundPlayer(Path.Combine(resources.Root, "bg", file));
                player.Load();
                players.Add(file, player);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.TimeoutException) { resources.Warn(LocalizationManager.GetString("AudioLoadFailed", file)); }
        }
        resume.Tick += (_, _) => { resume.Stop(); Background(); };
        Background();
    }
    private void StopAll()
    {
        foreach (var p in players.Values)
            p.Stop();
    }
    private void Play(string file, bool loop = false)
    {
        if (!players.TryGetValue(file, out var player))
            return;
        try
        {
            if (loop)
                player.PlayLooping();
            else
                player.Play();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.ComponentModel.Win32Exception) { resources.Warn(LocalizationManager.GetString("AudioPlayFailed", file)); players.Remove(file); player.Dispose(); }
    }
    private void Background()
    {
        if (Enabled && active)
            Play("background.wav", true);
    }
    public void Feedback(bool correct)
    {
        if (!Enabled)
            return;
        resume.Stop();
        StopAll();
        Play(correct ? "true.wav" : "false.wav");
        resume.Start();
    }
    public void Toggle()
    {
        Enabled = !Enabled;
        resume.Stop();
        StopAll();
        Background();
    }
    public void SetActive(bool value)
    {
        active = value;
        resume.Stop();
        StopAll();
        Background();
    }
    public void Dispose()
    {
        resume.Dispose();
        StopAll();
        foreach (var p in players.Values)
            p.Dispose();
    }
}
