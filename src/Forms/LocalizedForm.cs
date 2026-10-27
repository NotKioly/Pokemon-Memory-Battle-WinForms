using static PokemonMemoryBattle.LocalizationManager;

namespace PokemonMemoryBattle;

public class LocalizedForm : Form
{
    private readonly List<(Control Control, string Key)> bindings = [];
    private readonly ToolTip tips = new();
    private readonly string titleKey;

    protected LocalizedForm(string titleKey, int width = 1050, int height = 690)
    {
        this.titleKey = titleKey;
        Theme.Form(this, GetString(titleKey), width, height);
        LanguageChanged += OnLanguageChanged;
    }

    protected T BindText<T>(T control, string key) where T : Control
    {
        control.Name = key;
        bindings.Add((control, key));
        control.Text = GetString(key);
        control.AccessibleName = control.Text;
        tips.SetToolTip(control, control.Text);
        return control;
    }

    protected GameButton LocalButton(string key, EventHandler click) => BindText(Theme.Button("", click), key);
    protected Label LocalLabel(string key, int size = 12, Color? color = null) => BindText(Theme.Label("", size, color), key);

    protected void DynamicText(Control control, string text)
    {
        control.Text = text;
        control.AccessibleName = text;
        tips.SetToolTip(control, text);
    }

    public virtual void ApplyLocalization()
    {
        SuspendLayout();
        Text = GetString(titleKey);
        foreach (var (control, key) in bindings)
        {
            if (!control.IsDisposed)
                DynamicText(control, GetString(key));
        }
        ResumeLayout(true);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (IsDisposed)
            return;
        if (IsHandleCreated && InvokeRequired)
            BeginInvoke(ApplyLocalization);
        else
            ApplyLocalization();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LanguageChanged -= OnLanguageChanged;
            tips.Dispose();
        }
        base.Dispose(disposing);
    }
}
