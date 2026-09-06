using System;
using System.Text;

class HashCheck2
{
    static void Main()
    {
        string cnKey = "释放心灵之火，以*d**的基础概率<HyperLink stylename=\"M_Link\" u=\"7\">禁锢</>身前的敌方*f秒并造成spellfielddisc(*id)点魔法伤害，并向前移动喷发对大范围内敌方造成*d点魔法伤害并为目标附加*f秒mul(*d**,*d)<HyperLink stylename=\"M_Link\" u=\"20\">减疗</>，留下持续4秒的火场，火场每0.7秒对范围内敌方造成spellfielddisc(*id)点魔法伤害。本技能造成伤害时，有*f**的概率附加1层<HighLight>诱导印记</>，持续*f秒。\n仅<HighLight>噩梦姿态</>可用，<HighLight>空想姿态</>下切换为<HighLight>心理治疗</>。\n\n<FaintYellow>诱导印记</>：叠加3层后以<Highlight>100%</>的基础概率进入持续<Highlight>1.5</>秒的<HyperLink stylename=\"M_Link\" u=\"14\">催眠</>状态。";
        byte[] bytes = Encoding.UTF8.GetBytes(cnKey);
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
