# Kế hoạch và kết quả kiểm thử

Môi trường Windows 10 x64, .NET SDK 10.0.401, target net8.0-windows. Release build 0 lỗi/0 cảnh báo. Có **62 PASS**: 43 kiểm tra hồi quy gốc, 15 ca LANG và 4 kiểm tra bổ sung (đồng nhất khóa, delay đang chờ, đăng ký và gỡ sự kiện). Log nguyên bản trong TEST_RESULTS.txt và LOCALIZATION_RESULTS.txt.

Chạy `powershell -ExecutionPolicy Bypass -File tests/Run-Tests.ps1`. SelfTests/LocalizationTests mở form STA thật, gọi PerformClick và bơm message loop để Timer Tick thực sự chạy. Application.DoEvents chỉ nằm trong harness. Điểm và settings tạm không sửa dữ liệu/cài đặt cá nhân. LANG-04 khởi chạy tiến trình executable mới, không chỉ gán lại biến trong cùng tiến trình.

## Hồi quy gameplay và dịch vụ
| Nhóm | Bước | Mong đợi và kết quả thực tế |
|---|---|---|
| Bộ thẻ | Reset ba mức; đếm cặp; so seed | Đủ 4/6/16 thẻ, hai mỗi loài, shuffle khác: PASS |
| Nhập thẻ | Click Ready, lặp thẻ, 50 click nhanh khi sai | Click vô hiệu bỏ qua, khóa hai thẻ: PASS |
| Điểm/thắng | Ghép mọi cặp ba mức | +100/cặp và bonus đúng, khóa terminal, sàn 0: PASS |
| Timer | Pause khi đang mismatch, Resume, Restart trước tick, timeout thật | Giữ delay/thời gian, reset sạch, Lost ở 0: PASS |
| Form | Menu, Help, About, grid, GameOver tên và Save, DataGridView | Mở form và lưu/hiển thị được: PASS |
| File | Thiếu/hỏng ảnh, WAV, JSON, tên null; target file là directory | Callback cảnh báo, fallback, skip dòng, chuyển vị trí ghi: PASS |
| Âm thanh | Toggle, Feedback, đóng/dispose | Lệnh không ném lỗi: PASS; tiếng qua loa NOT VERIFIED |

## Kiểm thử đa ngôn ngữ
| Mã | Bước thực hiện | Kết quả mong đợi | Thực tế |
|---|---|---|---|
| LANG-01 | Khởi tạo tệp cấu hình chưa tồn tại rồi mở MainMenuForm | Mặc định vi-VN; nút Bắt đầu chơi | PASS |
| LANG-02 | SetLanguage en-US khi menu đang mở | Menu và Cài đặt đổi ngay sang English | PASS |
| LANG-03 | Đổi lại vi-VN | Menu trở về Tiếng Việt | PASS |
| LANG-04 | Lưu en-US; mở executable mới bằng --language-probe | Tiến trình mới đọc en-US từ tệp | PASS |
| LANG-05 | Xóa tệp tạm rồi Initialize | Dùng Tiếng Việt | PASS |
| LANG-06 | Thử JSON hỏng/null/mảng/sai kiểu/fr-FR, khóa tệp đọc, chặn thư mục ghi | Đọc lỗi về vi-VN; ghi lỗi trả false, không crash | PASS |
| LANG-07 | Ghép cặp rồi sai để có 90 điểm; đổi bằng ComboBox trong SettingsForm | Điểm/lượt giữ nguyên | PASS |
| LANG-08 | Đổi khi Playing | Remaining và state giữ nguyên tại thời điểm đổi; không reset Stopwatch | PASS |
| LANG-09 | Chụp mảng CardModel, flags và lock trước/sau đổi | Cùng object/order/mặt mở/Matched/lock/độ khó | PASS |
| LANG-10 | Mở bảng; đổi hai chiều | Header, độ khó, kết quả, ngày theo ngôn ngữ | PASS |
| LANG-11 | Gây lỗi ảnh và tên trống trong mỗi ngôn ngữ | Callback/exception dùng chuỗi đúng ngôn ngữ | PASS |
| LANG-12 | Kiểm tra chuỗi có dấu và tên, render ảnh | Giữ đúng Unicode với Segoe UI; đã xem ảnh | PASS |
| LANG-13 | Đo chữ nút/nhãn ở form 900×640; render mặc định 1050×690 | Không cắt chữ ở 100% DPI; cửa sổ nằm trong 1366×768/1920×1080 | PASS phạm vi 100%; NOT VERIFIED màn hình vật lý/DPI cao |
| LANG-14 | Mở Help/About/Team và các form khác; đổi hai chiều | Đúng tài nguyên mọi label tĩnh, đủ ba ID, xuống dòng thật | PASS |
| LANG-15 | PerformClick nút âm thanh, đổi ngôn ngữ hộp xác nhận | Trạng thái âm thanh và câu hỏi/nút được dịch | PASS |

## Bằng chứng và giới hạn
Ảnh DrawToBitmap thật của menu, board, settings, leaderboard, About, Help, kết quả và xác nhận trong cả hai ngôn ngữ được lưu ở screenshots. Đã xem ảnh để kiểm tra dấu và layout; phát hiện/sửa LF thành CRLF trong nội dung TextBox, đồng thời thêm test số dòng. Có kiểm tra mọi chuỗi tĩnh bind ở các form đang mở và key parity của 94 khóa mỗi tài nguyên.

LANG-13 không chứng minh DPI 150/200% hoặc đã đổi màn hình vật lý; cần thử trên máy thuyết trình. Các cảnh báo tài nguyên đã chạy qua callback, chưa có xác minh bằng mắt hộp MessageBox của lần demo thủ công. Chưa có video hoặc giọng thuyết minh. Bản baseline từng mở trong Visual Studio 2026; bản cập nhật đã build bằng CLI, chưa thao tác GUI Visual Studio lại.

## Bẫy lỗi thủ công có thể lặp lại
1. Build và sao chép toàn bộ output sang thư mục demo riêng. Không đổi tên source chính.
2. Đóng bản sao, đổi img thành img.saved, mở lại Bắt đầu chơi. Đóng cảnh báo và thử chơi bằng hình thay thế.
3. Khôi phục img, đổi bg; mở lại, đóng cảnh báo, xác nhận chơi không tiếng vẫn được.
4. Khôi phục bg, đổi thanhtichs.txt, mở Bảng thành tích: cảnh báo và tạo rỗng. Lưu tên rồi tải lại.
5. Chuyển English và lặp lỗi để kiểm tra nội dung cảnh báo English. Thêm dòng JSON hỏng để kiểm tra bỏ dòng.
6. Ảnh/WAV đã cache không tự mất khi rename; mở lại form/app để buộc nạp nguồn. Khôi phục tài nguyên trước khi quay/nộp.
