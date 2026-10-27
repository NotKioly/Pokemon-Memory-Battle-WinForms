using static PokemonMemoryBattle.LocalizationManager;
namespace PokemonMemoryBattle;
public sealed class LeaderboardForm : LocalizedForm
{
    private readonly DataGridView table = new()
    {
        Name = "LeaderboardTable", Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Theme.Navy, BorderStyle = BorderStyle.None, RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, EnableHeadersVisualStyles = false
    };
    public LeaderboardForm(LeaderboardService service) : base("Leaderboard")
    {
        foreach (string field in new[] { "Name", "Score", "Moves", "CompletionSeconds", "Date", "Difficulty", "Won" })
            table.Columns.Add(new DataGridViewTextBoxColumn { Name = field, DataPropertyName = field, SortMode = DataGridViewColumnSortMode.NotSortable });
        table.DefaultCellStyle.BackColor = Theme.Panel;
        table.DefaultCellStyle.ForeColor = Theme.Ink;
        table.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        table.DefaultCellStyle.SelectionBackColor = Color.FromArgb(65, 98, 119);
        table.ColumnHeadersDefaultCellStyle.BackColor = Theme.Panel;
        table.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Accent;
        table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        table.RowTemplate.Height = 34;
        table.CellFormatting += (_, e) =>
        {
            string column = table.Columns[e.ColumnIndex].Name;
            if (column == "Difficulty" && e.Value is string difficulty)
                e.Value = GetString("Difficulty" + difficulty);
            else if (column == "Won" && e.Value is bool won)
                e.Value = GetString(won ? "WonValue" : "LostValue");
            else if (column == "Date" && e.Value is DateTime date)
                e.Value = date.ToString("d", Culture);
            else
                return;
            e.FormattingApplied = true;
        };
        table.DataSource = service.Load();
        Controls.Add(table);
        var top = Theme.Row();
        top.Controls.Add(LocalLabel("Leaderboard", 26, Theme.Accent));
        Controls.Add(top);
        var bottom = Theme.Row();
        bottom.Dock = DockStyle.Bottom;
        bottom.Controls.Add(LocalButton("Close", (_, _) => Close()));
        bottom.Controls.Add(LocalButton("Settings", (_, _) => { using var f = new SettingsForm(); f.ShowDialog(this); }));
        bottom.Controls.Add(LocalLabel("LeaderboardSort", 11));
        Controls.Add(bottom);
        ApplyLocalization();
    }
    public override void ApplyLocalization()
    {
        base.ApplyLocalization();
        foreach (DataGridViewColumn column in table.Columns)
            column.HeaderText = GetString("Column" + column.Name);
        table.AccessibleName = GetString("Leaderboard");
        table.Invalidate();
    }
}
