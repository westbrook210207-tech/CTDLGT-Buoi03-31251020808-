using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length > 0 && args[0] == "--test")
{
    RunTests();
    return;
}

Console.Error.Write("Nhap chuoi: ");
string input = Console.ReadLine() ?? "";
Console.WriteLine(Format(BracketChecker.FindFirstError(input)));

// Đổi kết quả số của FindFirstError sang định dạng chữ: -1 là hợp lệ, số khác là vị trí lỗi.
static string Format(int pos) => pos == -1 ? "HOP LE" : $"SAI tai vi tri {pos}";

// Chạy 10 ca kiểm thử bắt buộc, so sánh với kết quả mong đợi và đo thời gian từng ca.
static void RunTests()
{
    var cases = new (string input, string display, string expected, string purpose)[]
    {
        ("{(a+b)×[c−d]}", "{(a+b)×[c−d]}", "HOP LE", "Truong hop thong thuong"),
        ("{(a+b]×c)", "{(a+b]×c)", "SAI tai vi tri 5", "Dong sai loai"),
        ("(a+b", "(a+b", "SAI tai vi tri 0", "Mo chua dong"),
        ("a+b)", "a+b)", "SAI tai vi tri 3", "Dong thua"),
        ("", "(chuoi rong)", "HOP LE", "Ca bien"),
        ("([)]", "([)]", "SAI tai vi tri 2", "Cac cap dan xen nhau"),
        ("a+b", "a+b", "HOP LE", "Khong co ngoac"),
        ("{[(", "{[(", "SAI tai vi tri 0", "Nhieu ngoac mo chua dong"),
        (new string('(', 500_000) + new string(')', 500_000), "500 000 '(' + 500 000 ')'", "HOP LE", "Ca lon"),
        (new string('(', 1_000_000), "1 000 000 '('", "SAI tai vi tri 0", "Ca lon; Stack sau nhat"),
    };

    BracketChecker.FindFirstError("([]{})");

    Console.WriteLine($"{"STT",-4}{"Chuoi vao",-28}{"Mong doi",-20}{"Thuc te",-20}{"Dat?",-6}{"Thoi gian",-12}Muc dich");
    Console.WriteLine(new string('-', 120));

    int passed = 0;
    for (int t = 0; t < cases.Length; t++)
    {
        var c = cases[t];
        var sw = Stopwatch.StartNew();
        string actual = Format(BracketChecker.FindFirstError(c.input));
        sw.Stop();
        bool ok = actual == c.expected;
        if (ok) passed++;
        Console.WriteLine($"{t + 1,-4}{c.display,-28}{c.expected,-20}{actual,-20}{(ok ? "DAT" : "SAI"),-6}{sw.Elapsed.TotalMilliseconds,9:F3} ms {c.purpose}");
    }

    Console.WriteLine(new string('-', 120));
    Console.WriteLine($"Ket qua: {passed}/{cases.Length} ca dat.");
}

// Kiểm tra dấu ngoặc ( ), [ ], { } cân bằng bằng Stack.
public static class BracketChecker
{
    // Trả về -1 nếu chuỗi hợp lệ; ngược lại trả về vị trí lỗi đầu tiên (chỉ số tính từ 0).
    public static int FindFirstError(string s)
    {
        var st = new Stack<(char ch, int pos)>();

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '(' || c == '[' || c == '{')
            {
                st.Push((c, i));
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                if (st.Count == 0) return i;

                var top = st.Pop();

                if (!IsPair(top.ch, c)) return i;
            }
        }

        if (st.Count == 0) return -1;

        while (st.Count > 1) st.Pop();

        return st.Peek().pos;
    }

    // Kiểm tra ngoặc mở và ngoặc đóng có cùng loại không.
    private static bool IsPair(char open, char close) =>
        (open == '(' && close == ')') || (open == '[' && close == ']') || (open == '{' && close == '}');
}
