using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

        var ruKeys = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(ruPath))
        {
            foreach (var line in File.ReadAllLines(ruPath, Encoding.UTF8))
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
        Console.WriteLine("Russian keys in dict: " + ruKeys.Count);

        int totalGemini = 0;
        int translated = 0;
        var untranslated = new List<Tuple<string, string>>();

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
                        totalGemini++;
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (ruKeys.Contains(cn) || ruKeys.Contains(en))
                        {
                            translated++;
                        }
                        else
                        {
                            untranslated.Add(Tuple.Create(cn, en));
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total in Gemini: " + totalGemini);
        Console.WriteLine("Translated: " + translated);
        Console.WriteLine("Untranslated: " + untranslated.Count);

        using (var writer = new StreamWriter(@"d:\gameDev\translate lotm\tools\remaining_untranslated.txt", false, Encoding.UTF8))
        {
            for (int i = 0; i < untranslated.Count; i++)
            {
                writer.WriteLine(string.Format("[{0}] CN: {1} | EN: {2}", i + 1, untranslated[i].Item1, untranslated[i].Item2));
            }
        }
        Console.WriteLine("Saved remaining untranslated to tools\\remaining_untranslated.txt");

        Console.WriteLine("\n--- First 25 Samples of Untranslated ---");
        for (int i = 0; i < Math.Min(25, untranslated.Count); i++)
        {
            Console.WriteLine(string.Format("{0}: CN={1} | EN={2}", i + 1, untranslated[i].Item1, untranslated[i].Item2));
        }
    }
}
