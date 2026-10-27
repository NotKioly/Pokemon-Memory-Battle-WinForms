# Kiểm tra xuất báo cáo Word

Đã cập nhật báo cáo DOCX hiện có bằng python-docx, giữ thiết lập trang/style, đủ bảy chương, đúng ba tên/MSSV, phân công dự kiến, mục đa ngôn ngữ và trích đoạn đọc từ source cuối. Đã kiểm tra cấu trúc ZIP/XML, nội dung và ảnh gameplay thật. REPORT.md chứa đầy đủ báo cáo dạng Markdown.

**Phân trang trực quan: NOT VERIFIED.** Đã chạy lại render_docx.py sau cập nhật; thất bại với `FileNotFoundError: LibreOffice soffice.exe was not found on PATH`. Bundle Windows không có LibreOffice và không có Word. Không tuyên bố đã render hoặc duyệt ảnh từng trang. Cần mở DOCX trên máy có Word/LibreOffice để kiểm tra ngắt trang, code và bảng trước khi nộp.
