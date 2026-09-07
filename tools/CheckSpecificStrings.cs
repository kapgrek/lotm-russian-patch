using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckSpecificStrings
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"source_en\RuntimeTextGemini.lua";
        string ruPath = @"RuntimeTextRussian.lua";
        string batchPath = @"translation_batches\batch_01.tsv";

        var bLines = File.ReadAllLines(batchPath, Encoding.UTF8);
        Console.WriteLine("Checking first 5 lines of batch_01.tsv in RuntimeTextRussian.lua:");
        
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

        for (int i = 1; i <= 5 && i < bLines.Length; i++)
        {
            var parts = bLines[i].Split('\t');
            string id = parts[0];
            string cn = parts[1];
            string en = parts[2];
            Console.WriteLine(string.Format("\n--- Item #{0} ---", id));
            Console.WriteLine("EN: " + (en.Length > 60 ? en.Substring(0, 60) + "..." : en));
            
            if (ruDict.ContainsKey(cn))
            {
                Console.WriteLine("In RU Dict by CN: " + (ruDict[cn].Length > 60 ? ruDict[cn].Substring(0, 60) + "..." : ruDict[cn]));
            }
            else
            {
                Console.WriteLine("In RU Dict by CN: NOT FOUND");
            }

            if (ruDict.ContainsKey(en))
            {
                Console.WriteLine("In RU Dict by EN: " + (ruDict[en].Length > 60 ? ruDict[en].Substring(0, 60) + "..." : ruDict[en]));
            }
            else
            {
                Console.WriteLine("In RU Dict by EN: NOT FOUND");
            }
        }
    }
}
