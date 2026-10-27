# Kịch bản báo cáo trực tiếp 10 phút

Hạn nộp link: 23h59 ngày 27/10/2026. Báo cáo từ 20h00 thứ Năm 29/10/2026. Nhóm xác nhận đúng khung giờ trong danh sách lớp; không suy ra nhóm từ số thành viên. Mỗi người vào trước ít nhất 10 phút, mở Visual Studio, bật thử mic/camera, thử chia sẻ màn hình và âm thanh. Điền đủ họ tên, MSSV, lớp trong About trước buổi báo cáo. Không cần slide lý thuyết dài.

## Phút 0 đến 1
Thành viên 3 giới thiệu: “Nhóm [TÊN NHÓM] xây dựng Pokémon Memory Battle theo luật Trúc Xanh. Thành viên 1 phụ trách logic và lưu file, thành viên 2 giao diện và báo cáo, thành viên 3 kiểm thử và demo.” Mở How to Play và About, chỉ rõ tên thật/MSSV/lớp sau khi cập nhật.

## Phút 1 đến 4
Chọn Easy 2×2, Start Game rồi Start. Chỉ Score, Moves/Clicks, Time, Pairs. Mở hai thẻ sai và giải thích chờ 750ms mà UI vẫn đáp ứng. Bật/tắt sound, Pause ngay khi hai thẻ đang mở, Resume. Ghép đủ hai cặp. Đọc điểm +100/cặp, phạt −10 sàn 0 và thưởng 5×giây còn lại. Nhập tên thật, Save Result, View Leaderboard và chỉ dòng đã sắp xếp. Nếu thời gian còn, Restart để chứng minh Ready mới; đừng chơi Hard kéo dài.

## Phút 4 đến 7
Mở GameManager.Reset và Select: tạo cặp, Fisher–Yates, kiểm tra InputLocked, so sánh Creature. Mở GameForm.ResetGame để chỉ TableLayoutPanel, `card.Click` và biến index. Chỉ clock.Tick, flip.Tick và TogglePause. Giải thích Timer thay Sleep(2000) để message loop không bị chặn. Chỉ LeaderboardService.Load/Save và MainMenuForm.Warn. Dùng bản biên dịch sao chép riêng, đóng rồi đổi tên img, mở lại để thấy MessageBox và fallback. Khôi phục ngay sau demo. Nếu bỏ bg, giới thiệu SoundPlayer.Load/Play catch. Không thao tác trên nguồn chính.

## Phút 7 đến 10 và câu trả lời ngắn
- **Event Handler là gì?** Hàm xử lý sự kiện như Click hoặc Tick. Khi sự kiện xảy ra, WinForms gọi handler đã đăng ký.
- **System.IO làm gì?** Đọc/ghi file và tạo thư mục. Dự án dùng ReadLines/AppendAllText cho thanhtichs.txt.
- **SoundPlayer hoạt động thế nào?** Nạp WAV, Play cho đúng/sai, PlayLooping cho nền; Stop khi tắt âm thanh và Dispose khi đóng.
- **Vì sao Timer?** Phát Tick theo message loop để cập nhật thời gian và đóng thẻ mà vẫn nhận input/vẽ UI.
- **Vì sao Thread.Sleep gây vấn đề?** Sleep trên UI thread làm ngừng xử lý sự kiện và vẽ; Pause cũng không phản hồi trong lúc đó.
- **Xáo thẻ ra sao?** Fisher–Yates chọn ngẫu nhiên một vị trí trong đoạn chưa chốt và đổi với vị trí cuối đoạn.
- **Tại sao khóa click?** Giữ đúng hai thẻ trong lần so khớp; không cho thẻ thứ ba thay đổi cặp đang chờ.
- **try catch làm gì?** Bắt các lỗi dự kiến tại chỗ nạp tài nguyên/lưu file, thông báo và chuyển phương án phục hồi.
- **Lưu thành tích thế nào?** Kiểm tra tên, serialize Result thành một dòng JSON UTF-8, append qua System.IO rồi reload và sort.
- **Thiếu file xử lý sao?** Ảnh trả null và control vẽ thay; âm thanh bỏ qua; score thiếu được tạo lại; lỗi quyền chuyển LocalAppData.
- **Pause giữ delay thế nào?** Stop Stopwatch giữ elapsed; Resume Start tiếp, không tạo 750ms mới.
- **Restart khi Timer đang chờ?** Dừng Timer trước, reset Stopwatch và state rồi dựng lại grid, tránh event cũ tác động ván mới.

Tất cả thành viên bật mic/camera và sẵn sàng sửa nhỏ trực tiếp. Không tuyên bố đã đọc mã bài mẫu; chỉ đối chiếu những hạn chế giảng viên nêu.
