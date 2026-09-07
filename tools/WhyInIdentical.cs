using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class WhyInIdentical
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"source_en\RuntimeTextGemini.lua";
        string ruPath = @"RuntimeTextRussian.lua";

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(ruPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string k = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                        ruDict[k] = v;
                    }
                }
            }
        }

        string testCn = "也许在那之前，我——罗塞尔·古斯塔夫，因蒂斯的皇帝，终将归来。";
        Console.WriteLine("Contains exact CN: " + ruDict.ContainsKey(testCn));
        if (ruDict.ContainsKey(testCn))
        {
            Console.WriteLine("RU value: [" + ruDict[testCn] + "]");
        }

        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("罗塞尔·古斯塔夫"))
                {
                    Console.WriteLine("Gemini line: " + line);
                    string t = line.Trim();
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        string ru = null;
                        if (ruDict.ContainsKey(cn)) ru = ruDict[cn];
                        else if (ruDict.ContainsKey(en)) ru = ruDict[en];

                        Console.WriteLine("Extracted CN: [" + cn + "]");
                        Console.WriteLine("Extracted EN: [" + en + "]");
                        Console.WriteLine("Extracted RU: [" + ru + "]");
                        Console.WriteLine("ru == en ? " + (ru == en));
                        Console.WriteLine("ru.Trim() == en.Trim() ? " + (ru != null && ru.Trim() == en.Trim()));
                    }
                }
            }
        }
    }
}
