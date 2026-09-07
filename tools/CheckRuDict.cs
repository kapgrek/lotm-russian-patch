using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckRuDict
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"RuntimeTextRussian.lua";
        string geminiPath = @"source_en\RuntimeTextGemini.lua";

        var ruKeys = new HashSet<string>(StringComparer.Ordinal);
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
                        ruKeys.Add(k);
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Total keys in RuntimeTextRussian.lua: {0}", ruKeys.Count));

        var geminiKeys = new HashSet<string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
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
                        geminiKeys.Add(k);
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Total keys in source_en/RuntimeTextGemini.lua: {0}", geminiKeys.Count));

        int ruNotInGemini = 0;
        foreach (var k in ruKeys)
        {
            if (!geminiKeys.Contains(k))
            {
                ruNotInGemini++;
            }
        }
        Console.WriteLine(string.Format("Keys in RuntimeTextRussian.lua NOT in source_en/RuntimeTextGemini.lua: {0}", ruNotInGemini));
    }
}
