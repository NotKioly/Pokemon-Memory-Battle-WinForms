namespace PokemonMemoryBattle;

public sealed class HelpForm : Form
{
    public HelpForm(bool about)
    {
        Theme.Form(this, about ? "About / Team" : "How to Play");
        var content = new TextBox()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Theme.Panel,
            ForeColor = Theme.Ink,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 14),
            Text = about ?
    "POKÉMON MEMORY BATTLE\r\nWindows Programming • 2611COMP101901\r\n\r\nOriginal creature artwork and generated WAV audio. Pokémon is a trademark of its owners; this educational fan title is unaffiliated. No commercial game art or music is included.\r\n\r\nTHÔNG TIN NHÓM\r\n1. [HỌ TÊN 1] • [MSSV 1] • [LỚP 1]\r\nLập trình gameplay, Timer, System.IO, xử lý lỗi.\r\n2. [HỌ TÊN 2] • [MSSV 2] • [LỚP 2]\r\nGiao diện, đồ họa, tài liệu và báo cáo.\r\n3. [HỌ TÊN 3] • [MSSV 3] • [LỚP 3]\r\nQA, thuyết trình, quay video và tài liệu GitHub.\r\n\r\nĐiền thông tin thật trước khi nộp bài." :
    "HOW TO PLAY / HƯỚNG DẪN\r\n\r\nChoose Easy (2×2, 45s), Normal (2×3, 60s) or Hard (4×4, 120s). Press Start to begin.\r\n\r\nSelect two cards with the mouse or Tab then Space. Identical creatures stay visible. A mismatch closes after 750ms; card input is locked during this delay.\r\n\r\nMatch: +100 points. Mismatch: −10, minimum zero. Victory bonus: 5 × remaining seconds. A move is two valid selections; clicks counts valid individual selections.\r\n\r\nPause freezes the countdown and mismatch delay and conceals the cards. Resume continues them. Restart creates a new shuffled deck in Ready state. Return to Menu ends the current round.\r\n\r\nFind all pairs before zero to win. At zero the round ends. Enter a name (1–24 characters), Save Result, then View Leaderboard. Results sort by score, moves and completion seconds. Different difficulties and wins/losses are shown separately in columns.\r\n\r\nSound toggles background and feedback audio together. Missing resources produce a warning and safe fallback. File locations are relative to the executable."
        };
        Controls.Add(content);
        var bottom = Theme.Row();
        bottom.Dock = DockStyle.Bottom;
        bottom.Controls.Add(Theme.Button("Close", (_, _) => Close()));
        Controls.Add(bottom);
    }
}
