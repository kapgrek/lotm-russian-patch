using System;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main()
    {
        string exePath = @"d:\gameDev\translate lotm\Lord-of-Mysteries-English-Patch-2.2.exe";
        Console.WriteLine("Inspecting " + exePath);
        try
        {
            using (ZipArchive zip = ZipFile.OpenRead(exePath))
            {
                Console.WriteLine("Total entries in zip: " + zip.Entries.Count);
                int count = 0;
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        if (count < 30) Console.WriteLine("  " + entry.FullName + " (" + entry.Length + " bytes)");
                        count++;
                    }
                }
                Console.WriteLine("Total matching entries: " + count);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Not a zip or error: " + ex.Message);
        }
    }
}
