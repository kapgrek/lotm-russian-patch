using System;
using System.Net;
using System.Text;

class TestTranslate
{
    static void Main()
    {
        try
        {
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q=" + Uri.EscapeDataString("Hello world");
            using (var wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "Mozilla/5.0");
                byte[] data = wc.DownloadData(url);
                string json = Encoding.UTF8.GetString(data);
                Console.WriteLine("Response: " + json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
