using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class AuditRussianDictionary
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string path = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found: " + path);
            return;
        }

        Console.WriteLine("Auditing " + path + "...");
        int totalLines = 0;
        int validEntries = 0;
        int malformedLines = 0;
        int brokenEscapes = 0;
        int unclosedQuotes = 0;
        int duplicateKeys = 0;

        var keys = new HashSet<string>(StringComparer.Ordinal);

        using (var reader = new StreamReader(path, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                totalLines++;
                string t = line.Trim();
                if (totalLines <= 3 || t == "}") continue;

                if (!t.StartsWith("[\""))
                {
                    malformedLines++;
                    if (malformedLines <= 10) Console.WriteLine(string.Format("Line {0} does not start with [\" : {1}", totalLines, t.Substring(0, Math.Min(60, t.Length))));
                    continue;
                }

                // Check for trailing backslash before closing quote
                if (t.EndsWith("\\\",") || t.EndsWith("\\\""))
                {
                    brokenEscapes++;
                    if (brokenEscapes <= 10) Console.WriteLine(string.Format("Line {0} has broken escape at end: {1}", totalLines, t));
                }

                int delim = t.IndexOf("\"] = \"");
                if (delim < 0)
                {
                    malformedLines++;
                    if (malformedLines <= 10) Console.WriteLine(string.Format("Line {0} missing delimiter \"] = \" : {1}", totalLines, t.Substring(0, Math.Min(60, t.Length))));
                    continue;
                }

                string key = t.Substring(2, delim - 2);
                if (keys.Contains(key))
                {
                    duplicateKeys++;
                }
                else
                {
                    keys.Add(key);
                }

                if (!t.EndsWith("\",") && !t.EndsWith("\""))
                {
                    unclosedQuotes++;
                    if (unclosedQuotes <= 10) Console.WriteLine(string.Format("Line {0} does not end with \", : {1}", totalLines, t.Substring(Math.Max(0, t.Length - 40))));
                    continue;
                }

                validEntries++;
            }
        }

        Console.WriteLine("\n--- Summary ---");
        Console.WriteLine("Total lines: " + totalLines);
        Console.WriteLine("Valid entries: " + validEntries);
        Console.WriteLine("Unique keys: " + keys.Count);
        Console.WriteLine("Duplicate keys: " + duplicateKeys);
        Console.WriteLine("Malformed lines: " + malformedLines);
        Console.WriteLine("Broken escapes at line ends: " + brokenEscapes);
        Console.WriteLine("Unclosed quotes: " + unclosedQuotes);
    }
}
