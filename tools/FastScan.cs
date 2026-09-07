using System;
using System.IO;

class Program
{
    static void Main()
    {
        byte[] bytes = File.ReadAllBytes("Lord-of-Mysteries-English-Patch-2.6.exe");
        Console.WriteLine("Loaded " + bytes.Length + " bytes");
        
        // Print DOS / PE header
        Console.WriteLine("First 64 bytes hex:");
        for (int i = 0; i < 64; i++) Console.Write(bytes[i].ToString("X2") + " ");
        Console.WriteLine();

        // Search for 7z
        for (int i = 0; i < bytes.Length - 6; i++)
        {
            if (bytes[i] == 0x37 && bytes[i+1] == 0x7A && bytes[i+2] == 0xBC && bytes[i+3] == 0xAF && bytes[i+4] == 0x27 && bytes[i+5] == 0x1C)
            {
                Console.WriteLine("Found 7z at: " + i);
                File.WriteAllBytes("payload.7z", SubArray(bytes, i, bytes.Length - i));
                Console.WriteLine("Saved payload.7z (" + (bytes.Length - i) + " bytes)");
                break;
            }
        }

        // Search for PK 03 04
        int pkCount = 0;
        for (int i = 0; i < bytes.Length - 4; i++)
        {
            if (bytes[i] == 0x50 && bytes[i+1] == 0x4B && bytes[i+2] == 0x03 && bytes[i+3] == 0x04)
            {
                if (pkCount < 5) Console.WriteLine("Found PK0304 at: " + i);
                pkCount++;
            }
            if (bytes[i] == 0x50 && bytes[i+1] == 0x4B && bytes[i+2] == 0x05 && bytes[i+3] == 0x06)
            {
                Console.WriteLine("Found EOCD (PK0506) at: " + i);
            }
        }
        Console.WriteLine("Total PK0304: " + pkCount);
    }

    static byte[] SubArray(byte[] data, int index, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}
