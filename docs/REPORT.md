# Báo cáo giữa kỳ Pokémon Memory Battle

Học phần Lập trình trên Windows 2611COMP101901

Nhóm [TÊN NHÓM] gồm ba thành viên

Ngày 27 tháng 10 năm 2026

Báo cáo mô tả phần mềm đã triển khai, mã thực tế và kết quả 43 kiểm tra PASS. Cần bổ sung danh tính thật, video thuyết minh và xác nhận nộp link.


# Chương 1 Giới thiệu đề tài

## Mục tiêu và kết quả
Pokémon Memory Battle là trò chơi ghép cặp thẻ cho học phần Lập trình trên Windows 2611COMP101901. Sản phẩm thể hiện xử lý sự kiện, sinh giao diện động, quản lý trạng thái, Timer, SoundPlayer và lưu thành tích bằng System.IO. Bản Release biên dịch thành công với 0 lỗi, 0 cảnh báo; 43 phép kiểm tra đã chạy trên Windows, gồm form thật và Timer thật.
## Lý do lựa chọn
Luật Trúc Xanh ngắn gọn giúp người xem hiểu ngay trong buổi báo cáo 10 phút. Lưới Easy 2×2 cho phép hoàn thành ván và lưu tên nhanh; Normal 2×3 và Hard 4×4 tăng thử thách mà không cần xây dựng hệ thống chiến đấu phức tạp. Toàn bộ sinh vật và WAV được tạo riêng, không lấy hình hoặc nhạc thương mại của Pokémon.
## Công nghệ và phạm vi
Chương trình dùng C#, WinForms, target net8.0-windows, không dùng UI package ngoài. Môi trường kiểm tra: Windows 10 x64, Visual Studio 2026 Community 18, SDK 10.0.401, Windows Desktop Runtime 10.0.12. RollForward=Major cho phép chạy target .NET 8 trên runtime mới đã cài. Máy khác có thể cài .NET 8 Desktop Runtime hoặc chạy bản publish tự chứa runtime. Visual Studio 2022 từ 17.8 với workload .NET desktop development có thể mở solution.
Phạm vi gồm một người chơi offline, ba độ khó, tính điểm, đếm ngược, pause/resume, restart, âm thanh, nhập tên và bảng xếp hạng. Không có tài khoản mạng, cơ sở dữ liệu, chiến đấu Pokémon hay chống sửa điểm. Ba thành viên cần điền thông tin thật và nhận bàn giao code trước khi trình bày.

# Chương 2 Phân tích yêu cầu

## Chức năng và luật chơi
Menu có chọn độ khó, bắt đầu, leaderboard, hướng dẫn, About, Sound và Exit. Màn chơi hiển thị Score, Moves/Clicks, Time, Pairs và Start/Pause/Resume/Restart/Menu/Sound. Kết thúc ván mở cửa sổ nhập tên, lưu kết quả, chơi lại hoặc xem xếp hạng.
Mỗi sinh vật có hai thẻ; chỉ mở tối đa hai thẻ trong một lần so khớp. Cặp đúng giữ mở, cặp sai đóng sau 750ms. Trong lúc chờ, input thẻ bị khóa; UI vẫn nhận Pause, Restart và đóng cửa sổ. Bỏ click trên thẻ đã mở, đã ghép hoặc ngoài Playing. Hết thời gian chuyển Lost; đủ cặp chuyển Won.
Easy có 4 thẻ/45 giây, Normal 6 thẻ/60 giây, Hard 16 thẻ/120 giây. Đúng +100, sai −10 sàn 0, thắng thưởng 5×giây còn lại. Moves tăng sau hai lựa chọn hợp lệ; Clicks tăng theo từng lựa chọn hợp lệ. CompletionSeconds là thời gian hoạt động, không tính pause.
## Phi chức năng
Giao diện navy, chữ sáng, nhấn vàng, nút tự vẽ có hover/pressed/focus. Lưới phần trăm và Dock thích nghi cửa sổ, kích thước tối thiểu 900×640. Đường dẫn dựa trên AppContext.BaseDirectory và tài nguyên được copy khi build/publish. Không dùng Thread.Sleep trên UI thread. Lỗi tài nguyên và dữ liệu có cảnh báo và phương án phục hồi riêng.
## Đối chiếu hướng dẫn giảng viên
Đã đọc toàn bộ văn bản và bảng trong tài liệu cung cấp. Bài nộp phải có solution, .gitignore chuẩn Visual Studio, img, bg/no-image.png, true.wav, false.wav, thanhtichs.txt mẫu, video 3–5 phút có thành viên nói. Báo cáo tối đa 10 phút phải thể hiện Help/About với họ tên/MSSV/lớp, gameplay, âm thanh, leaderboard, bẫy lỗi và giải thích mã thực tế. REQUIREMENTS.md ánh xạ từng nghĩa vụ tới lớp/hàm và thủ tục xác minh.
Nhãn 7.0 điểm ở phần gameplay không khớp tổng mục con; giữ mọi tiêu chí, không tự sửa thang điểm. Không suy ra tên nhóm/khung giờ từ số ba người. Hạn nộp link là 23h59 ngày 27/10/2026; buổi báo cáo từ 20h00 ngày 29/10/2026; đến sớm ít nhất 10 phút. Thông tin thật, video thuyết minh, nộp cột Giữa kì và vấn đáp chưa được coi là đã hoàn thành.

# Chương 3 Thiết kế hệ thống

## Thư mục và kiến trúc
src chứa Program.cs, Forms, Controls, Models, GameLogic, Services, Utilities. img/bg chứa ảnh thẻ, mặt úp và WAV; thanhtichs.txt ở gốc là dữ liệu mẫu. docs chứa checklist, giải thích code, test, kịch bản và báo cáo; tests chứa lệnh chạy; tools chứa công thức tạo tài nguyên.
GameManager giữ luật độc lập UI. CardModel lưu Creature/Revealed/Matched; Difficulty lưu hàng/cột/giây; ScoreManager tính điểm. GameForm giữ lưới và Timer. MemoryCardControl tự vẽ; Theme/GameButton định nghĩa màu và tương tác. MainMenuForm điều hướng và sở hữu AudioManager; GameOverForm nhập tên; LeaderboardForm dùng DataGridView; HelpForm chứa hướng dẫn/nhóm.
ResourceManager đọc ảnh từ stream và sao chép Bitmap để không khóa file gốc. AudioManager giữ SoundPlayer và Timer khôi phục nền. LeaderboardService xác thực, đọc/ghi và sắp xếp. Result có Name, Score, Moves, CompletionSeconds, Date, Difficulty, Won.
## Luồng xử lý
Ready → Start → Playing. Playing ↔ Paused. Playing → Won khi đủ cặp hoặc Lost khi thời gian bằng 0. Restart dừng Timer và xóa lựa chọn trước khi dựng lưới mới về Ready. FinishIfNeeded dừng clock/flip trước khi mở kết quả. Đóng GameForm giải phóng Timer và ảnh; MainMenuForm giải phóng audio khi thoát ứng dụng.
Select lưu first, rồi second. Nếu sai, second còn tồn tại để InputLocked chặn thẻ thứ ba trong 750ms. Nếu đúng, đánh Matched, cộng điểm, xóa first/second. Sau cặp cuối, chỉ thưởng một lần. Pause dừng Stopwatch để giữ cả phần giây và delay còn lại, đồng thời che mặt thẻ.
## Giao diện thực tế
Menu chia vùng tên game/sinh vật và vùng điều khiển. Board ưu tiên thẻ, HUD trên cùng, nút ở dưới. Viền cặp đúng chuyển xanh. Hỗ trợ chuột hoặc Tab/Space. Ảnh dưới lấy từ GameForm đang chạy qua DrawToBitmap, không phải mockup.
![Gameplay](screenshots/gameplay.png)
Native 1366×768, 1920×1080 và DPI cao cần thử trên máy trình bày; lưới nhỏ nhất đã được kiểm tra trong form 900×640.

# Chương 4 Giải thích mã nguồn


1. **Điểm vào** — `src/Program.cs`, `Main`: `[STAThread]` phù hợp WinForms; khởi tạo cấu hình rồi `Application.Run(new MainMenuForm())`. `--self-test` chỉ chạy bộ kiểm thử tích hợp.
2. **Event Handler** — `GameForm` gắn `card.Click += (_, _) => SelectCard(index)`. Biến `index` cục bộ giữ đúng vị trí từng thẻ. Timer Tick cũng là sự kiện, được xử lý khi message loop rảnh.
3. **Sinh Control** — `GameForm.ResetGame` lặp số thẻ, tạo MemoryCardControl, gắn sự kiện và thêm vào grid. Thẻ cũ được Dispose để giải phóng native handles.
4. **Lưới** — TableLayoutPanel có RowCount/ColumnCount theo Difficulty; RowStyles và ColumnStyles dùng phần trăm; cột `i % Columns`, hàng `i / Columns`. Dock Fill thích nghi kích thước.
5. **Xáo trộn** — `GameManager.Reset`: tạo mỗi ID hai lần. Với i giảm từ n−1 đến 1, chọn j trong [0,i], đổi chỗ. Đây là Fisher–Yates, không sắp xếp theo khóa Random.
6. **So khớp** — `GameManager.Select` mở thẻ đầu; thẻ thứ hai tăng Moves. Nếu cùng Creature thì đánh Matched, cộng điểm; đủ số cặp thì Won và cộng thưởng một lần.
7. **Khóa hai thẻ** — `second != null` tạo InputLocked. Trạng thái, giới hạn index, Revealed và Matched được kiểm tra trước khi thay đổi dữ liệu. Lượt nhấp vô hiệu không tính điểm hay lượt.
8. **Đợi sai không chặn** — `GameForm` có flip Timer tick 30ms; mismatch Stopwatch đạt 750ms thì gọi ResolveMismatch. Không dùng Thread.Sleep. Message loop vẫn vẽ và nhận Pause/Restart.
9. **Đếm ngược** — clock Timer tick 100ms gọi OnClockTick, kiểm tra Stopwatch; mỗi giây hoạt động đầy đủ gọi Game.Tick. processedSeconds đếm số giây đã xử lý; vòng while bù đủ số giây nếu UI tick đến trễ.
10. **Pause Resume** — TogglePause đổi state rồi Stop/Start Stopwatch và mismatch Timer. Không reset đồng hồ khi pause, nên giữ phần giây và phần delay còn lại. Các mặt thẻ được che khi pause.
11. **Tính điểm** — ScoreManager.Match +100, Miss Math.Max(0, score−10), Victory +seconds×5. RefreshGame hiển thị tức thì. Clicks là số lần chọn hợp lệ, Moves là số cặp đã thử.
12. **System.IO** — LeaderboardService dùng File.Exists, ReadLines, AppendAllText, WriteAllText và Directory.CreateDirectory. JSON Lines tránh lỗi phân cách tên có dấu. ValidateName cấm rỗng, control chars và quá 24 ký tự. Read sắp xếp LINQ rồi DataGridView bind danh sách.
13. **SoundPlayer** — AudioManager nạp WAV, Play cho sự kiện, PlayLooping cho nền. Dừng nguồn trước khi phát feedback; Timer 550ms khởi động lại nền nếu vẫn bật và ván đang hoạt động. Toggle dừng ngay và thay đổi Enabled. Dispose dừng Timer và giải phóng player.
14. **try catch** — ResourceManager bắt lỗi IO/quyền/ảnh sai và trả null; thẻ tự vẽ ký hiệu thay thế. AudioManager bắt lỗi nạp và phát; LeaderboardService bỏ dòng hỏng và chuyển LocalAppData khi không ghi được. MainMenuForm.Warn dùng MessageBox.Show. Các catch có thông báo hoặc hành vi phục hồi cụ thể.
15. **Reset và cleanup** — ResetGame dừng cả Timer, reset Stopwatch, state và dữ liệu, tạo lại lưới; FormClosed dispose Timer và ảnh. MainMenuForm sở hữu AudioManager chung và dispose khi đóng ứng dụng; GameForm chỉ tạm dừng âm thanh khi trở về menu.

## Cải tiến so với hạn chế bài mẫu được mô tả trong đề
Thay Sleep(2000) bằng Timer; thêm countdown/pause; nhập tên thật thay Noname; lưu và đọc bảng điểm; xử lý thiếu tài nguyên; tách state khỏi UI. Đây là đối chiếu với mô tả giảng viên, không phải đánh giá mã WIN_T4_TrucXanh chưa được cung cấp.

## Luồng trạng thái
Ready → Start → Playing. Playing ↔ Paused. Playing → Won khi đủ cặp; Playing → Lost khi hết giờ. Restart ở mọi trạng thái tạo Ready mới. Mọi Select ngoài Playing bị bỏ qua. Victory và timeout dừng Timer trước khi mở GameOverForm để hộp thoại không làm chạy ngầm ván chơi.

## Trích đoạn từ source triển khai

### Program.cs
Nguồn src/Program.cs
```csharp
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Contains("--self-test"))
        {
            SelfTests.Run();
            return;
        }
        Application.Run(new MainMenuForm());
    }
}
```

### Sinh thẻ động
Nguồn src/Forms/GameForm.cs
```csharp
        var back = resources.LoadImage("bg/no-image.png");
        for (int i = 0; i < Game.Cards.Count; i++)
        {
            int index = i;
            var card = new MemoryCardControl(Game.Cards[i], resources.LoadImage($"img/{Game.Cards[i].Creature}.png"), back);
            card.Click += (_, _) => SelectCard(index);
            CardControls.Add(card);
            grid.Controls.Add(card, i % difficulty.Columns, i / difficulty.Columns);
        }
```

### Fisher Yates
Nguồn src/GameLogic/GameManager.cs
```csharp
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
```

### Khóa và so khớp
Nguồn src/GameLogic/GameManager.cs
```csharp
    public bool? Select(int index)
    {
        if (State != GameState.Playing || InputLocked || index < 0 || index >= Cards.Count)
            return null;
        var card = Cards[index];
        if (card.Revealed || card.Matched)
            return null;
        card.Revealed = true;
        Clicks++;
        if (first == null)
        {
            first = card;
            return null;
        }
        second = card;
        Moves++;
        if (first.Creature != second.Creature)
        {
            Score = ScoreManager.Miss(Score);
            return false;
        }
        first.Matched = second.Matched = true;
        first = second = null;
        Score = ScoreManager.Match(Score);
        if (Pairs == Cards.Count / 2)
        {
            Score = ScoreManager.Victory(Score, Remaining);
            State = GameState.Won;
        }
        return true;
    }
    public void ResolveMismatch()
```

### Tính điểm
Nguồn src/GameLogic/GameManager.cs
```csharp
public static class ScoreManager
{
    public static int Match(int score) => score + 100;
    public static int Miss(int score) => Math.Max(0, score - 10);
    public static int Victory(int score, int seconds) => score + seconds * 5;
}
```

### Timer
Nguồn src/Forms/GameForm.cs
```csharp
    private void OnClockTick(object? sender, EventArgs e)
    {
        if (Game.State != GameState.Playing)
            return;

        // Process every complete active second, including delayed UI ticks.
        while (elapsed.ElapsedMilliseconds / 1000 > processedSeconds && Game.State == GameState.Playing)
        {
            processedSeconds++;
            Game.Tick();
        }
        RefreshGame();
        FinishIfNeeded();
    }

    private void OnFlipTick(object? sender, EventArgs e)
    {
```

### Pause Resume
Nguồn src/Forms/GameForm.cs
```csharp
    public void TogglePause()
    {
        if (Game.State is not (GameState.Playing or GameState.Paused))
            return;
        Game.TogglePause();
        if (Game.State == GameState.Paused)
        {
            elapsed.Stop();
            mismatch.Stop();
            flip.Stop();
            audio.SetActive(false);
        }
        else
        {
            elapsed.Start();
            if (Game.InputLocked)
            {
                mismatch.Start();
                flip.Start();
            }
            audio.SetActive(true);
        }
        RefreshGame();
    }
    private void RefreshGame()
```

### System IO
Nguồn src/Services/LeaderboardService.cs
```csharp
    public bool Save(Result result)
    {
        try
        {
            ValidateName(result.Name);
            EnsureFile();
            File.AppendAllText(FilePath, JsonSerializer.Serialize(result) + Environment.NewLine);
            return true;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { Fallback(); try { EnsureFile(); File.AppendAllText(FilePath, JsonSerializer.Serialize(result) + Environment.NewLine); return true; } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { warn($"Result could not be saved: {ex.Message}"); return false; } }
    }
```

### SoundPlayer và try catch
Nguồn src/Services/AudioManager.cs
```csharp
    private void Play(string file, bool loop = false)
    {
        if (!players.TryGetValue(file, out var player))
            return;
        try
        {
            if (loop)
                player.PlayLooping();
            else
                player.Play();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.ComponentModel.Win32Exception) { resources.Warn($"Audio playback failed for {file}. {ex.Message}"); players.Remove(file); player.Dispose(); }
    }
    private void Background()
```


# Chương 5 Kiểm thử

## Phương pháp và kết quả
Build Release có 0 lỗi và 0 cảnh báo. Solution mở trong Visual Studio, title/debug target được quan sát qua accessibility. SelfTests chạy STA, mở form thật, dùng PerformClick đi qua handler, bơm message loop để WinForms Timer thực sự Tick. Application.DoEvents chỉ dùng trong harness kiểm thử. TEST_RESULTS.txt ghi 43 PASS của lần chạy cuối; TEST_PLAN.md có từng bước, mong đợi và thực tế.
Các test xác nhận số cặp và thắng/tính thưởng cả ba mức, shuffle bằng seed, sàn điểm 0, click trước Start và click trùng bị bỏ qua, khóa 50 click nhanh, pause giữa mismatch, resume tiếp delay, restart khi đang chờ, timeout bằng Timer thật, lưới Normal/Hard ở 900×640, Help/About/menu, nhập tên và Save qua form, DataGridView nạp dữ liệu.
Test service dùng thư mục tạm, không sửa dữ liệu mẫu. File thiếu được tạo lại; JSON sai, tên null hoặc giá trị lỗi bị bỏ qua. Đường dẫn thanhtichs.txt là một thư mục tạo lỗi truy cập để kiểm tra fallback. Ảnh PNG hỏng, WAV hỏng và nguồn thiếu được nạp thật, callback cảnh báo được ghi nhận và chương trình tiếp tục.
## Một số ca tiêu biểu
| Tình huống | Mong đợi | Thực tế |
|---|---|---|
| Ghép hết thẻ | Won, điểm/bonus đúng | PASS |
| Sai rồi pause hơn một giây | Hai đồng hồ dừng; không đóng thẻ sớm | PASS |
| Restart khi chờ lật | Ready mới không bị event cũ đổi thẻ | PASS |
| Timer hết hạn | Lost ở 0 | PASS |
| Nhập tên và reload | Có tên đúng, xếp hạng đúng | PASS |
| Thiếu/hỏng ảnh/WAV | Cảnh báo callback, không crash | PASS |
| Không ghi được file chính | Chuyển thư mục an toàn | PASS |
| Video có giọng thật | Có bản ghi 3–5 phút | Chưa thực hiện |
## Giới hạn xác minh và bẫy lỗi
Desktop automation đọc được accessibility nhưng chụp cửa sổ timeout và click báo thiếu hình học. Không khẳng định đã demo thủ công bằng chuột, nhìn thấy modal MessageBox hay nghe được tiếng từ loa. Hộp thoại đã được cài bằng MessageBox.Show và callback lỗi đã thực thi. Cần nhóm xác nhận bằng mắt và tai trên máy trình bày. Native display/DPI cao cũng chưa được đổi để thử.
Khi demo, copy output ra thư mục riêng; đóng app, đổi tên img hoặc bg, mở lại và đóng cảnh báo, tiếp tục chơi. Đổi tên thanhtichs.txt rồi mở Leaderboard để thấy tái tạo file. Thêm dòng JSON hỏng để thử bỏ dòng. Khôi phục mọi tài nguyên sau khi ghi. Ảnh và WAV đã nạp được cache nên cần mở lại form/app để buộc đọc nguồn; không gây hỏng cây source chính.

# Chương 6 Kết quả và hướng phát triển

## Kết quả
Phần mềm có menu, board ba mức, state machine, điểm, clicks/moves, countdown, pause, reset, nhập tên, bảng điểm, audio nền/sự kiện và phục hồi lỗi. Tài nguyên ở đúng img/bg/thanhtichs.txt, được đóng gói khi build và publish. Ảnh và WAV gốc có công thức tái tạo; code MIT, tài nguyên CC0. Các tệp tài liệu giúp sinh viên tìm và giải thích code thật.
Kho công khai được tạo tại https://github.com/NotKioly/Pokemon-Memory-Battle-WinForms. Trạng thái push/remote được ghi trong DELIVERY.md khi bàn giao. Nhóm phải điền thông tin thật, quay video có người nói, nghe thử âm thanh, kiểm tra MessageBox và màn hình/DPI, clone/build trên máy trình bày. Đại diện cần nhập URL vào cột Giữa kì và xác nhận riêng.
## Hạn chế
Lưu file không chống sửa điểm hoặc khóa ghi đa tiến trình. Điểm không chuẩn hóa giữa độ khó nên table hiển thị mode và thắng/thua. Lật thẻ đổi mặt ngay và chờ 750ms nếu sai; chưa có animation 3D. Sinh vật gốc thay cho Pokémon chính thức. WinForms chỉ chạy Windows. Bài mẫu WIN_T4_TrucXanh không được cung cấp nên chỉ so với hạn chế giảng viên mô tả.
## Hướng phát triển
Có thể thêm lọc xếp hạng theo độ khó, mở rộng bộ sinh vật, animation flip nhẹ và profile người chơi. Cần hoàn thiện minh chứng nộp bài trước tính năng thêm. Sinh viên phải đọc, thử sửa nhỏ và diễn tập code; phân công đề xuất không phải tuyên bố các cá nhân chưa xác định đã thực hiện công việc.

# Chương 7 Phân công công việc

## Ba thành viên
| Người nhận bàn giao | Thông tin cần điền | Trách nhiệm |
|---|---|---|
| 1 | [HỌ TÊN 1], [MSSV 1], [LỚP 1] | Gameplay, GameManager, điểm, Timer, IO, bắt lỗi |
| 2 | [HỌ TÊN 2], [MSSV 2], [LỚP 2] | UI/UX, tài nguyên, hướng dẫn, tài liệu và báo cáo |
| 3 | [HỌ TÊN 3], [MSSV 3], [LỚP 3] | QA, thuyết trình, video thật, GitHub và nộp link |
## Nhận bàn giao và diễn tập
Thành viên 1 giải thích Select/Reset/Tick, chạy test, thử thay đổi điểm cộng và xác minh file lưu. Thành viên 2 duyệt bố cục/báo cáo, điền thông tin, giải thích TableLayoutPanel và OnPaint. Thành viên 3 chạy bẫy lỗi ở bản sao output, quay video 3–5 phút, xem lại đủ âm thanh và chữ rồi upload. Mỗi người chuẩn bị Event Handler, System.IO, SoundPlayer, Timer, try/catch và chỉnh sửa nhỏ trực tiếp.
Kịch bản 10 phút: 0–1 giới thiệu và About; 1–4 gameplay Easy, âm thanh, pause, lưu điểm; 4–7 code và tài nguyên thiếu; 7–10 vấn đáp. DEMO_SCRIPT.md có câu trả lời ngắn; VIDEO_RECORDING.md có checklist và mốc quay. Không dùng slide lý thuyết dài thay live demo.
## Xác nhận trước khi nộp
Kiểm tra repo public, clone thư mục mới, build/run Easy, lưu tên, mở leaderboard và đổi tên tài nguyên để xác nhận MessageBox. Upload video thật hoặc link Release thật. Điền URL kho vào cột Giữa kì trước 23h59 ngày 27/10/2026 và xác nhận với nhóm. Tất cả mở Visual Studio, thử mic/camera/chia sẻ âm thanh trước khung giờ ít nhất 10 phút.
Nguồn nội bộ: tài liệu hướng dẫn giảng viên được cung cấp; src; REQUIREMENTS.md; CODE_EXPLANATION.md; TEST_PLAN.md; TEST_RESULTS.txt. Trích đoạn Chương 4 lấy trực tiếp từ source, không đặt tên hàm hoặc lớp giả định.