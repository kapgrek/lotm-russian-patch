using System;
using System.IO;

class TestLock
{
    static void Main()
    {
        string p = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\allin_data\font\Aleo_TitleNew.ttf";
        try
        {
            using (var fs = File.Open(p, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                Console.WriteLine("Can open for read/write: YES");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Can open for read/write: NO - " + ex.Message);
        }
    }
}
