using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class ExportNonCyrillicTsv
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string path = @"d:\gameDev\translate lotm\tools\non_cyrillic_ru_strings.txt";
        string outTsv = @"d:\gameDev\translate lotm\tools\non_cyrillic_to_translate.tsv";
        var lines = File.ReadAllLines(path, Encoding.UTF8);

        string curCn = "", curEn = "", curRu = "", curIdx = "";
        var list = new List<Tuple<string, string, string, string>>();

        Action add = () =>
        {
            if (string.IsNullOrEmpty(curIdx)) return;
            list.Add(Tuple.Create(curIdx, curCn, curEn, curRu));
        };

        foreach (var l in lines)
        {
            var mIdx = Regex.Match(l, @"^\[(\d+)\]\s*CN:\s*(.*)");
            if (mIdx.Success)
            {
                add();
                curIdx = mIdx.Groups[1].Value;
                curCn = mIdx.Groups[2].Value;
                curEn = "";
                curRu = "";
                continue;
            }
            var mEn = Regex.Match(l, @"^\s*EN:\s*(.*)");
            if (mEn.Success) { curEn = mEn.Groups[1].Value; continue; }
            var mRu = Regex.Match(l, @"^\s*RU:\s*(.*)");
            if (mRu.Success) { curRu = mRu.Groups[1].Value; continue; }
        }
        add();

        using (var sw = new StreamWriter(outTsv, false, Encoding.UTF8))
        {
            sw.WriteLine("Id\tCN\tEN\tRU");
            foreach (var item in list)
            {
                sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", item.Item1, item.Item2, item.Item3, item.Item4));
            }
        }
        Console.WriteLine(string.Format("Exported {0} entries to {1}", list.Count, outTsv));
    }
}
