using System.Diagnostics;

int passed = 0;
const int Total = 7;

Header(1, "Hang rong", "Tao hang moi; Dequeue; Peek",
       "Count 0, Capacity 4; hai lenh sau cung nem InvalidOperationException");
{
    var q = new CircularQueue<string>();
    Console.WriteLine($"  Trang thai: {q.DumpState()}");
    bool dequeueThrows = Throws(() => q.Dequeue(), "Dequeue");
    bool peekThrows = Throws(() => q.Peek(), "Peek");
    Result(q.Count == 0 && q.Capacity == 4 && dequeueThrows && peekThrows,
           $"Count {q.Count}, Capacity {q.Capacity}");
}

Header(2, "Vua day", "Enqueue A, B, C, D; Peek", "Count 4, Capacity 4; Peek cho A");
{
    var q = new CircularQueue<string>();
    foreach (var x in new[] { "A", "B", "C", "D" }) q.Enqueue(x);
    Console.WriteLine($"  Trang thai: {q.DumpState()}");
    string peek = q.Peek();
    Result(q.Count == 4 && q.Capacity == 4 && peek == "A",
           $"Count {q.Count}, Capacity {q.Capacity}; Peek cho {peek}");
}

Header(3, "Quay vong chi so, chua mo rong",
       "Enqueue A, B, C, D; Dequeue 2 lan; Enqueue E, F; Dequeue 4 lan",
       "Lay ra: A, B roi C, D, E, F; Capacity van 4");
{
    var q = new CircularQueue<string>();
    foreach (var x in new[] { "A", "B", "C", "D" }) q.Enqueue(x);
    var first = DequeueMany(q, 2);
    Console.WriteLine($"  Sau 2 Dequeue: {q.DumpState()}");
    foreach (var x in new[] { "E", "F" }) q.Enqueue(x);
    Console.WriteLine($"  Sau Enqueue E, F: {q.DumpState()}");
    var second = DequeueMany(q, 4);
    Result(string.Join(",", first) == "A,B" && string.Join(",", second) == "C,D,E,F" && q.Capacity == 4,
           $"Lay ra: {string.Join(", ", first)} roi {string.Join(", ", second)}; Capacity {q.Capacity}");
}

Header(4, "Mo rong khi front = 0", "Enqueue 1, 2, 3, 4, 5; Dequeue 5 lan", "Capacity 8; lay ra 1, 2, 3, 4, 5");
{
    var q = new CircularQueue<int>();
    for (int i = 1; i <= 5; i++) q.Enqueue(i);
    Console.WriteLine($"  Sau 5 Enqueue: {q.DumpState()}");
    int cap = q.Capacity;
    var outList = DequeueMany(q, 5);
    Result(cap == 8 && string.Join(",", outList) == "1,2,3,4,5",
           $"Capacity {cap}; lay ra {string.Join(", ", outList)}");
}

Header(5, "Mo rong khi front khac 0",
       "Enqueue A, B, C, D; Dequeue 2 lan; Enqueue E, F, G; Dequeue 5 lan",
       "Capacity 8; lay ra C, D, E, F, G");
{
    var q = new CircularQueue<string>();
    foreach (var x in new[] { "A", "B", "C", "D" }) q.Enqueue(x);
    DequeueMany(q, 2);
    foreach (var x in new[] { "E", "F" }) q.Enqueue(x);
    Console.WriteLine($"  Truoc khi them G: {q.DumpState()}");
    q.Enqueue("G");
    Console.WriteLine($"  Sau khi them G:   {q.DumpState()}");
    int cap = q.Capacity;
    var outList = DequeueMany(q, 5);
    Result(cap == 8 && string.Join(",", outList) == "C,D,E,F,G",
           $"Capacity {cap}; lay ra {string.Join(", ", outList)}");
}

Header(6, "Quay vong rat nhieu lan", "Lap 1 000 000 lan: Enqueue roi Dequeue", "Khong loi; Capacity van 4");
{
    var q = new CircularQueue<int>();
    bool allMatch = true;
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < 1_000_000; i++)
    {
        q.Enqueue(i);
        if (q.Dequeue() != i) allMatch = false;
    }
    sw.Stop();
    Console.WriteLine($"  Trang thai cuoi: {q.DumpState()}");
    Result(allMatch && q.Capacity == 4 && q.Count == 0,
           $"Khong loi; Capacity {q.Capacity}; thoi gian {sw.Elapsed.TotalMilliseconds:F3} ms");
}

Header(7, "Ca lon, nhieu lan mo rong", "Enqueue 100 000 phan tu roi Dequeue het",
       "Lay ra dung thu tu; Capacity cuoi la 131072; ghi thoi gian");
{
    var q = new CircularQueue<int>();
    const int N = 100_000;
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < N; i++) q.Enqueue(i);
    int cap = q.Capacity;
    bool inOrder = true;
    for (int i = 0; i < N; i++)
        if (q.Dequeue() != i) inOrder = false;
    sw.Stop();
    Result(inOrder && cap == 131072 && q.Capacity == 131072 && q.Count == 0,
           $"Lay ra {(inOrder ? "dung" : "SAI")} thu tu; Capacity cuoi {q.Capacity}; thoi gian {sw.Elapsed.TotalMilliseconds:F3} ms");
}

Console.WriteLine();
Console.WriteLine($"Ket qua: {passed}/{Total} ca dat.");

// In tiêu đề một ca: số thứ tự, mục đích, thao tác và kết quả mong đợi.
static void Header(int no, string purpose, string ops, string expected)
{
    Console.WriteLine();
    Console.WriteLine($"===== Ca {no}: {purpose} =====");
    Console.WriteLine($"  Thao tac: {ops}");
    Console.WriteLine($"  Mong doi: {expected}");
}

void Result(bool ok, string actual)
{
    Console.WriteLine($"  Thuc te:  {actual}");
    Console.WriteLine($"  => {(ok ? "DAT" : "KHONG DAT")}");
    if (ok) passed++;
}

// Gọi Dequeue k lần và trả về danh sách các phần tử lấy ra theo thứ tự.
static List<T> DequeueMany<T>(CircularQueue<T> q, int k)
{
    var list = new List<T>(k);
    for (int i = 0; i < k; i++) list.Add(q.Dequeue());
    return list;
}

// Chạy một thao tác và trả về true nếu nó ném đúng InvalidOperationException.
static bool Throws(Action action, string name)
{
    try
    {
        action();
        Console.WriteLine($"  {name}: khong nem ngoai le (SAI)");
        return false;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  {name}: nem InvalidOperationException - \"{ex.Message}\"");
        return true;
    }
}

// Hàng đợi vòng cài bằng mảng, dùng ba biến front, rear, count.
public class CircularQueue<T>
{
    // Dung lượng ban đầu theo đề là 4.
    private const int InitialCapacity = 4;

    // Mảng lưu phần tử. Các phần tử hợp lệ nằm ở các ô front, front+1, ..., front+count−1 (lấy theo modulo Capacity).
    private T[] items = new T[InitialCapacity];
    // front: chỉ số ô chứa phần tử ĐẦU hàng (phần tử sẽ được Dequeue tiếp theo).
    private int front = 0;
    // rear: chỉ số ô TRỐNG kế tiếp, nơi phần tử Enqueue tiếp theo sẽ được ghi vào.
    private int rear = 0;
    // count: số phần tử đang có; giúp phân biệt rỗng và đầy khi front == rear.
    private int count = 0;

    // Số phần tử hiện có; chỉ đọc biến nên Θ(1).
    public int Count => count;

    // Dung lượng chính là độ dài mảng hiện tại; chỉ đọc nên Θ(1).
    public int Capacity => items.Length;

    // Thêm phần tử vào cuối hàng; tự mở rộng gấp đôi nếu mảng đầy.
    public void Enqueue(T value)
    {
        if (count == items.Length) Grow();

        items[rear] = value;
        rear = (rear + 1) % items.Length;
        count++;
    }

    // Lấy và xóa phần tử đầu hàng. Ném InvalidOperationException nếu hàng rỗng.
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException("Hang doi rong, khong the Dequeue.");

        T value = items[front];
        items[front] = default!;
        front = (front + 1) % items.Length;
        count--;
        return value;
    }

    // Xem phần tử đầu hàng mà không xóa. Ném InvalidOperationException nếu hàng rỗng.
    public T Peek()
    {
        if (count == 0) throw new InvalidOperationException("Hang doi rong, khong the Peek.");
        return items[front];
    }

    // Mở rộng mảng gấp đôi, chép các phần tử theo THỨ TỰ LOGIC bắt đầu từ front sang đầu mảng mới.
    private void Grow()
    {
        var newItems = new T[items.Length * 2];

        int firstPart = Math.Min(count, items.Length - front);
        Array.Copy(items, front, newItems, 0, firstPart);
        Array.Copy(items, 0, newItems, firstPart, count - firstPart);

        items = newItems;
        front = 0;
        rear = count;
    }

    // Trả về chuỗi mô tả trạng thái bên trong (mảng, front, rear, count) để minh họa khi chụp ảnh kiểm thử.
    public string DumpState()
    {
        var cells = new string[items.Length];
        for (int i = 0; i < cells.Length; i++) cells[i] = "_";
        for (int j = 0; j < count; j++)
        {
            int idx = (front + j) % items.Length;
            cells[idx] = items[idx]?.ToString() ?? "null";
        }
        return $"[{string.Join(", ", cells)}] front={front} rear={rear} count={count} capacity={Capacity}";
    }
}
