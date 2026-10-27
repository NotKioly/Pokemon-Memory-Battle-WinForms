namespace PokemonMemoryBattle;

public sealed class GameOverForm : Form
{
    public bool PlayAgain
    {
        get; private set;
    }
    public GameOverForm(GameManager game, LeaderboardService leaderboard)
    {
        Theme.Form(this, "Battle results", 900, 640);
        var body = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(48) };
        Controls.Add(body);
        body.Controls.Add(Theme.Label(game.State == GameState.Won ? "DECK COMPLETE!" : "TIME IS UP", 30, Theme.Accent));
        body.Controls.Add(Theme.Label($"Score {game.Score}   •   Moves {game.Moves}   •   Remaining {game.Remaining}s", 16));
        body.Controls.Add(Theme.Label("Player name (1–24 characters)", 12));
        var name = new TextBox() { Width = 350, MaxLength = 24, AccessibleName = "Player name" };
        body.Controls.Add(name);
        var save = Theme.Button("Save Result", (_, _) => { });
        save.Click += (_, _) => { try { string validated = LeaderboardService.ValidateName(name.Text); if (leaderboard.Save(new Result(validated, game.Score, game.Moves, game.Difficulty.Seconds - game.Remaining, DateTime.Now, game.Difficulty.Name, game.State == GameState.Won))) { save.Enabled = false; name.Enabled = false; save.Text = "Saved ✓"; } } catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Player name", MessageBoxButtons.OK, MessageBoxIcon.Warning); } };
        body.Controls.Add(save);
        body.Controls.Add(Theme.Button("Play Again", (_, _) => { PlayAgain = true; Close(); }));
        body.Controls.Add(Theme.Button("View Leaderboard", (_, _) => { using var f = new LeaderboardForm(leaderboard); f.ShowDialog(this); }));
        body.Controls.Add(Theme.Button("Return", (_, _) => Close()));
    }
}
