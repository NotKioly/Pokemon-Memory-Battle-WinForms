using System.Diagnostics;
using static PokemonMemoryBattle.LocalizationManager;

namespace PokemonMemoryBattle;

internal static class LocalizationTests
{
    private static void Pump(int milliseconds)
    {
        var time = Stopwatch.StartNew();
        while (time.ElapsedMilliseconds < milliseconds)
            Application.DoEvents();
    }
    private static Control Find(Control root, string key) => root.Controls.Find(key, true).Single();
    private static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (Control nested in Descendants(child))
                yield return nested;
        }
    }
    private static void Capture(Form form, string output, string name)
    {
        using var bitmap = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
        bitmap.Save(Path.Combine(output, name + ".png"));
    }

    public static void Run(string output, string temp, Action<bool, string> check)
    {
        string path = Path.Combine(temp, "language-settings.json");
        var records = new List<string>();
        void Case(bool passed, string id, string description)
        {
            records.Add($"{(passed ? "PASS" : "FAIL")} | {id} | {description}");
            File.WriteAllLines(Path.Combine(output, "localization-results.txt"), records);
            check(passed, id + " " + description);
        }

        Initialize(path);
        using var menu = new MainMenuForm();
        menu.Show();
        Pump(80);
        Case(CurrentLanguage == "vi-VN" && Find(menu, "StartGame").Text == "Bắt đầu chơi", "LANG-01", "First-run actual menu is Vietnamese");
        var viKeys = StringsFor("vi-VN").Keys.Order().ToArray();
        check(viKeys.SequenceEqual(StringsFor("en-US").Keys.Order()), "Localization key parity across embedded JSON resources");

        SetLanguage("en-US");
        Case(Find(menu, "StartGame").Text == "Start Game" && Find(menu, "Settings").Text == "Settings", "LANG-02", "Open menu updates immediately to English");
        SetLanguage("vi-VN");
        Case(Find(menu, "StartGame").Text == "Bắt đầu chơi" && Find(menu, "Settings").Text == "Cài đặt", "LANG-03", "Open menu returns to Vietnamese");

        SetLanguage("en-US");
        bool saved = SaveLanguagePreference();
        string probe = Path.Combine(temp, "probe-language.txt");
        var start = new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "PokemonMemoryBattle.exe")) { UseShellExecute = false, CreateNoWindow = true };
        start.ArgumentList.Add("--language-probe");
        start.ArgumentList.Add(path);
        start.ArgumentList.Add(probe);
        using (var process = Process.Start(start)!)
            Case(saved && process.WaitForExit(10000) && process.ExitCode == 0 && File.ReadAllText(probe) == "en-US", "LANG-04", "Saved preference restored in a fresh process");
        File.Delete(path);
        Initialize(path);
        Case(CurrentLanguage == "vi-VN", "LANG-05", "Missing settings restores Vietnamese");
        bool safe = true;
        foreach (string invalid in new[] { "bad-json", "null", "[]", "{}", "{\"language\":42}", "{\"language\":\"fr-FR\"}" })
        {
            File.WriteAllText(path, invalid);
            SetLanguage("en-US");
            Initialize(path);
            safe &= CurrentLanguage == "vi-VN" && SettingsLoadFailed;
        }
        using (var locked = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            safe &= LoadLanguagePreference() == "vi-VN" && SettingsLoadFailed;
        string blocked = Path.Combine(temp, "blocked-settings-parent");
        File.WriteAllText(blocked, "file instead of directory");
        Initialize(Path.Combine(blocked, "settings.json"));
        safe &= !SaveLanguagePreference();
        Initialize(path);
        Case(safe, "LANG-06", "Invalid JSON/value and inaccessible settings fall back safely; failed save returns false");

        var warnings = new List<string>();
        using var resources = new ResourceManager(AppContext.BaseDirectory, warnings.Add);
        using var audio = new AudioManager(resources);
        var board = new LeaderboardService(Path.Combine(temp, "localized-board"), warnings.Add);
        board.Load();
        board.Save(new Result("Người thử", 200, 2, 10, DateTime.Now, "Easy", true));
        using var game = new GameForm(Difficulty.All[1], audio, board, true, warnings.Add);
        using var ranking = new LeaderboardForm(board);
        using var help = new HelpForm(false);
        using var about = new HelpForm(true);
        using var settings = new SettingsForm();
        using var confirm = new ConfirmationForm("ConfirmRestart");
        var expired = new GameManager();
        expired.Reset(Difficulty.All[0]);
        expired.Start();
        while (expired.State == GameState.Playing)
            expired.Tick();
        var winner = new GameManager();
        winner.Reset(Difficulty.All[0]);
        winner.Start();
        foreach (var group in winner.Cards.Select((card, index) => (card, index)).GroupBy(x => x.card.Creature))
        foreach (var item in group)
            winner.Select(item.index);
        using var end = new GameOverForm(expired, board);
        using var victory = new GameOverForm(winner, board);
        LocalizedForm[] forms = [menu, game, ranking, help, about, settings, confirm, end, victory];
        foreach (var form in forms.Skip(1))
            form.Show();
        game.StartGame();
        var pair = game.Game.Cards.Select((card, index) => (card, index)).GroupBy(x => x.card.Creature).First();
        foreach (var item in pair)
            game.CardControls[item.index].PerformClick();
        var unmatched = game.Game.Cards.Select((card, index) => (card, index)).Where(x => !x.card.Matched).GroupBy(x => x.card.Creature).Select(g => g.First()).ToArray();
        game.CardControls[unmatched[0].index].PerformClick();
        game.CardControls[unmatched[1].index].PerformClick();
        Pump(350);
        int score = game.Game.Score, remaining = game.Game.Remaining, moves = game.Game.Moves;
        var deck = game.Game.Cards.ToArray();
        string flags = string.Join(',', deck.Select(c => $"{c.Revealed}/{c.Matched}"));
        var selected = (ComboBox)Find(settings, "LanguageSelector");
        selected.SelectedIndex = 1; // Executes the actual selector handler and saves preference.
        Case(game.Game.Score == score && game.Game.Moves == moves && score == 90, "LANG-07", "Settings selector preserves live score and moves");
        Case(game.Game.Remaining == remaining && game.Game.State == GameState.Playing, "LANG-08", "Switch preserves active countdown value and state");
        Case(game.Game.Cards.SequenceEqual(deck) && game.Game.InputLocked && flags == string.Join(',', deck.Select(c => $"{c.Revealed}/{c.Matched}")) && game.Game.Difficulty == Difficulty.All[1], "LANG-09", "Same card objects/order/reveal flags, pending lock and difficulty");
        var table = ranking.Controls.OfType<DataGridView>().Single();
        bool headers = table.Columns.Cast<DataGridViewColumn>().All(c => c.HeaderText == GetString("Column" + c.Name));
        selected.SelectedIndex = 0;
        headers &= table.Columns["Name"].HeaderText == "Người chơi" && table.Rows[0].Cells["Difficulty"].FormattedValue!.ToString() == "Dễ";
        selected.SelectedIndex = 1;
        headers &= table.Columns["Name"].HeaderText == "Player" && table.Rows[0].Cells["Won"].FormattedValue!.ToString() == "Won";
        Case(headers, "LANG-10", "All leaderboard headers, difficulty and result values translate in place");

        bool messages = true;
        foreach (string culture in SupportedLanguages)
        {
            SetLanguage(culture);
            var observed = new List<string>();
            using var missing = new ResourceManager(Path.Combine(temp, "missing-" + culture), observed.Add);
            missing.LoadImage("img/missing.png");
            messages &= observed.Single() == GetString("ImageLoadFailed", "img/missing.png");
            try { LeaderboardService.ValidateName(" "); messages = false; }
            catch (ArgumentException ex) { messages &= ex.Message == GetString("NameInvalid"); }
        }
        Case(messages, "LANG-11", "Actual resource errors and validation exceptions use selected language");
        SetLanguage("vi-VN");
        Case(Find(menu, "Settings").Text == "Cài đặt" && Find(about, "AboutText").Text.Contains("Dương Đình Khôi") &&
            Find(about, "AboutText").Text.Contains("Đào Trọng Quyền") && Find(about, "AboutText").Text.Contains("Cổ Gia Phúc") &&
            forms.All(f => f.Font.Name == "Segoe UI"), "LANG-12", "UTF-8 Vietnamese strings/names retained with Segoe UI");

        bool fits = true;
        foreach (string culture in SupportedLanguages)
        {
            SetLanguage(culture);
            foreach (var form in forms)
            {
                form.Size = new Size(900, 640);
                form.PerformLayout();
                foreach (var button in Descendants(form).OfType<GameButton>().Where(b => b is not MemoryCardControl))
                {
                    var textSize = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                    fits &= textSize.Width <= button.Width - 8 && textSize.Height <= button.Height - 4;
                }
                foreach (var label in Descendants(form).OfType<Label>().Where(l => !l.AutoSize && l.Visible))
                {
                    var textSize = TextRenderer.MeasureText(label.Text, label.Font, new Size(label.Width, int.MaxValue), TextFormatFlags.WordBreak);
                    fits &= textSize.Height <= label.Height;
                }
            }
            game.ClientSize = new Size(1050, 690);
            menu.ClientSize = new Size(1050, 690);
            game.PerformLayout();
            menu.PerformLayout();
            Capture(menu, output, "menu-" + culture);
            Capture(game, output, "gameplay-" + culture);
            Capture(settings, output, "settings-" + culture);
            Capture(ranking, output, "leaderboard-" + culture);
            Capture(about, output, "about-" + culture);
            Capture(help, output, "help-" + culture);
            Capture(confirm, output, "confirmation-" + culture);
            Capture(end, output, "result-" + culture);
            Capture(victory, output, "victory-" + culture);
            fits &= game.Width <= 1366 && game.Height <= 768 && game.Width <= 1920 && game.Height <= 1080;
        }
        Case(fits, "LANG-13", "Button text measurements and window bounds fit 900x640/default at 100% DPI; native high-DPI remains manual");
        bool content = true;
        foreach (string culture in SupportedLanguages)
        {
            SetLanguage(culture);
            content &= Find(help, "HelpText").Text == GetString("HelpText") && Find(about, "AboutText").Text == GetString("AboutText");
            content &= ((TextBox)Find(help, "HelpText")).Lines.Length > 10 && ((TextBox)Find(about, "AboutText")).Lines.Length > 10;
            content &= Find(about, "AboutText").Text.Contains("51.01.104.049") && Find(about, "AboutText").Text.Contains("51.01.104.085") && Find(about, "AboutText").Text.Contains("51.01.104.126");
            foreach (var form in forms)
            foreach (var control in Descendants(form).Where(c => StringsFor(culture).ContainsKey(c.Name) && !c.Name.StartsWith("Hud") && c.Name is not ("SoundOn" or "Pause")))
                content &= control.Text == GetString(control.Name);
        }
        Case(content, "LANG-14", "Open Help/About/Team forms and exact three IDs switch together");
        bool soundAndDialogs = true;
        foreach (string culture in SupportedLanguages)
        {
            SetLanguage(culture);
            ((GameButton)Find(game, "SoundOn")).PerformClick();
            soundAndDialogs &= Find(game, "SoundOn").Text == GetString(audio.Enabled ? "SoundOn" : "SoundOff");
            soundAndDialogs &= Find(confirm, "Yes").Text == GetString("Yes") && Find(confirm, "No").Text == GetString("No") &&
                Find(confirm, "ConfirmRestart").Text == GetString("ConfirmRestart");
        }
        Case(soundAndDialogs, "LANG-15", "Actual sound button handlers and confirmation question/buttons translate");
        Pump(850);
        check(!game.Game.InputLocked, "Language switches preserve nonblocking pending mismatch completion");
        int subscribers = SubscriberCount;
        using (var disposable = new SettingsForm())
            check(SubscriberCount == subscribers + 1, "Localization form subscribes once");
        check(SubscriberCount == subscribers, "Disposed form releases localization event subscription");
        foreach (var form in forms)
            form.Close();
        SetLanguage("vi-VN");
    }
}
