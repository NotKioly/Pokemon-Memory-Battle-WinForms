using static PokemonMemoryBattle.LocalizationManager;
namespace PokemonMemoryBattle;
public sealed class GameOverForm : LocalizedForm
{
    public bool PlayAgain { get; private set; }
    private readonly GameManager game;
    private readonly Label summary;
    private readonly TextBox playerName = new() { Name = "PlayerNameInput", Width = 350, MaxLength = 24 };
    private readonly GameButton save;
    public GameOverForm(GameManager game, LeaderboardService leaderboard) : base("GameOver", 900, 640)
    {
        this.game = game;
        var body = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(32) };
        Controls.Add(body);
        body.Controls.Add(LocalLabel(game.State == GameState.Won ? "Victory" : "Defeat", 28, Theme.Accent));
        summary = Theme.Label("", 14);
        body.Controls.Add(summary);
        body.Controls.Add(LocalLabel("PlayerName"));
        body.Controls.Add(playerName);
        save = LocalButton("SaveResult", (_, _) => SaveResult(leaderboard));
        body.Controls.Add(save);
        body.Controls.Add(LocalButton("PlayAgain", (_, _) => { PlayAgain = true; Close(); }));
        body.Controls.Add(LocalButton("ViewLeaderboard", (_, _) => { using var f = new LeaderboardForm(leaderboard); f.ShowDialog(this); }));
        body.Controls.Add(LocalButton("Settings", (_, _) => { using var f = new SettingsForm(); f.ShowDialog(this); }));
        body.Controls.Add(LocalButton("Return", (_, _) => Close()));
        ApplyLocalization();
    }
    private void SaveResult(LeaderboardService leaderboard)
    {
        try
        {
            string validated = LeaderboardService.ValidateName(playerName.Text);
            if (leaderboard.Save(new Result(validated, game.Score, game.Moves, game.Difficulty.Seconds - game.Remaining,
                DateTime.Now, game.Difficulty.Name, game.State == GameState.Won)))
            {
                save.Enabled = false;
                playerName.Enabled = false;
                ApplyLocalization();
            }
        }
        catch (ArgumentException)
        {
            MessageBox.Show(this, GetString("NameInvalid"), GetString("PlayerName"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    public override void ApplyLocalization()
    {
        base.ApplyLocalization();
        DynamicText(summary, GetString("FinalStats", game.Score, game.Moves, game.Remaining));
        playerName.AccessibleName = GetString("PlayerName");
        playerName.PlaceholderText = GetString("PlayerNameHint");
        if (!save.Enabled)
            DynamicText(save, GetString("Saved"));
    }
}
