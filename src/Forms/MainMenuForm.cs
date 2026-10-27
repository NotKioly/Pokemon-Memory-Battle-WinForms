using static PokemonMemoryBattle.LocalizationManager;
namespace PokemonMemoryBattle;
public sealed class MainMenuForm : LocalizedForm
{
    private readonly AudioManager audio;
    private readonly LeaderboardService leaderboard;
    private readonly ComboBox difficulty = new() { Name = "DifficultySelector", DropDownStyle = ComboBoxStyle.DropDownList, Width = 260 };
    private readonly GameButton sound;
    public MainMenuForm() : base("AppTitle")
    {
        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(32) };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        Controls.Add(body);
        var left = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        body.Controls.Add(left);
        left.Controls.Add(LocalLabel("FieldNotes", 12, Theme.Accent));
        left.Controls.Add(LocalLabel("Logo", 30));
        left.Controls.Add(LocalLabel("Tagline", 16));
        var description = LocalLabel("MenuDescription", 11, Theme.Muted);
        description.AutoSize = false;
        description.Size = new Size(410, 55);
        left.Controls.Add(description);
        var art = new Panel { Width = 380, Height = 200, Margin = new Padding(10, 20, 10, 10) };
        art.Paint += (_, e) =>
        {
            using var resources = new ResourceManager(AppContext.BaseDirectory, _ => { });
            for (int i = 0; i < 3; i++)
            {
                var image = resources.LoadImage($"img/{i}.png");
                if (image != null)
                    e.Graphics.DrawImage(image, new Rectangle(i * 120, 24 + (i % 2) * 20, 120, 120));
            }
        };
        left.Controls.Add(art);
        var right = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(8, 12, 0, 0) };
        body.Controls.Add(right);
        right.Controls.Add(LocalLabel("ChooseDifficulty", 12, Theme.Accent));
        difficulty.Items.AddRange(Difficulty.All);
        difficulty.SelectedIndex = 0;
        right.Controls.Add(difficulty);
        using var audioResources = new ResourceManager(AppContext.BaseDirectory, Warn);
        audio = new AudioManager(audioResources);
        leaderboard = new LeaderboardService(AppContext.BaseDirectory, Warn);
        right.Controls.Add(LocalButton("StartGame", (_, _) => OpenGame()));
        right.Controls.Add(LocalButton("Leaderboard", (_, _) => { using var f = new LeaderboardForm(leaderboard); f.ShowDialog(this); }));
        right.Controls.Add(LocalButton("Help", (_, _) => { using var f = new HelpForm(false); f.ShowDialog(this); }));
        right.Controls.Add(LocalButton("About", (_, _) => { using var f = new HelpForm(true); f.ShowDialog(this); }));
        right.Controls.Add(LocalButton("Settings", (_, _) => { using var f = new SettingsForm(); f.ShowDialog(this); }));
        sound = LocalButton("SoundOn", (_, _) => { audio.Toggle(); ApplyLocalization(); });
        right.Controls.Add(sound);
        right.Controls.Add(LocalButton("Exit", (_, _) => { if (ConfirmationForm.Ask(this, "ConfirmExit")) Close(); }));
        Activated += (_, _) => ApplyLocalization();
        FormClosed += (_, _) => audio.Dispose();
        ApplyLocalization();
    }
    private void OpenGame()
    {
        Hide();
        audio.SetActive(false);
        using var game = new GameForm((Difficulty)difficulty.SelectedItem!, audio, leaderboard);
        game.ShowDialog(this);
        Show();
        audio.SetActive(true);
    }
    public override void ApplyLocalization()
    {
        base.ApplyLocalization();
        if (sound != null)
            DynamicText(sound, GetString(audio.Enabled ? "SoundOn" : "SoundOff"));
        int selected = difficulty.SelectedIndex;
        difficulty.BeginUpdate();
        difficulty.Items.Clear();
        difficulty.Items.AddRange(Difficulty.All);
        difficulty.SelectedIndex = selected >= 0 ? selected : 0;
        difficulty.AccessibleName = GetString("ChooseDifficulty");
        difficulty.EndUpdate();
    }
    internal static void Warn(string message) => MessageBox.Show(message, GetString("WarningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
