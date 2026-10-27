namespace PokemonMemoryBattle;
public sealed class HelpForm : LocalizedForm
{
    public HelpForm(bool about) : base(about ? "About" : "Help")
    {
        var content = new TextBox
        {
            Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Theme.Panel, ForeColor = Theme.Ink, BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 13)
        };
        BindText(content, about ? "AboutText" : "HelpText");
        Controls.Add(content);
        var bottom = Theme.Row();
        bottom.Dock = DockStyle.Bottom;
        bottom.Controls.Add(LocalButton("Close", (_, _) => Close()));
        Controls.Add(bottom);
    }
}
