using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckExistingShards
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string shardsDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        Console.WriteLine("Checking shards in " + shardsDir);

        int shardCount = 0;
        int totalEntries = 0;
        var shardFiles = Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua");
        shardCount = shardFiles.Length;

        foreach (var file in shardFiles)
        {
            using (var reader = new StreamReader(file, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string t = line.Trim();
                    if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                    {
                        totalEntries++;
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total shard files: {0}", shardCount));
        Console.WriteLine(string.Format("Total entries across all shards: {0}", totalEntries));
    }
}
