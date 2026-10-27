# Kiểm toán cuối theo rubric giảng viên

Nguồn: đã đọc lại toàn bộ “Lich bao cao + huong dan.docx”, gồm bảng lịch và rubric. Không có source WIN_T4_TrucXanh để kiểm tra; chỉ so các hạn chế đề mô tả. Danh tính do người dùng cung cấp khớp AltF4. Vai trò cá nhân là dự kiến, chưa có bằng chứng đóng góp riêng.

## Quy ước tính trên 10 điểm
Đề ghi bốn phần **1 + 7 + 1 + 1 = 10**, nhưng mục con phần 2 là **2 + 4 + 1 + 1 = 8**. Không thể khẳng định cách giảng viên sẽ xử lý sai lệch. Để đưa một ước tính /10 có thể kiểm tra, bảng giữ điểm gốc và chỉ **quy đổi mục con phần 2 theo 7/8**. Đây là quy ước ước tính, không phải sửa rubric chính thức. Không cho tối đa với hạng mục thiếu hoặc chưa xác minh. Làm tròn tổng sau cùng, không cộng các giá trị đã làm tròn.

| Tiêu chí | Điểm tối đa | File / phương thức liên quan và tài nguyên | Bằng chứng kiểm thử | Trạng thái | Điểm ước tính |
|---|---|---|---|---|---|
| Giới thiệu, Help/About, họ tên/MSSV/lớp và phân công | 1,0 | HelpForm; LocalizedForm.ApplyLocalization; AboutText/HelpText trong Strings.*.json; TEAM_CONTRIBUTIONS | LANG-12/14, ảnh About hai tiếng; ba tên/ID đúng; lớp chưa cung cấp | NOT VERIFIED — thiếu lớp và live giới thiệu | 0,7000 |
| UI: bố cục, điểm, nhấp, countdown, nút | Gốc 2,0 → 1,75 | Theme; GameForm.RefreshGame/ResetGame; MemoryCardControl.OnPaint; img/*.png; bg/no-image.png | Form thật, đo chữ/nhãn, ảnh VI/EN đã xem; DPI cao chưa thử | NOT VERIFIED toàn bộ độ phân giải/DPI | 1,5750 (gốc 1,8/2) |
| Luồng game, Start/Pause/Restart/End, lưới nhỏ | Gốc 4,0 → 3,5 | GameManager.Reset/Select/Tick; ScoreManager; GameForm.StartGame/TogglePause/OnClockTick/OnFlipTick | 43 kiểm tra gốc, thắng cả ba mô hình, handler Easy, Timer thật, rapid click; LANG-07/08/09 bảo toàn ván | PASS chức năng chạy được | 3,5000 |
| Nhạc nền, đúng/sai, bật/tắt | Gốc 1,0 → 0,875 | AudioManager.Play/Feedback/Toggle/Dispose; bg/background.wav, true.wav, false.wav | Lệnh và toggle thật, WAV hỏng bắt an toàn; LANG-15; chưa nghe qua loa | NOT VERIFIED tiếng nghe được | 0,4375 (gốc 0,5/1) |
| Tên người chơi, System.IO, bảng đã sort | Gốc 1,0 → 0,875 | GameOverForm.SaveResult; LeaderboardService.ValidateName/Save/Read; LeaderboardForm; thanhtichs.txt | Handler Save, reload/tie-break, JSON hỏng, fallback; LANG-10 header/giá trị | PASS | 0,8750 |
| Bẫy lỗi try/catch, MessageBox, giải thích mã | 1,0 | ResourceManager.LoadImage/CheckDirectories; AudioManager; LeaderboardService; MainMenuForm.Warn; CODE_EXPLANATION | Thiếu/hỏng nguồn chạy thật qua callback, LANG-11; MessageBox.Show đã cài; demo modal trực tiếp chưa xác minh | NOT VERIFIED phần trình diễn trực tiếp | 0,7000 |
| Vấn đáp cá nhân, mic/cam, sửa nhỏ trong Visual Studio | 1,0 | DEMO_SCRIPT; TEAM_CONTRIBUTIONS; các handler/IO/SoundPlayer/Timer thật | Đã có Q&A và mã dễ tìm; chưa có buổi vấn đáp hay chứng cứ cá nhân | NOT VERIFIED | 0,0000 |
| **Tổng quy đổi** | **10,0** | **1 + 7 + 1 + 1** | **62 PASS tự động; các phần live chưa tính đủ điểm** | **Ước tính, không phải điểm chính thức** | **7,7875 ≈ 7,79/10** |

## Điều kiện nộp và hạng mục bổ sung không tự gán điểm
| Nghĩa vụ | Bằng chứng / vị trí | Trạng thái |
|---|---|---|
| Solution và .gitignore, toàn bộ source | .sln, src, git tracked tree; build Release | PASS cục bộ |
| Tài nguyên bắt buộc và thành tích mẫu | img; bg/no-image.png; true.wav; false.wav; thanhtichs.txt | PASS |
| Hai ngôn ngữ, mặc định/lưu/phục hồi, giữ ván | LocalizationManager, LocalizedForm, SettingsForm, 2 JSON nhúng; LANG-01–15 | PASS phạm vi log; DPI cao chưa xác minh |
| README Tiếng Việt và danh tính nhất quán | README 17 mục; Help/About; báo cáo; kịch bản; phân công | PASS; lớp chưa cung cấp |
| Báo cáo Markdown và DOCX cập nhật | REPORT.md; BaoCao_GiuaKy_PokemonMemoryBattle.docx | DOCX xuất/kiểm tra cấu trúc; render trang NOT VERIFIED |
| Video 3–5 phút có thành viên nói và thử lỗi | VIDEO_RECORDING.md mới là hướng dẫn, không có MP4 | FAIL |
| Kho public, source/resources mới đã push | DELIVERY.md và kiểm tra remote cuối | NOT VERIFIED cho đến push cuối |
| Nhập link cột Giữa kì trước 23:59 27/10/2026 | Cần nhóm xác nhận riêng; không tự chấm điểm vì chưa có chứng cứ | NOT VERIFIED |
| Có mặt trước giờ ít nhất 10 phút | AltF4 21:10–21:20 ngày 29/10/2026; chuẩn bị trước 21:00 | NOT VERIFIED sự kiện tương lai |

## Các việc còn lại
Bổ sung tên lớp từ nguồn thật; quay/upload video có giọng thành viên; nghe tiếng và kiểm tra MessageBox trên máy trình bày; thử DPI cao; duyệt phân trang Word; xác nhận nộp cột Giữa kì; diễn tập và tham gia vấn đáp. Mã giao diện được hỗ trợ triển khai; nhóm phải hiểu, duyệt và tự giải thích theo yêu cầu không phụ thuộc UI sinh sẵn. Không tuyên bố đáp ứng phần trình diễn bằng chỉ source hoặc test tự động.
