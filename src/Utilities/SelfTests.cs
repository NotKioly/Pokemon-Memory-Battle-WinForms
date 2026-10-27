using System.Diagnostics;
using System.Text.Json;
namespace PokemonMemoryBattle;

internal static class SelfTests
{
    private static readonly List<string> log = [];
    private static void Check(bool condition, string name)
    {
        log.Add($"{(condition ? "PASS" : "FAIL")} | {name}");
        if (!condition)
            throw new Exception(name);
    }
    private static void Pump(int milliseconds)
    {
        var timer = Stopwatch.StartNew();
        while (timer.ElapsedMilliseconds < milliseconds)
        {
            Application.DoEvents();
        }
    }
    public static void Run()
    {
        string output = Path.Combine(Environment.CurrentDirectory, "artifacts");
        Directory.CreateDirectory(output);
        string temp = Path.Combine(Path.GetTempPath(), "MemoryBattleTests-" + Guid.NewGuid());
        Directory.CreateDirectory(temp);
        try
        {
            foreach (var d in Difficulty.All)
            {
                var g = new GameManager();
                g.Reset(d, new Random(42));
                Check(g.Cards.Count == d.Rows * d.Columns && g.Cards.GroupBy(c => c.Creature).All(x => x.Count() == 2), $"{d.Name} paired deck");
                g.Start();
                foreach (var group in g.Cards.Select((c, i) => (c, i)).GroupBy(x => x.c.Creature))
                {
                    foreach (var x in group)
                        g.Select(x.i);
                }
                Check(g.State == GameState.Won && g.Score == g.Cards.Count / 2 * 100 + d.Seconds * 5, $"{d.Name} victory and time bonus");
                int clicks = g.Clicks;
                g.Select(0);
                Check(g.Clicks == clicks, $"{d.Name} end-state input rejected");
            }
            var game = new GameManager();
            game.Reset(Difficulty.All[2], new Random(2));
            string order = string.Join(',', game.Cards.Select(c => c.Creature));
            game.Reset(Difficulty.All[2], new Random(3));
            Check(order != string.Join(',', game.Cards.Select(c => c.Creature)), "Fisher�Yates changes deck order");
            game.Reset(Difficulty.All[0], new Random(1));
            game.Select(0);
            Check(game.Clicks == 0, "Ready rejects input");
            game.Start();
            int other = game.Cards.FindIndex(c => c.Creature != game.Cards[0].Creature);
            game.Select(0);
            game.Select(0);
            Check(game.Clicks == 1, "Repeated revealed-card click rejected");
            game.Select(other);
            for (int i = 0; i < 50; i++)
                game.Select(i % 4);
            Check(game.Clicks == 2 && game.Moves == 1 && game.Score == 0 && game.InputLocked, "Rapid clicks locked and nonnegative penalty");
            game.TogglePause();
            int time = game.Remaining;
            game.Tick();
            game.ResolveMismatch();
            Check(game.Remaining == time && game.InputLocked, "Paused model freezes countdown and mismatch");
            game.TogglePause();
            game.ResolveMismatch();
            Check(!game.InputLocked && game.Cards.All(c => !c.Revealed), "Mismatch resolves after resume");
            while (game.State == GameState.Playing)
                game.Tick();
            Check(game.State == GameState.Lost && game.Remaining == 0, "Timeout ends game");
            game.Reset(Difficulty.All[0]);
            Check(game.State == GameState.Ready && !game.InputLocked && game.Score == 0, "Reset clears terminal state");
            List<string> warnings = [];
            var resources = new ResourceManager(temp, warnings.Add);
            resources.CheckDirectories();
            Check(resources.LoadImage("img/0.png") == null && resources.LoadImage("bg/no-image.png") == null && warnings.Count >= 4, "Missing folders and images trigger warning callbacks with fallback");
            Directory.CreateDirectory(Path.Combine(temp, "img"));
            File.WriteAllText(Path.Combine(temp, "img", "bad.png"), "invalid");
            Check(resources.LoadImage("img/bad.png") == null, "Malformed image handled");
            using (var silent = new AudioManager(resources))
            {
                silent.Feedback(true);
                silent.Toggle();
                silent.Feedback(false);
            }
            Check(warnings.Any(x => x.Contains("true.wav")) && warnings.Any(x => x.Contains("false.wav")), "Missing audio safe and warning callbacks invoked");
            Directory.CreateDirectory(Path.Combine(temp, "bg"));
            File.WriteAllText(Path.Combine(temp, "bg", "true.wav"), "not-wave");
            using (var invalid = new AudioManager(resources))
            {
                invalid.Feedback(true);
            }
            Check(warnings.Any(x => x.Contains("playback failed") || x.Contains("Cannot play bg/true.wav")), "Invalid WAV safely skipped");
            resources.Dispose();
            var board = new LeaderboardService(temp, warnings.Add);
            Check(board.Load().Count == 0 && File.Exists(board.FilePath), "Missing leaderboard recreated");
            var one = new Result("Tester", 250, 4, 12, DateTime.Now, "Easy", true);
            Check(board.Save(one), "System.IO save");
            board.Save(one with
            {
                Name = "Better",
                Score = 300
            });
            board.Save(one with
            {
                Name = "Tie",
                Score = 300,
                Moves = 2
            });
            File.AppendAllText(board.FilePath, "bad-json\n{\"Score\":-1}\n{\"Name\":null,\"Score\":1,\"Moves\":1,\"CompletionSeconds\":1,\"Date\":\"2026-10-27T00:00:00\",\"Difficulty\":\"Easy\"}\n");
            var rows = board.Load();
            Check(rows.Count == 3 && rows[0].Name == "Tie" && warnings.Any(x => x.Contains("invalid leaderboard")), "Reload sorts ties and skips corrupt rows");
            bool rejected = false;
            try
            {
                LeaderboardService.ValidateName("  ");
            }
            catch (ArgumentException) { rejected = true; }
            Check(rejected, "Blank player name rejected");
            string blocked = Path.Combine(temp, "blocked");
            Directory.CreateDirectory(blocked);
            Directory.CreateDirectory(Path.Combine(blocked, "thanhtichs.txt"));
            var fallback = new LeaderboardService(blocked, warnings.Add, Path.Combine(temp, "fallback"));
            fallback.Load();
            Check(fallback.Save(one) && fallback.FilePath.Contains("fallback"), "Access failure uses writable fallback");
            using var realResources = new ResourceManager(AppContext.BaseDirectory, warnings.Add);
            using var audio = new AudioManager(realResources);
            audio.Toggle();
            Check(!audio.Enabled, "Sound toggle OFF");
            audio.Toggle();
            Check(audio.Enabled, "Sound toggle ON");
            using (var form = new GameForm(Difficulty.All[0], audio, board, true, warnings.Add))
            {
                form.Show();
                Pump(100);
                Check(form.Visible && form.CardControls.Count == 4, "Real GameForm opens and generates controls");
                form.StartGame();
                form.CardControls[0].PerformClick();
                int wrong = form.Game.Cards.FindIndex(c => c.Creature != form.Game.Cards[0].Creature);
                form.CardControls[wrong].PerformClick();
                form.TogglePause();
                int remaining = form.Game.Remaining;
                Pump(1100);
                Check(form.Game.Remaining == remaining && form.Game.InputLocked, "Real timers freeze on pause during mismatch");
                form.TogglePause();
                Pump(850);
                Check(!form.Game.InputLocked, "Real WinForms mismatch timer resolves after resume");
                Pump(400);
                Check(form.Game.Remaining < remaining, "Real countdown timer ticks");
                form.CardControls[0].PerformClick();
                form.CardControls[wrong].PerformClick();
                form.ResetGame();
                Pump(850);
                Check(form.Game.State == GameState.Ready && form.Game.Cards.All(c => !c.Revealed), "Restart cancels pending timer event");
                form.StartGame();
                foreach (var group in form.Game.Cards.Select((c, i) => (c, i)).GroupBy(x => x.c.Creature))
                foreach (var x in group)
                    form.CardControls[x.i].PerformClick();
                Check(form.Game.State == GameState.Won, "Real card click handlers win");
                form.ResetGame();
                form.StartGame();
                form.Size = new Size(1040, 680);
                form.CardControls[0].PerformClick();
                Pump(100);
                using var bitmap = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(Path.Combine(output, "gameplay.png"));
                form.Close();
            }
            foreach (var d in Difficulty.All.Skip(1))
            {
                using var f = new GameForm(d, audio, board, true, warnings.Add);
                f.Size = new Size(900, 640);
                f.Show();
                f.StartGame();
                Pump(80);
                Check(f.CardControls.All(c => c.Width > 40 && c.Height > 40), $"{d.Name} grid fits at minimum form size");
                f.Close();
            }
            using (var timeout = new GameForm(new Difficulty("Easy", 2, 2, 1), audio, board, true, warnings.Add))
            {
                timeout.Show();
                timeout.StartGame();
                Pump(1200);
                Check(timeout.Game.State == GameState.Lost, "Real countdown reaches loss");
                timeout.Close();
            }
            using (var help = new HelpForm(false))
            {
                help.Show();
                Pump(50);
                Check(help.Visible, "Help opens");
                help.Close();
            }
            using (var about = new HelpForm(true))
            {
                about.Show();
                Pump(50);
                Check(about.Visible, "About opens");
                about.Close();
            }
            using (var ranking = new LeaderboardForm(board))
            {
                ranking.Show();
                Pump(50);
                Check(ranking.Controls.OfType<DataGridView>().Single().Rows.Count == 3, "Leaderboard DataGridView displays saved results");
                ranking.Close();
            }
            using (var end = new GameOverForm(game, board))
            {
                end.Show();
                var body = end.Controls.OfType<FlowLayoutPanel>().Single();
                body.Controls.OfType<TextBox>().Single().Text = "UI Tester";
                body.Controls.OfType<GameButton>().Single(b => b.Text == "Save Result").PerformClick();
                Check(board.Load().Any(r => r.Name == "UI Tester"), "GameOver name entry and Save Result handler");
                end.Close();
            }
            using (var menu = new MainMenuForm())
            {
                menu.Show();
                Pump(100);
                using var bmp = new Bitmap(menu.Width, menu.Height);
                menu.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
                bmp.Save(Path.Combine(output, "menu.png"));
                Check(menu.Visible, "Main menu launches");
                menu.Close();
            }
            Check(warnings.Count(x => x.Contains("img/")) > 0, "Missing-resource warnings observed");
        }
        catch (Exception ex) { log.Add("FAIL | " + ex); Environment.ExitCode = 1; }
        finally { File.WriteAllLines(Path.Combine(output, "test-results.txt"), log); try { Directory.Delete(temp, true); } catch (IOException) { } }
    }
}

