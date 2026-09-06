using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class TestExactLookups
{
    static int SourceKeyHash(byte[] bytes)
    {
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash = hash ^ bytes[i];
            uint h1 = hash << 1;
            uint h4 = hash << 4;
            uint h7 = hash << 7;
            uint h8 = hash << 8;
            uint h24 = hash << 24;
            hash = unchecked(hash + h1 + h4 + h7 + h8 + h24);
        }
        return unchecked((int)hash);
    }

    static string GetPrefix(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int hash = SourceKeyHash(bytes);
        string hex = ((uint)hash).ToString("x8");
        string hashPrefix = hex.Substring(0, 3);
        int num = Convert.ToInt32(hashPrefix, 16);
        int shardNum = num / 4;
        return shardNum.ToString("x3");
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string shardsDir = @"d:\gameDev\translate lotm\data\shards";

        // Read line 118131 from Gemini
        string targetLine = null;
        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
        {
            for (int i = 1; i <= 118131; i++) targetLine = reader.ReadLine();
        }

        int delim = targetLine.IndexOf("\"] = \"");
        string cnKeyEscaped = targetLine.Substring(targetLine.IndexOf("[\"") + 2, delim - 6);
        int valStart = delim + 6;
        int valEnd = targetLine.EndsWith("\",") ? targetLine.Length - 2 : targetLine.Length - 1;
        string enValEscaped = targetLine.Substring(valStart, valEnd - valStart);

        string cnRaw = Regex.Unescape(cnKeyEscaped);
        string enRaw = Regex.Unescape(enValEscaped);

        Console.WriteLine("Testing exact Mind Fire lookup...");

        // 1. Test CN
        string cnPrefix = GetPrefix(cnRaw);
        string cnShardFile = Path.Combine(shardsDir, "RuntimeTextGemini_" + cnPrefix + ".lua");
        Console.WriteLine(string.Format("CN Key prefix: {0} (File: {1})", cnPrefix, File.Exists(cnShardFile) ? "EXISTS" : "MISSING"));

        bool cnFound = false;
        foreach (var line in File.ReadAllLines(cnShardFile, Encoding.UTF8))
        {
            if (line.Contains("Выпускает Пламя разума"))
            {
                cnFound = true;
                break;
            }
        }
        Console.WriteLine(string.Format("CN Key in Shard {0}: {1}", cnPrefix, cnFound ? "✅ FOUND" : "❌ NOT FOUND"));

        // 2. Test EN
        string enPrefix = GetPrefix(enRaw);
        string enShardFile = Path.Combine(shardsDir, "RuntimeTextGemini_" + enPrefix + ".lua");
        Console.WriteLine(string.Format("EN Key prefix: {0} (File: {1})", enPrefix, File.Exists(enShardFile) ? "EXISTS" : "MISSING"));

        bool enFound = false;
        foreach (var line in File.ReadAllLines(enShardFile, Encoding.UTF8))
        {
            if (line.Contains("Выпускает Пламя разума"))
            {
                enFound = true;
                break;
            }
        }
        Console.WriteLine(string.Format("EN Key in Shard {0}: {1}", enPrefix, enFound ? "✅ FOUND" : "❌ NOT FOUND"));
    }
}
