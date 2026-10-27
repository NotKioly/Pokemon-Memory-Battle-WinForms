# Giải thích mã nguồn để vấn đáp

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
