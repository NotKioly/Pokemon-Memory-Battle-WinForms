namespace PokemonMemoryBattle;

public sealed class MainMenuForm : Form
{
    private readonly AudioManager audio;
    private readonly LeaderboardService leaderboard;
    private readonly ComboBox difficulty = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 230 };
    public MainMenuForm()
    {
        Theme.Form(this, "Pokémon Memory Battle");
        var body = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(44) };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        Controls.Add(body);
        var left = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        body.Controls.Add(left);
        left.Controls.Add(Theme.Label("FIELD NOTES   /   001", 12, Theme.Accent));
        left.Controls.Add(Theme.Label("POKÉMON\nMEMORY BATTLE", 32));
        left.Controls.Add(Theme.Label("A small deck. A sharp memory.", 16));
        left.Controls.Add(Theme.Label("Original pocket creatures • Classic matching rules", 11, Theme.Muted));
        var art = new Panel() { Width = 380, Height = 220, Margin = new Padding(10, 32, 10, 10) };
        art.Paint += (_, e) => { using var r = new ResourceManager(AppContext.BaseDirectory, _ => { }); for (int i = 0; i < 3; i++) { var img = r.LoadImage($"img/{i}.png"); if (img != null) e.Graphics.DrawImage(img, new Rectangle(i * 120, 30 + (i % 2) * 20, 120, 120)); } };
        left.Controls.Add(art);
        var right = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(10, 32, 0, 0) };
        body.Controls.Add(right);
        right.Controls.Add(Theme.Label("CHOOSE YOUR CHALLENGE", 12, Theme.Accent));
        difficulty.Items.AddRange(Difficulty.All);
        difficulty.SelectedIndex = 0;
        right.Controls.Add(difficulty);
        var resources = new ResourceManager(AppContext.BaseDirectory, Warn);
        audio = new AudioManager(resources);
        resources.Dispose();
        leaderboard = new LeaderboardService(AppContext.BaseDirectory, Warn);
        right.Controls.Add(Theme.Button("Start Game", (_, _) => { Hide(); audio.SetActive(false); using var game = new GameForm((Difficulty)difficulty.SelectedItem!, audio, leaderboard); game.ShowDialog(this); Show(); audio.SetActive(true); }));
        right.Controls.Add(Theme.Button("Leaderboard", (_, _) => { using var f = new LeaderboardForm(leaderboard); f.ShowDialog(this); }));
        right.Controls.Add(Theme.Button("How to Play", (_, _) => { using var f = new HelpForm(false); f.ShowDialog(this); }));
        right.Controls.Add(Theme.Button("About / Team", (_, _) => { using var f = new HelpForm(true); f.ShowDialog(this); }));
        var sound = Theme.Button("Sound: ON", (_, _) => { });
        sound.Click += (_, _) => { audio.Toggle(); sound.Text = $"Sound: {(audio.Enabled ? "ON" : "OFF")}"; };
        right.Controls.Add(sound);
        Activated += (_, _) => sound.Text = $"Sound: {(audio.Enabled ? "ON" : "OFF")}";
        right.Controls.Add(Theme.Button("Exit", (_, _) => Close()));
        FormClosed += (_, _) => audio.Dispose();
    }
    internal static void Warn(string message) => MessageBox.Show(message, "Resource warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}

