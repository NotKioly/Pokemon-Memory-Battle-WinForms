using static PokemonMemoryBattle.LocalizationManager;

namespace PokemonMemoryBattle;

public sealed class ConfirmationForm : LocalizedForm
{
    public ConfirmationForm(string messageKey) : base("ConfirmTitle", 900, 640)
    {
        var body = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(40), FlowDirection = FlowDirection.TopDown, WrapContents = false };
        Controls.Add(body);
        var question = LocalLabel(messageKey, 18);
        question.AutoSize = false;
        question.Size = new Size(760, 140);
        body.Controls.Add(question);
        var yes = LocalButton("Yes", (_, _) => { DialogResult = DialogResult.Yes; Close(); });
        var no = LocalButton("No", (_, _) => { DialogResult = DialogResult.No; Close(); });
        body.Controls.Add(yes);
        body.Controls.Add(no);
        AcceptButton = yes;
        CancelButton = no;
    }

    public static bool Ask(IWin32Window owner, string messageKey)
    {
        using var dialog = new ConfirmationForm(messageKey);
        return dialog.ShowDialog(owner) == DialogResult.Yes;
    }
}
