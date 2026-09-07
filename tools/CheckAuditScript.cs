using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckAuditScript
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

        Console.WriteLine("Loaded ruDict: " + ruDict.Count);

        int countIdentical = 0;
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
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        string ru = null;
                        if (ruDict.ContainsKey(cn)) ru = ruDict[cn];
                        else if (ruDict.ContainsKey(en)) ru = ruDict[en];

                        if (ru != null && ru.Trim() == en.Trim())
                        {
                            countIdentical++;
                            if (countIdentical <= 5)
                            {
                                Console.WriteLine(string.Format("Identical #{0}: CN=[{1}], EN=[{2}], RU=[{3}]", countIdentical, cn, en, ru));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("Current total identical to EN in RuntimeTextRussian.lua: " + countIdentical);
    }
}
