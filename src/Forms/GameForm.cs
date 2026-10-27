using System.Diagnostics;
namespace PokemonMemoryBattle;

public sealed class GameForm : Form
{
    public GameManager Game { get; } = new();
    public List<MemoryCardControl> CardControls { get; } = [];
    private readonly Difficulty difficulty;
    private readonly AudioManager audio;
    private readonly LeaderboardService leaderboard;
    private readonly ResourceManager resources;
    private readonly TableLayoutPanel grid = new() { Dock = DockStyle.Fill, Padding = new Padding(20) };
    private readonly Label status = Theme.Label("", 16);
    private readonly System.Windows.Forms.Timer clock = new() { Interval = 100 };
    private readonly System.Windows.Forms.Timer flip = new() { Interval = 30 };
    private readonly Stopwatch elapsed = new(), mismatch = new();
    private readonly GameButton pause;
    private long processedSeconds;
    private bool ended;
    private readonly bool testMode;
    public GameForm(Difficulty difficulty, AudioManager audio, LeaderboardService leaderboard, bool testMode = false, Action<string>? warning = null)
    {
        this.difficulty = difficulty;
        this.audio = audio;
        this.leaderboard = leaderboard;
        this.testMode = testMode;
        Theme.Form(this, $"Memory Battle • {difficulty.Name}");
        resources = new ResourceManager(AppContext.BaseDirectory, warning ?? MainMenuForm.Warn);
        resources.CheckDirectories();
        Controls.Add(grid);
        var footer = Theme.Row();
        footer.Dock = DockStyle.Bottom;
        Controls.Add(footer);
        footer.Controls.Add(Theme.Button("Start", (_, _) => StartGame()));
        pause = Theme.Button("Pause", (_, _) => TogglePause());
        footer.Controls.Add(pause);
        footer.Controls.Add(Theme.Button("Restart", (_, _) => ResetGame()));
        footer.Controls.Add(Theme.Button("Return to Menu", (_, _) => Close()));
        var sound = Theme.Button($"Sound: {(audio.Enabled ? "ON" : "OFF")}", (_, _) => { });
        sound.Click += (_, _) => { audio.Toggle(); sound.Text = $"Sound: {(audio.Enabled ? "ON" : "OFF")}"; };
        footer.Controls.Add(sound);
        var top = Theme.Row();
        top.Controls.Add(status);
        Controls.Add(top);
        clock.Tick += OnClockTick;
        flip.Tick += OnFlipTick;
        FormClosed += (_, _) => { clock.Dispose(); flip.Dispose(); elapsed.Stop(); mismatch.Stop(); audio.SetActive(false); resources.Dispose(); };
        ResetGame();
    }

    private void OnClockTick(object? sender, EventArgs e)
    {
        if (Game.State != GameState.Playing)
            return;

        // Process every complete active second, including delayed UI ticks.
        while (elapsed.ElapsedMilliseconds / 1000 > processedSeconds && Game.State == GameState.Playing)
        {
            processedSeconds++;
            Game.Tick();
        }
        RefreshGame();
        FinishIfNeeded();
    }

    private void OnFlipTick(object? sender, EventArgs e)
    {
        if (Game.State != GameState.Playing || mismatch.ElapsedMilliseconds < 750)
            return;

        flip.Stop();
        mismatch.Reset();
        Game.ResolveMismatch();
        RefreshGame();
    }
    public void ResetGame()
    {
        clock.Stop();
        flip.Stop();
        elapsed.Reset();
        mismatch.Reset();
        processedSeconds = 0;
        ended = false;
        Game.Reset(difficulty);
        audio.SetActive(false);
        foreach (var card in CardControls)
            card.Dispose();
        CardControls.Clear();
        grid.Controls.Clear();
        grid.RowStyles.Clear();
        grid.ColumnStyles.Clear();
        grid.RowCount = difficulty.Rows;
        grid.ColumnCount = difficulty.Columns;
        for (int r = 0; r < difficulty.Rows; r++)
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / difficulty.Rows));
        for (int c = 0; c < difficulty.Columns; c++)
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / difficulty.Columns));
        var back = resources.LoadImage("bg/no-image.png");
        for (int i = 0; i < Game.Cards.Count; i++)
        {
            int index = i;
            var card = new MemoryCardControl(Game.Cards[i], resources.LoadImage($"img/{Game.Cards[i].Creature}.png"), back);
            card.Click += (_, _) => SelectCard(index);
            CardControls.Add(card);
            grid.Controls.Add(card, i % difficulty.Columns, i / difficulty.Columns);
        }
        RefreshGame();
    }
    public void StartGame()
    {
        if (Game.State != GameState.Ready)
            return;
        Game.Start();
        elapsed.Start();
        clock.Start();
        audio.SetActive(true);
        RefreshGame();
    }
    public void SelectCard(int index)
    {
        var match = Game.Select(index);
        if (match.HasValue)
            audio.Feedback(match.Value);
        if (match == false)
        {
            mismatch.Restart();
            flip.Start();
        }
        RefreshGame();
        FinishIfNeeded();
    }
    public void TogglePause()
    {
        if (Game.State is not (GameState.Playing or GameState.Paused))
            return;
        Game.TogglePause();
        if (Game.State == GameState.Paused)
        {
            elapsed.Stop();
            mismatch.Stop();
            flip.Stop();
            audio.SetActive(false);
        }
        else
        {
            elapsed.Start();
            if (Game.InputLocked)
            {
                mismatch.Start();
                flip.Start();
            }
            audio.SetActive(true);
        }
        RefreshGame();
    }
    private void RefreshGame()
    {
        status.Text = $"{Game.State.ToString().ToUpperInvariant()}    SCORE {Game.Score:0000}    MOVES {Game.Moves} / CLICKS {Game.Clicks}    TIME {Game.Remaining / 60:00}:{Game.Remaining % 60:00}    PAIRS {Game.Pairs}/{Game.Cards.Count / 2}";
        pause.Text = Game.State == GameState.Paused ? "Resume" : "Pause";
        foreach (var c in CardControls)
        {
            c.Concealed = Game.State == GameState.Paused;
            c.Invalidate();
        }
    }
    private void FinishIfNeeded()
    {
        if (ended || Game.State is not (GameState.Won or GameState.Lost))
            return;
        ended = true;
        clock.Stop();
        flip.Stop();
        elapsed.Stop();
        mismatch.Stop();
        audio.SetActive(false);
        if (testMode)
            return;
        using var result = new GameOverForm(Game, leaderboard);
        result.ShowDialog(this);
        if (result.PlayAgain)
            ResetGame();
    }
}
