using static PokemonMemoryBattle.LocalizationManager;

namespace PokemonMemoryBattle;

public sealed class SettingsForm : LocalizedForm
{
    private readonly ComboBox language = new() { Name = "LanguageSelector", DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
    private bool applying;

    public SettingsForm() : base("Settings", 900, 640)
    {
        var body = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(32), FlowDirection = FlowDirection.TopDown, WrapContents = false };
        Controls.Add(body);
        body.Controls.Add(LocalLabel("Settings", 28, Theme.Accent));
        body.Controls.Add(LocalLabel("Language", 15));
        language.Items.AddRange(["Tiếng Việt", "English"]);
        body.Controls.Add(language);
        foreach (string key in new[] { "SettingsInfo", "SettingsTiming", "SettingsSaved" })
        {
            var label = LocalLabel(key);
            label.AutoSize = false;
            label.Size = new Size(760, 60);
            body.Controls.Add(label);
        }
        var fallback = LocalLabel("SettingsLoadFallback");
        fallback.AutoSize = false;
        fallback.Size = new Size(760, 60);
        fallback.Visible = SettingsLoadFailed;
        body.Controls.Add(fallback);
        body.Controls.Add(LocalButton("Close", (_, _) => Close()));
        language.SelectedIndexChanged += (_, _) =>
        {
            if (applying || language.SelectedIndex < 0)
                return;
            SetLanguage(SupportedLanguages[language.SelectedIndex]);
            if (!SaveLanguagePreference())
                MessageBox.Show(this, GetString("SettingsSaveFailed"), GetString("SettingsWarningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            fallback.Visible = SettingsLoadFailed;
        };
        ApplyLocalization();
    }

    public override void ApplyLocalization()
    {
        base.ApplyLocalization();
        applying = true;
        language.AccessibleName = GetString("Language");
        language.SelectedIndex = Array.IndexOf(SupportedLanguages, CurrentLanguage);
        applying = false;
    }
}
