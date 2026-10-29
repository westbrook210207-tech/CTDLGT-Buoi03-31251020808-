using System.Diagnostics;

Console.WriteLine("===== Ca 1: Enqueue 1,2,3; Dequeue; Enqueue 4; Dequeue 3 lan =====");
Console.WriteLine("Mong doi: lay ra 1 roi 2, 3, 4; Transfers = 4");
{
    var q = new TwoStackQueue<int>();
    q.Enqueue(1); q.Enqueue(2); q.Enqueue(3);
    int a = q.Dequeue();
    Console.WriteLine($"  Dequeue -> {a}  (Transfers = {q.Transfers})");
    q.Enqueue(4);
    var rest = new List<int>();
    for (int i = 0; i < 3; i++)
    {
        rest.Add(q.Dequeue());
        Console.WriteLine($"  Dequeue -> {rest[^1]}  (Transfers = {q.Transfers})");
    }
    bool ok = a == 1 && string.Join(",", rest) == "2,3,4" && q.Transfers == 4;
    Console.WriteLine($"Thuc te: lay ra {a} roi {string.Join(", ", rest)}; Transfers = {q.Transfers} => {(ok ? "DAT" : "KHONG DAT")}");
    try { q.Dequeue(); Console.WriteLine("  Dequeue tren hang rong: khong nem ngoai le (SAI)"); }
    catch (InvalidOperationException ex) { Console.WriteLine($"  Dequeue tren hang rong: nem InvalidOperationException - \"{ex.Message}\""); }
}

Console.WriteLine();
Console.WriteLine("===== Ca 2: 1 000 000 Enqueue roi 1 000 000 Dequeue =====");
Console.WriteLine("Mong doi: lay ra dung thu tu; Transfers = 1 000 000; thoi gian duoi 1 giay");
{
    const int N = 1_000_000;
    var q = new TwoStackQueue<int>();
    bool inOrder = true;
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < N; i++) q.Enqueue(i);
    for (int i = 0; i < N; i++)
        if (q.Dequeue() != i) inOrder = false;
    sw.Stop();
    bool ok = inOrder && q.Transfers == N && sw.ElapsedMilliseconds < 1000;
    Console.WriteLine($"Thuc te: thu tu {(inOrder ? "dung" : "SAI")}; Transfers = {q.Transfers}; thoi gian {sw.Elapsed.TotalMilliseconds:F3} ms => {(ok ? "DAT" : "KHONG DAT")}");
}

Console.WriteLine();
Console.WriteLine("===== Bang 6: n lan Enqueue roi n lan Dequeue =====");
Measure(new NaiveTwoStack<int>(), 100);
Measure(new TwoStackQueue<int>(), 100);
Console.WriteLine($"{"n",-10}{"NaiveTwoStack (ms)",-22}{"TwoStackQueue (ms)",-22}{"Transfers Naive",-20}Transfers TwoStack");
foreach (int n in new[] { 1_000, 5_000, 20_000 })
{
    var naive = new NaiveTwoStack<int>();
    double tNaive = Measure(naive, n);
    var fast = new TwoStackQueue<int>();
    double tFast = Measure(fast, n);
    Console.WriteLine($"{n,-10}{tNaive,-22:F3}{tFast,-22:F3}{naive.Transfers,-20}{fast.Transfers}");
}

// Đo thời gian n lần Enqueue rồi n lần Dequeue trên một hàng đợi bất kỳ cài giao diện ISimpleQueue.
static double Measure(ISimpleQueue<int> q, int n)
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < n; i++) q.Enqueue(i);
    for (int i = 0; i < n; i++)
        if (q.Dequeue() != i) throw new Exception("Sai thu tu FIFO");
    sw.Stop();
    return sw.Elapsed.TotalMilliseconds;
}

// Giao diện chung để đo thời gian hai cách cài đặt bằng cùng một hàm mà không tốn chi phí dynamic.
public interface ISimpleQueue<T>
{
    int Count { get; }
    long Transfers { get; }
    void Enqueue(T value);
    T Dequeue();
}

// Phương án A: hàng đợi chỉ dùng hai Stack.
public class TwoStackQueue<T> : ISimpleQueue<T>
{
    // Ngăn nhận: phần tử mới nhất nằm trên đỉnh (thứ tự ngược với thứ tự lấy ra).
    private readonly Stack<T> inbox = new();
    // Ngăn lấy ra: sau khi đổ, thứ tự bị đảo lại nên phần tử cũ nhất nằm trên đỉnh.
    private readonly Stack<T> outbox = new();

    // Tổng số phần tử đã đổ từ inbox sang outbox; dùng long vì có thể đổ hàng triệu phần tử.
    public long Transfers { get; private set; }

    // Số phần tử của hàng là tổng hai ngăn, vì mỗi phần tử nằm ở đúng một trong hai ngăn.
    public int Count => inbox.Count + outbox.Count;

    // Thêm vào cuối hàng: chỉ một lần Push vào ngăn nhận → Θ(1) (khấu hao của Stack).
    public void Enqueue(T value)
    {
        inbox.Push(value);
    }

    // Lấy phần tử đầu hàng. Ném InvalidOperationException nếu hàng rỗng.
    public T Dequeue()
    {
        if (Count == 0) throw new InvalidOperationException("Hang doi rong, khong the Dequeue.");

        if (outbox.Count == 0)
        {
            while (inbox.Count > 0)
            {
                outbox.Push(inbox.Pop());
                Transfers++;
            }
        }

        return outbox.Pop();
    }
}

// Bản đối chứng "ngây thơ": giữ mọi phần tử trong một ngăn chính với phần tử đầu hàng nằm trên ĐỈNH.
public class NaiveTwoStack<T> : ISimpleQueue<T>
{
    // Ngăn chính: đỉnh luôn là phần tử đầu hàng (cũ nhất), đáy là phần tử cuối hàng (mới nhất).
    private readonly Stack<T> main = new();
    // Ngăn phụ: chỉ dùng tạm trong lúc Enqueue để lật ngăn chính.
    private readonly Stack<T> aux = new();

    // Tổng số lần chuyển phần tử giữa hai ngăn (cả lượt đi và lượt về) để so sánh với TwoStackQueue.
    public long Transfers { get; private set; }

    // Mọi phần tử luôn nằm trong ngăn chính khi không có thao tác nào đang chạy.
    public int Count => main.Count;

    // Thêm vào cuối hàng: đổ hết sang ngăn phụ, Push phần tử mới, đổ lại → Θ(n).
    public void Enqueue(T value)
    {
        while (main.Count > 0)
        {
            aux.Push(main.Pop());
            Transfers++;
        }
        main.Push(value);
        while (aux.Count > 0)
        {
            main.Push(aux.Pop());
            Transfers++;
        }
    }

    // Lấy phần tử đầu hàng: chính là đỉnh ngăn chính → O(1).
    public T Dequeue()
    {
        if (main.Count == 0) throw new InvalidOperationException("Hang doi rong, khong the Dequeue.");
        return main.Pop();
    }
}
