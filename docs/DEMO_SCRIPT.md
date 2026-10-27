# Kịch bản báo cáo trực tiếp 10 phút

Hạn nộp link: 23h59 ngày 27/10/2026. Báo cáo từ 20h00 thứ Năm 29/10/2026. Ba tên/MSSV xác nhận nhóm AltF4, 21:10–21:20; chuẩn bị trước 21:00. Mỗi người vào trước ít nhất 10 phút, mở Visual Studio, bật thử mic/camera, thử chia sẻ màn hình và âm thanh. About đã có đúng Dương Đình Khôi (51.01.104.049), Đào Trọng Quyền (51.01.104.085), Cổ Gia Phúc (51.01.104.126). Chưa có lớp; chỉ bổ sung khi được cung cấp. Không cần slide lý thuyết dài.

## Phút 0 đến 1
Cổ Gia Phúc giới thiệu: “Nhóm AltF4 xây dựng Pokémon Memory Battle theo luật Trúc Xanh. Dương Đình Khôi dự kiến lập trình/Timer/điểm/bắt lỗi/GitHub; Đào Trọng Quyền UI/UX/tài nguyên/test giao diện/hỗ trợ tài liệu; Cổ Gia Phúc thuyết trình/demo/QA/báo cáo cuối.” Mở How to Play và About, chỉ tên/MSSV đã cập nhật và tình trạng lớp chưa được cung cấp.

## Phút 1 đến 4
Chọn Dễ 2×2, Bắt đầu chơi rồi Bắt đầu. Mở Cài đặt, đổi English rồi Tiếng Việt, chỉ điểm/thời gian/thẻ không reset. Chỉ Score, Moves/Clicks, Time, Pairs. Mở hai thẻ sai và giải thích chờ 750ms mà UI vẫn đáp ứng. Bật/tắt sound, Pause ngay khi hai thẻ đang mở, Resume. Ghép đủ hai cặp. Đọc điểm +100/cặp, phạt −10 sàn 0 và thưởng 5×giây còn lại. Nhập tên thật, Save Result, View Leaderboard và chỉ dòng đã sắp xếp. Nếu thời gian còn, Restart để chứng minh Ready mới; đừng chơi Hard kéo dài.

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

Vấn đáp thêm: Đổi tiếng giữ ván thế nào? — LanguageChanged chỉ cập nhật chữ qua ApplyLocalization, không ResetGame. Lưu ở đâu? — settings.json trong LocalAppData; lỗi đọc về vi-VN, lỗi ghi báo và giữ lựa chọn trong phiên. Phân công trên là dự kiến, không xác nhận đóng góp đã hoàn thành.
