using System.Diagnostics;
using static PokemonMemoryBattle.LocalizationManager;
namespace PokemonMemoryBattle;

public sealed class GameForm : LocalizedForm
{
    public GameManager Game { get; } = new();
    public List<MemoryCardControl> CardControls { get; } = [];
    private readonly Difficulty difficulty;
    private readonly AudioManager audio;
    private readonly LeaderboardService leaderboard;
    private readonly ResourceManager resources;
    private readonly TableLayoutPanel grid = new() { Dock = DockStyle.Fill, Padding = new Padding(20) };
    private readonly Label[] statistics = Enumerable.Range(0, 6).Select(_ => new Label { Dock = DockStyle.Fill, ForeColor = Theme.Ink, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(8), Font = new Font("Segoe UI", 12) }).ToArray();
    private readonly System.Windows.Forms.Timer clock = new() { Interval = 100 };
    private readonly System.Windows.Forms.Timer flip = new() { Interval = 30 };
    private readonly Stopwatch elapsed = new(), mismatch = new();
    private readonly GameButton pause;
    private readonly GameButton sound;
    private long processedSeconds;
    private bool ended;
    private readonly bool testMode;
    public GameForm(Difficulty difficulty, AudioManager audio, LeaderboardService leaderboard, bool testMode = false, Action<string>? warning = null) : base("AppTitle")
    {
        this.difficulty = difficulty;
        this.audio = audio;
        this.leaderboard = leaderboard;
        this.testMode = testMode;
        resources = new ResourceManager(AppContext.BaseDirectory, warning ?? MainMenuForm.Warn);
        resources.CheckDirectories();
        Controls.Add(grid);
        var footer = Theme.Row();
        footer.Dock = DockStyle.Bottom;
        Controls.Add(footer);
        footer.Controls.Add(LocalButton("Start", (_, _) => StartGame()));
        pause = LocalButton("Pause", (_, _) => TogglePause());
        footer.Controls.Add(pause);
        footer.Controls.Add(LocalButton("Restart", (_, _) => { if (ConfirmationForm.Ask(this, "ConfirmRestart")) ResetGame(); }));
        footer.Controls.Add(LocalButton("ReturnMenu", (_, _) => { if (ConfirmationForm.Ask(this, "ConfirmMenu")) Close(); }));
        sound = LocalButton("SoundOn", (_, _) => { audio.Toggle(); RefreshGame(); });
        footer.Controls.Add(sound);
        footer.Controls.Add(LocalButton("Settings", (_, _) => { using var f = new SettingsForm(); f.ShowDialog(this); }));
        var top = new TableLayoutPanel { Dock = DockStyle.Top, Height = 100, ColumnCount = 3, RowCount = 2, Padding = new Padding(12, 4, 12, 4) };
        for (int column = 0; column < 3; column++)
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
        top.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        top.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        for (int i = 0; i < statistics.Length; i++)
            top.Controls.Add(statistics[i], i % 3, i / 3);
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
        Text = GetString("GameTitle", GetString("Difficulty" + difficulty.Name));
        string[] keys = ["HudState", "HudScore", "HudMoves", "HudTime", "HudPairs", "HudDifficulty"];
        string[] values = [GetString("HudState", GetString("State" + Game.State)), GetString("HudScore", Game.Score),
            GetString("HudMoves", Game.Moves, Game.Clicks), GetString("HudTime", Game.Remaining / 60, Game.Remaining % 60),
            GetString("HudPairs", Game.Pairs, Game.Cards.Count / 2), GetString("HudDifficulty", GetString("Difficulty" + difficulty.Name))];
        for (int i = 0; i < statistics.Length; i++)
        {
            statistics[i].Name = keys[i];
            DynamicText(statistics[i], values[i]);
        }
        DynamicText(pause, GetString(Game.State == GameState.Paused ? "Resume" : "Pause"));
        DynamicText(sound, GetString(audio.Enabled ? "SoundOn" : "SoundOff"));
        foreach (var c in CardControls)
        {
            c.Concealed = Game.State == GameState.Paused;
            c.ApplyLocalization();
            c.Invalidate();
        }
    }
    public override void ApplyLocalization()
    {
        base.ApplyLocalization();
        // Text only: never call ResetGame or touch the Stopwatches here.
        RefreshGame();
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
