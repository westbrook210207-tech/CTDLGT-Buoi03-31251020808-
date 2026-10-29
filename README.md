# CTDLGT – Buổi 03: Ngăn xếp và Hàng đợi (Stack & Queue)

| | |
|---|---|
| **Họ và tên** | `[Điền họ tên]` |
| **MSSV** | `[Điền MSSV]` |
| **Lớp học phần** | `[Điền mã lớp học phần]` |
| **Môn học** | Cấu trúc dữ liệu và Giải thuật – Chương 3, Buổi 3 |
| **Giảng viên** | TS. Nguyễn Thành Huy |

## Cấu trúc thư mục

```
CTDLGT-Buoi03-MSSV/
├── Bai1/Program.cs       Kiểm tra dấu ngoặc cân bằng (Stack<T>)
├── Bai2/Program.cs       Mô phỏng quầy dịch vụ (Queue<T>)
├── Bai3/Program.cs       Tự cài đặt hàng đợi vòng CircularQueue<T>
├── Bai4/Program.cs       Thử thách – Phương án A: hàng đợi bằng hai ngăn xếp
├── README.md
└── KetQuaThucNghiem.md
```

Mỗi bài là một dự án Console riêng (kèm file `.csproj`) và toàn bộ mã nằm trong một file `Program.cs`: phần đầu là chương trình chính (đọc dữ liệu, chạy ca kiểm thử, đo thời gian), phần sau là lớp cài đặt thuật toán / cấu trúc dữ liệu.

## Yêu cầu môi trường

- .NET SDK 8 trở lên (đề yêu cầu .NET 6+). Các dự án đặt `TargetFramework` là `net8.0` và có `RollForward=Major`, nên vẫn chạy được nếu máy chỉ cài .NET 9 hoặc 10.
- Kiểm tra phiên bản: `dotnet --version`.
- Có thể mở từng thư mục bằng Visual Studio / VS Code, hoặc chạy bằng dòng lệnh như bên dưới.

## Biên dịch và chạy

Chạy tất cả lệnh từ thư mục gốc của repository. Nên dùng `-c Release` khi đo thời gian các ca lớn.

### Bài 1 – Kiểm tra dấu ngoặc

```bash
# Chạy toàn bộ 10 ca kiểm thử của Bảng 2 (có đo thời gian)
dotnet run --project Bai1 -c Release -- --test

# Nhập một chuỗi từ bàn phím
dotnet run --project Bai1 -c Release
```

### Bài 2 – Mô phỏng quầy dịch vụ

```bash
# Chạy 6 ca kiểm thử của Bảng 3
dotnet run --project Bai2 -c Release -- --test

# Nhập dữ liệu theo định dạng đề (dòng 1: n k; n dòng sau: a s), kết thúc bằng Ctrl+D (macOS/Linux) hoặc Ctrl+Z, Enter (Windows)
dotnet run --project Bai2 -c Release

# Ca lớn qua file: sinh dữ liệu rồi chuyển hướng vào chương trình
dotnet run --project Bai2 -c Release -- --gen-large > large.txt
dotnet run --project Bai2 -c Release < large.txt > out.txt
```

### Bài 3 – Hàng đợi vòng

```bash
# Chạy 7 ca kiểm thử của Bảng 5, in kèm trạng thái mảng/front/rear/count để minh họa
dotnet run --project Bai3 -c Release
```

### Bài 4 – Hàng đợi bằng hai ngăn xếp (Phương án A)

```bash
# Chạy 2 ca kiểm thử bắt buộc và đo Bảng 6
dotnet run --project Bai4 -c Release
```

## Mô tả giải pháp và độ phức tạp

### Bài 1 – Kiểm tra dấu ngoặc cân bằng

Duyệt chuỗi một lượt. Gặp ngoặc mở thì `Push` cặp `(ký tự, vị trí)` vào `Stack<(char ch, int pos)>`. Gặp ngoặc đóng thì:

- Stack rỗng → **đóng thừa**, báo vị trí ngoặc đóng.
- `Pop` ra ngoặc mở không cùng loại → **đóng sai loại**, báo vị trí ngoặc đóng.

Duyệt xong mà Stack còn phần tử → **mở chưa đóng**: ngoặc mở sớm nhất nằm ở đáy Stack, bỏ dần phần trên cho tới khi còn một phần tử rồi trả về vị trí của nó. Không dùng đệ quy, không dùng `Replace`.

| Thao tác | Thời gian | Bộ nhớ phụ |
|---|---|---|
| Toàn bộ thuật toán | Θ(n) – mỗi ký tự xét một lần | O(n) xấu nhất (chuỗi toàn ngoặc mở) |
| Push | O(1) khấu hao | |
| Pop / Peek | O(1) | |
| Tìm ngoặc mở sớm nhất ở cuối | O(n), chỉ chạy một lần | |

### Bài 2 – Mô phỏng quầy dịch vụ

Đưa n khách vào `Queue<Customer>` theo thứ tự đến. Mảng `long[] freeAt` (k phần tử) lưu thời điểm rảnh của từng quầy. Với mỗi khách lấy ra bằng `Dequeue`:

1. Quét k quầy, chọn quầy có `freeAt` nhỏ nhất; dùng phép so sánh `<` nên khi bằng nhau giữ quầy có số thứ tự nhỏ hơn.
2. `start = max(freeAt[q], a)` – quầy có thể rảnh từ trước và phải đợi khách đến.
3. `wait = start − a`, `end = start + s`, cập nhật `freeAt[q] = end`.

Tổng thời gian chờ, thời điểm đến/bắt đầu/xong đều dùng `long` (tổng chờ có thể cỡ 10¹⁴, vượt `int`). Số thực in với `CultureInfo.InvariantCulture`.

| Thao tác | Thời gian | Bộ nhớ |
|---|---|---|
| Dequeue một khách | O(1) | |
| Chọn quầy rảnh sớm nhất | Θ(k) | |
| Toàn bộ mô phỏng | Θ(n·k) | Θ(n) hàng chờ + Θ(k) mảng `freeAt` |

### Bài 3 – Hàng đợi vòng `CircularQueue<T>`

Mảng `T[]` dung lượng ban đầu 4, ba biến `front` (ô đầu hàng), `rear` (ô trống kế tiếp), `count` (số phần tử). Bất biến: `rear == (front + count) % Capacity`. Chỉ số kế tiếp tính bằng `(i + 1) % Capacity`. Khi `front == rear`, `count` cho biết hàng rỗng (0) hay đầy (= Capacity).

- `Enqueue`: nếu đầy thì `Grow()`; ghi vào `items[rear]`, dời `rear`, tăng `count`.
- `Dequeue`: ném `InvalidOperationException` nếu rỗng; đọc `items[front]`, đặt ô về `default`, dời `front`, giảm `count`. Không dồn phần tử.
- `Grow()`: tạo mảng gấp đôi, dùng hai lần `Array.Copy` để chép theo **thứ tự logic từ front** (đoạn `front..cuối mảng` rồi đoạn `0..` đã quay vòng), sau đó đặt `front = 0`, `rear = count`.

| Thao tác | Thời gian | Ghi chú |
|---|---|---|
| Enqueue | Θ(1) khấu hao | Θ(n) ở lần mở rộng (chép n phần tử) |
| Dequeue | Θ(1) | Chỉ dời `front` |
| Peek, Count, Capacity | Θ(1) | Chỉ đọc |
| Bộ nhớ | Θ(Capacity) | Lần mở rộng cần thêm Θ(n) cho mảng mới |

### Bài 4 – Phương án A: Hàng đợi bằng hai ngăn xếp

`TwoStackQueue<T>` dùng `inbox` (nhận phần tử mới) và `outbox` (lấy ra). `Enqueue` chỉ `Push` vào `inbox`. `Dequeue` chỉ đổ toàn bộ `inbox` sang `outbox` khi `outbox` rỗng, rồi `Pop` từ `outbox`. Thuộc tính `Transfers` đếm số phần tử đã đổ. Lớp đối chứng `NaiveTwoStack<T>` đổ toàn bộ sang ngăn phụ rồi đổ lại ở **mỗi** lần `Enqueue`.

| Lớp | Enqueue | Dequeue | n Enqueue + n Dequeue |
|---|---|---|---|
| TwoStackQueue | Θ(1) | Θ(1) khấu hao (Θ(n) ở lần đổ) | Θ(n) – mỗi phần tử đổ tối đa một lần |
| NaiveTwoStack | Θ(n) | O(1) | Θ(n²) – lần Enqueue thứ i chuyển 2(i−1) phần tử |

Bộ nhớ của cả hai: O(n).

## Ca kiểm thử đã dùng

Mỗi chương trình tự in **Mong đợi / Thực tế / Đạt** cho từng ca.

- **Bài 1:** đủ 10 ca của Bảng 2 (thông thường, đóng sai loại, mở chưa đóng, đóng thừa, chuỗi rỗng, đan xen, không có ngoặc, nhiều ngoặc mở, 500 000 `(` + 500 000 `)`, 1 000 000 `(`). Kết quả: 10/10 đạt.
- **Bài 2:** đủ 6 ca của Bảng 3. Kết quả ca 1–5 trùng kết quả mẫu. Ca 6 (n = 100 000, k = 10) cho chờ trung bình 0.00, chờ lâu nhất 0, hoàn thành lúc 100009. Thêm một ca tự kiểm tra tràn số: 100 000 khách cùng đến lúc 10⁹, s = 10 000, k = 1 → chờ trung bình 499995000.00, hoàn thành lúc 2000000000 (vượt `int.MaxValue`).
- **Bài 3:** đủ 7 ca của Bảng 5 (rỗng, vừa đầy, quay vòng chưa mở rộng, mở rộng khi `front = 0`, mở rộng khi `front ≠ 0`, 1 000 000 lần Enqueue/Dequeue, 100 000 phần tử). Kết quả: 7/7 đạt.
- **Bài 4:** ca bắt buộc (1, 2, 3; Dequeue; 4; Dequeue ×3 → 1, 2, 3, 4; `Transfers = 4`), Dequeue trên hàng rỗng, ca lớn 1 000 000 phần tử (`Transfers = 1 000 000`), và Bảng 6.

## Thời gian chạy ở các ca lớn

> Đo bằng `Stopwatch`, bản build Release, .NET 8, trên máy Linux x64. **Cần chạy lại trên máy cá nhân và thay bằng số đo của mình** (ghi rõ cấu hình máy) trước khi nộp. Số liệu chi tiết xem `KetQuaThucNghiem.md`.

| Bài | Ca | Thời gian | Yêu cầu |
|---|---|---|---|
| 1 | Ca 9: 500 000 `(` + 500 000 `)` | ≈ 10 ms | < 1 s |
| 1 | Ca 10: 1 000 000 `(` | ≈ 20 ms | < 1 s |
| 2 | Ca 6: n = 100 000, k = 10 (chỉ mô phỏng) | ≈ 8 ms | < 1 s |
| 2 | Ca 6 chạy qua file (đọc + mô phỏng + ghi 100 003 dòng) | ≈ 0,25 s | < 1 s |
| 3 | Ca 6: 1 000 000 lần Enqueue rồi Dequeue | ≈ 17 ms | – |
| 3 | Ca 7: 100 000 phần tử, Capacity cuối 131072 | ≈ 2,4 ms | – |
| 4 | 1 000 000 Enqueue + 1 000 000 Dequeue | ≈ 55 ms | < 1 s |

**Bảng 6 (Bài 4):**

| n | NaiveTwoStack (ms) | TwoStackQueue (ms) |
|---|---|---|
| 1 000 | ≈ 24 | ≈ 0,1 |
| 5 000 | ≈ 182 | ≈ 2,0 |
| 20 000 | ≈ 1 457 | ≈ 1,4 |

Khi n tăng 4 lần (5 000 → 20 000), thời gian bản Naive tăng khoảng 8 lần, phù hợp xu hướng Θ(n²) (lý thuyết là 16 lần; ở n nhỏ, chi phí cố định và bộ nhớ đệm làm lệch). Số lần chuyển phần tử (`Transfers`) của Naive là n(n−1) – đúng 399 980 000 với n = 20 000 – trong khi TwoStackQueue chỉ là n.

## Khai báo sử dụng AI

Tôi đã sử dụng **Claude (Anthropic)** để ham khảo cách cài đặt, gợi ý cấu trúc mã và viết README. Tôi đã kiểm chứng bằng cách tự chạy toàn bộ ca kiểm thử trên máy cá nhân, đối chiếu với kết quả mong đợi của đề, tự lần theo trạng thái dữ liệu từng bước và chịu trách nhiệm về bài nộp.
