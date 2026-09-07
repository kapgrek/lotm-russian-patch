using System;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        string exePath = @"d:\gameDev\translate lotm\Lord-of-Mysteries-English-Patch-2.6.exe";
        if (args.Length > 0) exePath = args[0];
        Console.WriteLine("Inspecting: " + exePath);
        if (!File.Exists(exePath))
        {
            Console.WriteLine("File not found: " + exePath);
            return;
        }

        try
        {
            using (ZipArchive zip = ZipFile.OpenRead(exePath))
            {
                Console.WriteLine("Total entries in zip: " + zip.Entries.Count);
                foreach (var entry in zip.Entries)
                {
                    Console.WriteLine(string.Format("{0,-60} | {1,10} bytes | {2}", entry.FullName, entry.Length, entry.LastWriteTime));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening zip: " + ex);
        }
    }
}
