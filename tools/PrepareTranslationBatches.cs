using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class PrepareBatches
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string tsvPath = @"tools\untranslated_identical_to_en.tsv";
        string batchDir = @"translation_batches";
        if (!Directory.Exists(batchDir)) Directory.CreateDirectory(batchDir);

        var lines = File.ReadAllLines(tsvPath, Encoding.UTF8);
        Console.WriteLine("Total lines in TSV: " + lines.Length);
        
        var entries = new List<Tuple<string, string>>();
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split('\t');
            if (parts.Length >= 2)
            {
                entries.Add(Tuple.Create(parts[0], parts[1]));
            }
        }
        Console.WriteLine("Parsed valid entries: " + entries.Count);

        int batchSize = 250;
        int batchCount = (entries.Count + batchSize - 1) / batchSize;
        Console.WriteLine("Creating " + batchCount + " batches of up to " + batchSize + " entries...");

        for (int b = 0; b < batchCount; b++)
        {
            int start = b * batchSize;
            int count = Math.Min(batchSize, entries.Count - start);
            string batchFile = Path.Combine(batchDir, string.Format("batch_{0:D2}.tsv", b + 1));
            
            var sb = new StringBuilder();
            sb.AppendLine("ID\tCN\tEN\tRU_TRANSLATION");
            for (int j = 0; j < count; j++)
            {
                var e = entries[start + j];
                sb.AppendLine(string.Format("{0}\t{1}\t{2}\t", start + j + 1, e.Item1, e.Item2));
            }
            File.WriteAllText(batchFile, sb.ToString(), Encoding.UTF8);
            Console.WriteLine(string.Format("Batch {0:D2}: {1} entries -> {2}", b + 1, count, batchFile));
        }
    }
}
