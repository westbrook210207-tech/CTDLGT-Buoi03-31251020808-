using System.Diagnostics;
using System.Globalization;
using System.Text;

if (args.Length > 0 && args[0] == "--test")
{
    RunTests();
    return;
}

if (args.Length > 0 && args[0] == "--gen-large")
{
    GenerateLargeInput(100_000, 10);
    return;
}

string all = Console.In.ReadToEnd();
string[] tok = all.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
int n = int.Parse(tok[0]);
int k = int.Parse(tok[1]);

var waiting = new Queue<Customer>(n);
for (int i = 0; i < n; i++)
{
    long a = long.Parse(tok[2 + 2 * i]);
    int s = int.Parse(tok[3 + 2 * i]);
    waiting.Enqueue(new Customer(i + 1, a, s));
}

var (results, summary) = ServiceSimulator.Simulate(waiting, k);
PrintReport(Console.Out, results, summary);

// In kết quả đúng định dạng đề: mỗi khách một dòng, sau đó ba dòng tổng kết.
static void PrintReport(TextWriter outw, List<ServiceResult> results, Summary summary)
{
    var sb = new StringBuilder();
    foreach (var r in results)
    {
        sb.Append("KH ").Append(r.Id).Append(": den ").Append(r.Arrival)
          .Append(", quay ").Append(r.Counter).Append(", bat dau ").Append(r.Start)
          .Append(", cho ").Append(r.Wait).Append(", xong ").Append(r.End).Append('\n');
    }
    sb.Append("Cho trung binh: ").Append(summary.AverageWait.ToString("F2", CultureInfo.InvariantCulture)).Append('\n');
    sb.Append("Cho lau nhat: ").Append(summary.MaxWait).Append('\n');
    sb.Append("Hoan thanh luc: ").Append(summary.FinishTime).Append('\n');
    outw.Write(sb.ToString());
}

// Tạo hàng chờ từ danh sách cặp (a, s) để dùng trong các ca kiểm thử.
static Queue<Customer> BuildQueue((long a, int s)[] data)
{
    var q = new Queue<Customer>(data.Length);
    for (int i = 0; i < data.Length; i++) q.Enqueue(new Customer(i + 1, data[i].a, data[i].s));
    return q;
}

// Chạy 5 ca nhỏ (in đầy đủ) và ca lớn (đo thời gian, chỉ in vài dòng đầu/cuối).
static void RunTests()
{
    var cases = new (int k, (long a, int s)[] data, string expected, string purpose)[]
    {
        (1, new (long, int)[] { (0, 5), (1, 3), (2, 4) }, "Cho: 0, 4, 6; TB 3.33; lau nhat 6; xong luc 12", "Mot quay"),
        (2, new (long, int)[] { (0, 5), (1, 3), (2, 4) }, "Cho: 0, 0, 2; quay: 1, 2, 2; TB 0.67; lau nhat 2; xong luc 8", "Hai quay"),
        (2, new (long, int)[] { (0, 3), (0, 3), (0, 3) }, "Cho: 0, 0, 3; TB 1.00; lau nhat 3; xong luc 6", "Den cung luc"),
        (1, new (long, int)[] { (0, 2), (10, 3) }, "Cho: 0, 0; TB 0.00; xong luc 13", "Quay ranh roi moi co khach"),
        (3, new (long, int)[] { (4, 6) }, "Cho: 0; xong luc 10", "n = 1, k lon hon n"),
    };

    for (int t = 0; t < cases.Length; t++)
    {
        var c = cases[t];
        string input = string.Join(" ", c.data.Select(d => $"({d.a},{d.s})"));
        Console.WriteLine($"===== Ca {t + 1}: {c.purpose} =====");
        Console.WriteLine($"Vao: n = {c.data.Length}, k = {c.k}  {input}");
        Console.WriteLine($"Mong doi: {c.expected}");
        Console.WriteLine("Thuc te:");
        var (res, sum) = ServiceSimulator.Simulate(BuildQueue(c.data), c.k);
        PrintReport(Console.Out, res, sum);
        Console.WriteLine();
    }

    Console.WriteLine("===== Ca 6: Ca lon =====");
    Console.WriteLine("Vao: n = 100000, k = 10, khach thu i den luc i-1, s = 10");
    var big = new (long a, int s)[100_000];
    for (int i = 0; i < big.Length; i++) big[i] = (i, 10);
    var bigQueue = BuildQueue(big);
    var sw = Stopwatch.StartNew();
    var (bigRes, bigSum) = ServiceSimulator.Simulate(bigQueue, 10);
    sw.Stop();
    var sw2 = Stopwatch.StartNew();
    PrintReport(TextWriter.Null, bigRes, bigSum);
    sw2.Stop();
    foreach (var r in bigRes.Take(3).Concat(bigRes.Skip(bigRes.Count - 3)))
        Console.WriteLine($"KH {r.Id}: den {r.Arrival}, quay {r.Counter}, bat dau {r.Start}, cho {r.Wait}, xong {r.End}");
    Console.WriteLine($"Cho trung binh: {bigSum.AverageWait.ToString("F2", CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Cho lau nhat: {bigSum.MaxWait}");
    Console.WriteLine($"Hoan thanh luc: {bigSum.FinishTime}");
    Console.WriteLine($"Thoi gian mo phong: {sw.Elapsed.TotalMilliseconds:F3} ms; tao dau ra 100000 dong: {sw2.Elapsed.TotalMilliseconds:F3} ms");
}

// In dữ liệu ca lớn theo đúng định dạng đầu vào của đề.
static void GenerateLargeInput(int n, int k)
{
    var sb = new StringBuilder();
    sb.Append(n).Append(' ').Append(k).Append('\n');
    for (int i = 0; i < n; i++) sb.Append(i).Append(" 10\n");
    Console.Out.Write(sb.ToString());
}

// Một khách hàng trong hàng chờ: số thứ tự, thời điểm đến và thời gian phục vụ.
public record Customer(int Id, long Arrival, int Service);

// Kết quả phục vụ một khách: quầy (đánh số từ 1), lúc bắt đầu, thời gian chờ, lúc xong.
public record ServiceResult(int Id, long Arrival, int Counter, long Start, long Wait, long End);

// Tổng kết toàn bộ mô phỏng.
public record Summary(double AverageWait, long MaxWait, long FinishTime);

// Mô phỏng k quầy phục vụ khách theo thứ tự đến (FIFO).
public static class ServiceSimulator
{
    public static (List<ServiceResult> results, Summary summary) Simulate(Queue<Customer> waiting, int k)
    {
        long[] freeAt = new long[k];

        int n = waiting.Count;
        var results = new List<ServiceResult>(n);

        long totalWait = 0;
        long maxWait = 0;
        long finish = 0;

        while (waiting.Count > 0)
        {
            Customer c = waiting.Dequeue();

            int best = 0;
            for (int q = 1; q < k; q++)
            {
                if (freeAt[q] < freeAt[best]) best = q;
            }

            long start = Math.Max(freeAt[best], c.Arrival);
            long wait = start - c.Arrival;
            long end = start + c.Service;
            freeAt[best] = end;

            totalWait += wait;
            if (wait > maxWait) maxWait = wait;
            if (end > finish) finish = end;

            results.Add(new ServiceResult(c.Id, c.Arrival, best + 1, start, wait, end));
        }

        double avg = n == 0 ? 0 : (double)totalWait / n;
        return (results, new Summary(avg, maxWait, finish));
    }
}
