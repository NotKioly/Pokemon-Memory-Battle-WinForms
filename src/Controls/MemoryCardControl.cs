using System.Drawing.Drawing2D;
using static PokemonMemoryBattle.LocalizationManager;
namespace PokemonMemoryBattle;

public sealed class MemoryCardControl : GameButton
{
    public CardModel Card
    {
        get;
    }
    private readonly Image? front, back;
    public bool Concealed
    {
        get; set;
    }
    public MemoryCardControl(CardModel card, Image? frontImage, Image? backImage)
    {
        Card = card;
        front = frontImage;
        back = backImage;
        Dock = DockStyle.Fill;
        Margin = new Padding(7);
        ApplyLocalization();
    }
    public void ApplyLocalization() => AccessibleName = !Concealed && (Card.Revealed || Card.Matched)
        ? GetString("CardAccessible", GetString("Creature" + Card.Creature)) : GetString("CardBack");
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.Round(new Rectangle(2, 2, Width - 5, Height - 5), 12);
        using var fill = new SolidBrush(Card.Matched ? Color.FromArgb(35, 85, 81) : Theme.Panel);
        g.FillPath(fill, path);
        using var pen = new Pen(Card.Matched ? Color.FromArgb(114, 218, 179) : Color.FromArgb(80, 112, 145), 2);
        g.DrawPath(pen, path);
        bool visible = !Concealed && (Card.Revealed || Card.Matched);
        var art = visible ? front : back;
        int side = Math.Max(12, Math.Min(Width - 24, Height - 40));
        var rect = new Rectangle((Width - side) / 2, 8, side, side);
        if (art != null)
        {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(art, rect);
        }
        else
        {
            using var f = new Font("Segoe UI", Math.Max(14, side / 4), FontStyle.Bold);
            TextRenderer.DrawText(g, visible ? "★" : "?", f, rect, Theme.Accent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        var label = new Rectangle(3, Height - 28, Width - 6, 24);
        TextRenderer.DrawText(g, visible ? GetString("Creature" + Card.Creature) : GetString("CardBack"), Font, label, visible ? Theme.Ink : Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        if (Focused)
            ControlPaint.DrawFocusRectangle(g, new Rectangle(6, 6, Width - 12, Height - 12));
    }
}
