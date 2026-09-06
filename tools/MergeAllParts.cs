using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static Dictionary<string, string> entries = new Dictionary<string, string>();
    static List<string> orderedKeys = new List<string>();

    static void LoadLua(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found: " + filePath);
            return;
        }

        string content = File.ReadAllText(filePath, Encoding.UTF8);
        int pos = 0;
        int len = content.Length;
        int count = 0;

        while (pos < len)
        {
            int keyStart = content.IndexOf("[\"", pos);
            if (keyStart == -1) break;
            keyStart += 2; // skip ["

            // Read key until unescaped "]
            StringBuilder sbKey = new StringBuilder();
            bool escaped = false;
            int keyEnd = -1;

            for (int i = keyStart; i < len - 1; i++)
            {
                char c = content[i];
                if (escaped)
                {
                    sbKey.Append(c);
                    escaped = false;
                }
                else if (c == '\\')
                {
                    sbKey.Append(c);
                    escaped = true;
                }
                else if (c == '"' && content[i + 1] == ']')
                {
                    keyEnd = i;
                    break;
                }
                else
                {
                    sbKey.Append(c);
                }
            }

            if (keyEnd == -1) break;

            // Look for = "
            int valAssign = content.IndexOf("=", keyEnd + 2);
            if (valAssign == -1) break;

            int valQuoteStart = content.IndexOf("\"", valAssign + 1);
            if (valQuoteStart == -1) break;
            valQuoteStart += 1;

            // Read value until unescaped "
            StringBuilder sbVal = new StringBuilder();
            escaped = false;
            int valEnd = -1;

            for (int i = valQuoteStart; i < len; i++)
            {
                char c = content[i];
                if (escaped)
                {
                    sbVal.Append(c);
                    escaped = false;
                }
                else if (c == '\\')
                {
                    sbVal.Append(c);
                    escaped = true;
                }
                else if (c == '"')
                {
                    valEnd = i;
                    break;
                }
                else
                {
                    sbVal.Append(c);
                }
            }

            if (valEnd == -1) break;

            string key = sbKey.ToString();
            string val = sbVal.ToString();

            if (!entries.ContainsKey(key))
            {
                orderedKeys.Add(key);
            }
            entries[key] = val;
            count++;

            pos = valEnd + 1;
        }

        Console.WriteLine(string.Format("Loaded {0} entries from {1}. Total unique keys: {2}", count, Path.GetFileName(filePath), entries.Count));
    }

    static void Main(string[] args)
    {
        string rootDir = @"d:\gameDev\translate lotm";
        string targetFile = Path.Combine(rootDir, "RuntimeTextRussian.lua");

        // 1. Load original base
        Console.WriteLine("Loading base RuntimeTextRussian.lua...");
        LoadLua(targetFile);

        // 2. Load all 6 parts
        for (int i = 1; i <= 6; i++)
        {
            string partPath = Path.Combine(rootDir, "tools", "translated_part" + i + ".lua");
            LoadLua(partPath);
        }

        // 3. Write back cleanly
        Console.WriteLine(string.Format("Writing {0} entries to {1}...", entries.Count, targetFile));
        using (StreamWriter sw = new StreamWriter(targetFile, false, new UTF8Encoding(false)))
        {
            sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
            sw.WriteLine(string.Format("-- Entries: {0}", entries.Count));
            sw.WriteLine("return {");

            for (int i = 0; i < orderedKeys.Count; i++)
            {
                string key = orderedKeys[i];
                string val = entries[key];
                sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", key, val));
            }

            sw.WriteLine("}");
        }

        Console.WriteLine("Successfully merged and written RuntimeTextRussian.lua!");
    }
}
