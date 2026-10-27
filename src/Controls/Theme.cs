using System.Drawing.Drawing2D;
namespace PokemonMemoryBattle;

public static class Theme
{
    public static readonly Color Navy = Color.FromArgb(17, 29, 48), Panel = Color.FromArgb(29, 47, 70), Accent = Color.FromArgb(250, 204, 90), Ink = Color.FromArgb(230, 239, 247), Muted = Color.FromArgb(158, 181, 203);
    public static void Form(Form f, string title, int width = 1050, int height = 690)
    {
        f.Text = title;
        f.ClientSize = new Size(width, height);
        f.MinimumSize = new Size(900, 640);
        f.StartPosition = FormStartPosition.CenterScreen;
        f.BackColor = Navy;
        f.ForeColor = Ink;
        f.Font = new Font("Segoe UI", 11);
        f.AutoScaleMode = AutoScaleMode.Dpi;
    }
    public static Label Label(string text, int size = 12, Color? color = null) => new() { Text = text, AutoSize = true, Font = new Font("Segoe UI", size, size >= 22 ? FontStyle.Bold : FontStyle.Regular), ForeColor = color ?? Ink, Margin = new Padding(10) };
    public static GameButton Button(string text, EventHandler click)
    {
        var b = new GameButton() { Text = text };
        b.Click += click;
        return b;
    }
    public static FlowLayoutPanel Row() => new() { AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(12), WrapContents = true };
    public static GraphicsPath Round(Rectangle bounds, int radius = 16)
    {
        var p = new GraphicsPath();
        int d = radius * 2;
        p.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        p.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        p.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        p.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}
public class GameButton : Button
{
    private bool hover, pressed;
    public GameButton()
    {
        Size = new Size(150, 44);
        Margin = new Padding(6);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        UseVisualStyleBackColor = false;
    }
    protected override void OnMouseEnter(EventArgs e)
    {
        hover = true;
        Invalidate();
        base.OnMouseEnter(e);
    }
    protected override void OnMouseLeave(EventArgs e)
    {
        hover = pressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        pressed = true;
        Invalidate();
        base.OnMouseDown(e);
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        pressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.Round(new Rectangle(1, 1, Width - 3, Height - 3), 10);
        using var brush = new SolidBrush(pressed ? Color.FromArgb(56, 77, 95) : hover ? Color.FromArgb(57, 83, 108) : Theme.Panel);
        e.Graphics.FillPath(brush, path);
        using var pen = new Pen(Focused ? Theme.Accent : Color.FromArgb(80, 111, 139), Focused ? 2 : 1);
        e.Graphics.DrawPath(pen, path);
        TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, Enabled ? Theme.Ink : Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
