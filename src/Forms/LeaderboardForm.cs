namespace PokemonMemoryBattle;

public sealed class LeaderboardForm : Form
{
    public LeaderboardForm(LeaderboardService service)
    {
        Theme.Form(this, "Field records • Leaderboard");
        var table = new DataGridView() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Theme.Navy, BorderStyle = BorderStyle.None, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = true, DataSource = service.Load(), EnableHeadersVisualStyles = false };
        table.DefaultCellStyle.BackColor = Theme.Panel;
        table.DefaultCellStyle.ForeColor = Theme.Ink;
        table.DefaultCellStyle.SelectionBackColor = Color.FromArgb(65, 98, 119);
        table.ColumnHeadersDefaultCellStyle.BackColor = Theme.Panel;
        table.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Accent;
        table.RowTemplate.Height = 34;
        table.DataBindingComplete += (_, _) =>
        {
            if (table.Columns.Contains("CompletionSeconds"))
                table.Columns["CompletionSeconds"].HeaderText = "Time (s)";
            if (table.Columns.Contains("Name"))
                table.Columns["Name"].HeaderText = "Player";
            if (table.Columns.Contains("Difficulty"))
                table.Columns["Difficulty"].HeaderText = "Mode";
            if (table.Columns.Contains("Date"))
                table.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";
        };
        Controls.Add(table);
        var top = Theme.Row();
        top.Controls.Add(Theme.Label("FIELD RECORDS", 26, Theme.Accent));
        Controls.Add(top);
        var bottom = Theme.Row();
        bottom.Dock = DockStyle.Bottom;
        bottom.Controls.Add(Theme.Button("Close", (_, _) => Close()));
        bottom.Controls.Add(Theme.Label("Score ↓  •  Moves ↑  •  CompletionSeconds ↑  •  Date ↑", 11));
        Controls.Add(bottom);
    }
}
