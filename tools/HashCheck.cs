using System;
using System.Text;

class HashCheck
{
    static void Main()
    {
        string s = "Releases Mind Fire, with a *d** base probability to <HyperLink stylename=\"M_Link\" u=\"7\"> Imprison </> enemies in front for *f seconds and deal spellfielddisc(*id) magic damage. It then moves forward, erupting to deal *d magic damage to enemies in a large area, applying mul(*d**,*d) <HyperLink stylename=\"M_Link\" u=\"20\"> Healing Reduction </> for *f seconds, and leaving behind a fire zone that lasts for 4 seconds, dealing spellfielddisc(*id) magic damage to enemies within range every 0.7 seconds. When this skill deals damage, it has a *f** probability to apply 1 stack of <HighLight> Induction Mark </> for *f seconds. \\n Only available in <HighLight> Nightmare Stance </>; switches to <HighLight> Psychotherapy </> in <HighLight> Imagination Stance </>. \\n \\n <FaintYellow> Induction Mark </>: After stacking 3 times, there is a <Highlight> 100% </> base probability to enter the <HyperLink stylename=\"M_Link\" u=\"14\"> Hypnosis </> state for <Highlight> 1.5 </> seconds.";
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash = unchecked(hash + (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24));
        }
        string sKey = bytes.Length.ToString() + ":" + hash.ToString("x8");
        int colon = sKey.IndexOf(':');
        string hashPrefix = sKey.Substring(colon + 1, 3);
        int num = Convert.ToInt32(hashPrefix, 16);
        string prefix = (num / 4).ToString("x3");
        Console.WriteLine("Key: " + sKey);
        Console.WriteLine("Prefix: " + prefix);
    }
}
