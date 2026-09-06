using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class TestHash
{
    // Lua bit.tobit(x) treats as int32
    static int SourceKeyHash(byte[] bytes)
    {
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash = hash ^ bytes[i];
            // hash + hash<<1 + hash<<4 + hash<<7 + hash<<8 + hash<<24
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
        string shard0 = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextGemini_000.lua";
        Console.WriteLine("Testing hash on " + shard0);

        int matched = 0;
        int mismatched = 0;

        using (var reader = new StreamReader(shard0, Encoding.UTF8))
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
                        string cnKey = t.Substring(2, delim - 2);
                        // Unescape Lua string
                        string unescaped = Regex.Unescape(cnKey);
                        string computed = GetPrefix(unescaped);
                        if (computed == "000")
                        {
                            matched++;
                        }
                        else
                        {
                            mismatched++;
                            if (mismatched <= 5)
                            {
                                Console.WriteLine(string.Format("Mismatch! Key: '{0}' -> computed '{1}' instead of '000'", cnKey, computed));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Matched: {0}, Mismatched: {1}", matched, mismatched));
    }
}
