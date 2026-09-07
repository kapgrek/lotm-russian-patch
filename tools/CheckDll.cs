using System;
using System.IO;
using System.Text;

class CheckDll
{
    static void Main()
    {
        byte[] bytes = File.ReadAllBytes(@"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\AllinPCSDK.dll");
        string s = Encoding.ASCII.GetString(bytes);
        foreach (string term in new string[] { "font", "Aleo", "allin_data", "SourceHanSans", "HYQiHei" })
        {
            int idx = 0;
            Console.WriteLine("=== Term: " + term + " ===");
            while ((idx = s.IndexOf(term, idx, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                int start = Math.Max(0, idx - 30);
                int len = Math.Min(s.Length - start, 80);
                Console.WriteLine("Found at " + idx + ": " + s.Substring(start, len).Replace("\0", " "));
                idx += term.Length;
                if (idx > s.Length - 1) break;
            }
        }
    }
}
