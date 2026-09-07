using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

class Final71Patcher
{
    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string masterPath = @"RuntimeTextRussian.lua";
        Console.WriteLine("=== TRANSLATING AND PATCHING FINAL 71 STRINGS ===");

        var pairs = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // 1. Notice board in Tingen
            { @"\n<Letter_Highlight>采摘草药</>：在第三个月升日采摘铁筷子草，6镑。\n<Letter_Highlight>短期雇佣</>：雇佣一位有航海经验的保镖，为期一月，600镑。\n<Letter_Highlight>金梧桐区案件</>：本周金梧桐区发生一起恶性入室抢劫杀人案，提供相关线索者，一经核实有效，即可获得10镑奖励。\n<Letter_Highlight>通缉悬赏</>：通缉犯海盗格斯出现于廷根，每条有效线索5镑。\n<Letter_Highlight>寻狗启事</>：三日前爱犬维纳于水仙花街走丢，通体黑色面部白色，若有线索，重金酬谢！",
              @"\n<Letter_Highlight>Сбор трав</>: Сбор морозника в третий день восхода луны — 6 фунтов.\n<Letter_Highlight>Краткосрочный наём</>: Нанять телохранителя с морским опытом на один месяц — 600 фунтов.\n<Letter_Highlight>Дело района Золотого Вяза</>: На этой неделе в районе Золотого Вяза произошло жестокое ограбление с убийством. Награда за достоверную зацепку — 10 фунтов.\n<Letter_Highlight>Награда за поимку</>: Разыскиваемый пират Гас замечен в Тингене. 5 фунтов за каждую подтверждённую наводку.\n<Letter_Highlight>Пропала собака</>: Три дня назад на улице Нарциссов пропала любимая собака Вина, чёрная с белой мордой. Нашедшему — щедрое вознаграждение!" },
            { @"\n<Letter_Highlight>Picking herbs</>: Pick iron clematis on the third day of the month, 6 pounds.\n<Letter_Highlight>Short-term employment</>: Hire a bodyguard with sailing experience, for one month, 600 pounds.\n<Letter_Highlight>Golden Elm District Case</>: A vicious home invasion and murder occurred in the Golden Elm District this week. Those who provide relevant clues, once verified as effective, can receive a 10-pound reward.\n<Letter_Highlight>Wanted Bounty</>: Wanted pirate Gus appeared in Tingen, 5 pounds per effective clue.\n<Letter_Highlight>Dog Search Notice</>: Three days ago, my beloved dog Vina was lost on Daffodil Street, all black with a white face. If there are clues, heavy reward!",
              @"\n<Letter_Highlight>Сбор трав</>: Сбор морозника в третий день восхода луны — 6 фунтов.\n<Letter_Highlight>Краткосрочный наём</>: Нанять телохранителя с морским опытом на один месяц — 600 фунтов.\n<Letter_Highlight>Дело района Золотого Вяза</>: На этой неделе в районе Золотого Вяза произошло жестокое ограбление с убийством. Награда за достоверную зацепку — 10 фунтов.\n<Letter_Highlight>Награда за поимку</>: Разыскиваемый пират Гас замечен в Тингене. 5 фунтов за каждую подтверждённую наводку.\n<Letter_Highlight>Пропала собака</>: Три дня назад на улице Нарциссов пропала любимая собака Вина, чёрная с белой мордой. Нашедшему — щедрое вознаграждение!" },

            // 2. Room sharing notice
            { @"\n<Tips stylename=""Letter_Highlight"" id=""#160"">接受拼租：</>\n白班与夜班工人可以共享一张床位。\n<Tips stylename=""Letter_Highlight"" id=""#160"">安全保障：</>\n管理员24小时巡逻，只保障您个人安全，不保障您财务安全。\n<Tips stylename=""Letter_Highlight"" id=""#160"">设施齐全：</>\n热水价格仅为冷水的一倍，灶台公用，按小时收费。\n<Tips stylename=""Letter_Highlight"" id=""#160"">价格实惠：</>\n“每周仅需8便士，押金10便士，欢迎年轻工人入住。\n咨询请找管理员老汤姆——他在一楼左手第一个房间，随时恭候！",
              @"\n<Tips stylename=""Letter_Highlight"" id=""#160"">Совместная аренда:</>\nРабочие дневной и ночной смен могут делить одну кровать.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Безопасность:</>\nСмотритель патрулирует 24 часа в сутки. Обеспечивается только личная безопасность, сохранность имущества не гарантируется.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Удобства:</>\nГорячая вода всего вдвое дороже холодной. Плита общая, почасовая оплата.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Доступная цена:</>\n«Всего 8 пенсов в неделю, залог 10 пенсов. Приглашаем молодых рабочих.\nПо всем вопросам обращаться к смотрителю Старине Тому — первая комната слева на первом этаже, готов принять в любое время!»" },
            { @"\n<Tips stylename=""Letter_Highlight"" id=""#160"">Accepting room sharing:</>\nDay shift and night shift workers can share a bed.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Security guarantee:</>\nAdministrator patrols 24 hours a day, only guarantees your personal safety, does not guarantee your financial safety.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Complete facilities:</>\nHot water price is only double that of cold water, stove is public, charged by the hour.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Affordable price:</>\n""Only 8 pence per week, 10 pence deposit, young workers are welcome to move in.\nFor inquiries, please find the administrator Old Tom—he is in the first room on the left on the first floor, waiting at any time!",
              @"\n<Tips stylename=""Letter_Highlight"" id=""#160"">Совместная аренда:</>\nРабочие дневной и ночной смен могут делить одну кровать.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Безопасность:</>\nСмотритель патрулирует 24 часа в сутки. Обеспечивается только личная безопасность, сохранность имущества не гарантируется.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Удобства:</>\nГорячая вода всего вдвое дороже холодной. Плита общая, почасовая оплата.\n<Tips stylename=""Letter_Highlight"" id=""#160"">Доступная цена:</>\n«Всего 8 пенсов в неделю, залог 10 пенсов. Приглашаем молодых рабочих.\nПо всем вопросам обращаться к смотрителю Старине Тому — первая комната слева на первом этаже, готов принять в любое время!»" },

            // 3. Stained note Desire Messenger
            { @"\n<Tips stylename=""Letter_Highlight"" u=""1"" id=""#160_R"">布满污迹的纸条上，只剩几行还能辨认的字：</>\n“我终于明白了……”\n“第一轮是<Letter_Highlight_HW>二号</>！二号是<Letter_Highlight_HW>真的</>！”\n“欲念使者说的都是<Letter_Highlight_HW>实话</>，可我们还是会死——”\n<Hide stylename=""Transparent"" id=""#161_R"">字迹断了</>，<Hide id=""#157"">后面是一道擦过的暗色污痕。</>",
              @"\n<Tips stylename=""Letter_Highlight"" u=""1"" id=""#160_R"">На испачканной записке осталось лишь несколько разборчивых строк:</>\n«Я наконец-то понял...»\n«В первом раунде был <Letter_Highlight_HW>номер два</>! Номер два — <Letter_Highlight_HW>настоящий</>!»\n«Посланник Желаний говорил чистую <Letter_Highlight_HW>правду</>, но мы всё равно погибнем...»\n<Hide stylename=""Transparent"" id=""#161_R"">Почерк обрывается</>, <Hide id=""#157"">дальше идёт растёртое тёмное пятно.</>" },
            { @"\n<Tips stylename=""Letter_Highlight"" u=""1"" id=""#160_R"">On the stained note, only a few lines remain legible:</>\n""I finally understand...""\n""The first round is <Letter_Highlight_HW>number two</>! Number two is <Letter_Highlight_HW>real</>!""\n""What the Desire Messenger said is all <Letter_Highlight_HW>truth</>, but we will still die—""\n<Hide stylename=""Transparent"" id=""#161_R"">The handwriting broke</>, <Hide id=""#157"">behind it is a dark stain that has been wiped.</>",
              @"\n<Tips stylename=""Letter_Highlight"" u=""1"" id=""#160_R"">На испачканной записке осталось лишь несколько разборчивых строк:</>\n«Я наконец-то понял...»\n«В первом раунде был <Letter_Highlight_HW>номер два</>! Номер два — <Letter_Highlight_HW>настоящий</>!»\n«Посланник Желаний говорил чистую <Letter_Highlight_HW>правду</>, но мы всё равно погибнем...»\n<Hide stylename=""Transparent"" id=""#161_R"">Почерк обрывается</>, <Hide id=""#157"">дальше идёт растёртое тёмное пятно.</>" },

            // 4. Missing children in Tingen
            { @"\n……\n\n从七月初起，廷根市儿童失踪事件多次发生，失踪者总人数为<Mark id=""#159""> 十三人 </Item>，其中男孩五人、女孩八人。\n目前已找到三人尸体，并已将凶手捉捕结案。\n但尸检检定致死原因均不同，归属于三例不同的现实普通事件。",
              @"\n……\n\nС начала июля в Тингене участились случаи пропажи детей. Общее число пропавших без вести составляет <Mark id=""#159""> тринадцать человек </>, среди них пять мальчиков и восемь девочек.\nНа данный момент найдены тела троих, убийца задержан, дело закрыто.\nОднако вскрытие показало разные причины смерти, относящиеся к трём не связанным обычным происшествиям." },
            { @"\n……\n\nSince the beginning of July, child disappearance incidents in Tingen have occurred many times. The total number of missing persons is <Mark id=""#159""> thirteen </>, including five boys and eight girls.\nCurrently, three bodies have been found, and the murderer has been caught and the case closed.\nBut the autopsy determined that the causes of death were all different, belonging to three different realistic ordinary events.",
              @"\n……\n\nС начала июля в Тингене участились случаи пропажи детей. Общее число пропавших без вести составляет <Mark id=""#159""> тринадцать человек </>, среди них пять мальчиков и восемь девочек.\nНа данный момент найдены тела троих, убийца задержан, дело закрыто.\nОднако вскрытие показало разные причины смерти, относящиеся к трём не связанным обычным происшествиям." },

            // 5. Police protest note
            { @"\n　　……\n　　失踪者家属聚集警局门口抗议，警方表示“失踪案件均已登记处理”。据悉，近期此类事件已发生十余起，多为政府雇员。\n　　……\n\n<Note_Normal_HW>这些是我们的人，这家伙在干嘛！</>\n<Note_Normal_HW>他疯了，赶紧处理掉，不要留下把柄！</>\n<Note_Normal_HW>查理先生马上要参选了，这时候不能出任何丑闻！</>",
              @"\n　　……\n　　Семьи пропавших собрались у полицейского участка на акцию протеста. Полиция заявила, что «все случаи исчезновения зарегистрированы и расследуются». По сообщениям, недавно произошло более десяти подобных инцидентов, в основном среди госслужащих.\n　　……\n\n<Note_Normal_HW>Это же наши люди, что этот болван творит?!</>\n<Note_Normal_HW>Он спятил, немедленно избавьтесь от него, не оставляйте улик!</>\n<Note_Normal_HW>Мистер Чарли скоро баллотируется, сейчас никаких скандалов быть не должно!</>" },
            { @"\n　　……\n　　Families of the missing gathered at the police station to protest, and the police stated that ""all disappearance cases have been registered and processed."" It is reported that more than ten such incidents have occurred recently, mostly government employees.\n　　……\n\n<Note_Normal_HW>These are our people, what is this guy doing!</>\n<Note_Normal_HW>He's crazy, deal with it quickly, don't leave any evidence!</>\n<Note_Normal_HW>Mr. Charlie is about to run for office, there can't be any scandals at this time!</>",
              @"\n　　……\n　　Семьи пропавших собрались у полицейского участка на акцию протеста. Полиция заявила, что «все случаи исчезновения зарегистрированы и расследуются». По сообщениям, недавно произошло более десяти подобных инцидентов, в основном среди госслужащих.\n　　……\n\n<Note_Normal_HW>Это же наши люди, что этот болван творит?!</>\n<Note_Normal_HW>Он спятил, немедленно избавьтесь от него, не оставляйте улик!</>\n<Note_Normal_HW>Мистер Чарли скоро баллотируется, сейчас никаких скандалов быть не должно!</>" },

            // 6. Drawing cabbage vs art gallery
            { @"\n　　今天画了一幅画，同学说很好看，我也觉得很好看。\n　　但是父亲说这很丑，他需要我种出好看的白菜，南瓜，去市场上卖个好价钱。\n　　我说好看的画，在画廊也能卖出好价钱。他说菜市场没有门，我们能走过去。画廊的门，我们可推不开。\n　　“永暗的蜡烛，不毁的身躯”，<Letter_RedBig>我觉得这个时代很坏，我不想在这个时代浪费时间了。</>",
              @"\n　　Сегодня нарисовал картину. Одноклассники сказали, что она очень красивая, и я тоже так думаю.\n　　Но отец сказал, что это мазня. Ему нужно, чтобы я выращивал красивую капусту и тыквы, чтобы выгодно продать их на рынке.\n　　Я сказал, что красивые картины в галерее тоже стоят дорого. Он ответил, что на рынок нет дверей, и мы можем туда пройти. А двери галереи нам ни за что не открыть.\n　　«Свеча Вечной Тьмы, Несокрушимое Тело»... <Letter_RedBig>Я считаю, что эта эпоха ужасна, и я больше не хочу тратить на неё своё время.</>" },
            { @"\n　　Drew a picture today, my classmate said it was very beautiful, and I also think it is very beautiful.\n　　But Father said it was ugly, he needs me to grow good-looking cabbage and pumpkins to sell for a good price at the market.\n　　I said beautiful paintings can also sell for a good price in galleries. He said the vegetable market has no doors, and we can walk over. We can't push open the doors of the gallery.\n　　""Candle of Eternal Darkness, Indestructible Body"", <Letter_RedBig>I think this era is very bad, I don't want to waste time in this era anymore.</>",
              @"\n　　Сегодня нарисовал картину. Одноклассники сказали, что она очень красивая, и я тоже так думаю.\n　　Но отец сказал, что это мазня. Ему нужно, чтобы я выращивал красивую капусту и тыквы, чтобы выгодно продать их на рынке.\n　　Я сказал, что красивые картины в галерее тоже стоят дорого. Он ответил, что на рынок нет дверей, и мы можем туда пройти. А двери галереи нам ни за что не открыть.\n　　«Свеча Вечной Тьмы, Несокрушимое Тело»... <Letter_RedBig>Я считаю, что эта эпоха ужасна, и я больше не хочу тратить на неё своё время.</>" },

            // 7. Artifact See Me
            { @"\n　　名称：看见我\n　　外观描述：一块<Letter_Highlight>透明水晶</>，约拇指大小，清澈无瑕。\n　　能力评估：持有者可“看见”他人的真实情绪与表层想法。持续注视目标可使其产生短暂的“认同感”，从而微妙引导对方行为。\n　　副作用警示：每次使用，持有者<Letter_Highlight>随机丢失</>一段属于自己的<Letter_Highlight>记忆</>。\n\n　　备注：三周前失窃，占卜显示仍处于<Letter_Highlight>激活</>状态。",
              @"\n　　Название: «Узри меня»\n　　Описание внешнего вида: Кусок <Letter_Highlight>прозрачного кристалла</> размером с большой палец, чистый и безупречный.\n　　Оценка способностей: Владелец способен «видеть» истинные эмоции и поверхностные мысли других. Непрерывный взгляд на цель вызывает у неё кратковременное «чувство близости», позволяя незаметно направлять её действия.\n　　Предупреждение о побочных эффектах: При каждом использовании владелец <Letter_Highlight>случайно теряет</> фрагмент собственной <Letter_Highlight>памяти</>.\n\n　　Примечание: Украден три недели назад. Гадание показывает, что артефакт всё ещё находится в <Letter_Highlight>активном</> состоянии." },
            { @"\n　　Name: See Me\n　　Appearance description: A piece of <Letter_Highlight>transparent crystal</>, about the size of a thumb, clear and flawless.\n　　Ability assessment: The holder can ""see"" others' true emotions and surface thoughts. Continuously staring at the target can make them have a brief ""sense of identification,"" thereby subtly guiding the other party's behavior.\n　　Side effect warning: Every time it is used, the holder <Letter_Highlight>randomly loses</> a piece of their own <Letter_Highlight>memory</>.\n\n　　Note: Stolen three weeks ago, divination shows it is still in an <Letter_Highlight>activated</> state.",
              @"\n　　Название: «Узри меня»\n　　Описание внешнего вида: Кусок <Letter_Highlight>прозрачного кристалла</> размером с большой палец, чистый и безупречный.\n　　Оценка способностей: Владелец способен «видеть» истинные эмоции и поверхностные мысли других. Непрерывный взгляд на цель вызывает у неё кратковременное «чувство близости», позволяя незаметно направлять её действия.\n　　Предупреждение о побочных эффектах: При каждом использовании владелец <Letter_Highlight>случайно теряет</> фрагмент собственной <Letter_Highlight>памяти</>.\n\n　　Примечание: Украден три недели назад. Гадание показывает, что артефакт всё ещё находится в <Letter_Highlight>активном</> состоянии." },

            // Numbers & Multipliers
            { "%s倍", "%s-кратн." },
            { "%sx", "%s-кратн." },
            { "<Double>%s倍</>", "<Double>%s-кратн.</>" },
            { "<Double>%sx</>", "<Double>%s-кратн.</>" },
            { "1号", "№ 1" },
            { "No. 1", "№ 1" },
            { "3号", "№ 3" },
            { "No. 3", "№ 3" },
            { "4号", "№ 4" },
            { "No. 4", "№ 4" },
            { "第%s名", "№ %s" },
            { "No. %s", "№ %s" },
            { "第……", "№..." },
            { "The...", "№..." },
            { "角色编号：%s", "ID персонажа: %s" },
            { "Character ID: %s", "ID персонажа: %s" },

            // Dungeons & Bosses
            { "5208010   0129肉鸽本4   副本", "5208010   0129 Рогалик-подземелье 4   Подземелье" },
            { "5208010   0129 Roguelike Dungeon 4   Dungeon", "5208010   0129 Рогалик-подземелье 4   Подземелье" },
            { "5208011   0129肉鸽本5   副本", "5208011   0129 Рогалик-подземелье 5   Подземелье" },
            { "5208011   0129 Roguelike Dungeon 5   Dungeon", "5208011   0129 Рогалик-подземелье 5   Подземелье" },
            { "<BigText>契</>言交<BigText>换</>", "<BigText>Об</>мен клят<BigText>ва</>ми" },
            { "<BigText> Oa </> th Ex <BigText> chan </> ge", "<BigText>Об</>мен клят<BigText>ва</>ми" },
            { "BOSS_猎犬", "БОСС_Гончая" },
            { "BOSS_Hound", "БОСС_Гончая" },
            { "BattleZone_子嗣守卫", "БоеваяЗона_Страж потомства" },
            { "BattleZone_Offspring Guard", "БоеваяЗона_Страж потомства" },
            { "BattleZone_猎犬", "БоеваяЗона_Гончая" },
            { "BattleZone_Hound", "БоеваяЗона_Гончая" },
            { "Boss_子嗣守卫", "Босс_Страж потомства" },
            { "Boss_Offspring Guard", "Босс_Страж потомства" },
            { "Boss_梅高欧丝", "Босс_Мегоуз" },
            { "Boss_Megose", "Босс_Мегоуз" },
            { "Boss_特莉丝_战斗", "Босс_Трис_Битва" },
            { "Boss_Trissy_Battle", "Босс_Трис_Битва" },
            { "Boss_猎犬", "Босс_Гончая" },

            // NPCs
            { "NPC_档案编号_0123", "NPC_Архивный_Номер_0123" },
            { "NPC_Archive_Number_0123", "NPC_Архивный_Номер_0123" },
            { "NPC_空气1", "NPC_Воздух 1" },
            { "NPC_Air 1", "NPC_Воздух 1" },
            { "NPC_空气2", "NPC_Воздух 2" },
            { "NPC_Air 2", "NPC_Воздух 2" },
            { "NPC_西迦", "NPC_Сика" },
            { "NPC_Xiga", "NPC_Сика" },
            { "Npc_传送_一号信徒", "NPC_Телепорт_Верующий номер один" },
            { "NPC_Teleport_Believer Number One", "NPC_Телепорт_Верующий номер один" },
            { "Npc_传送_猎犬", "NPC_Телепорт_Гончая" },
            { "Npc_Teleport_Hound", "NPC_Телепорт_Гончая" },
            { "Npc_弗莱_入口", "NPC_Фрай_Вход" },
            { "Npc_Frye_Entrance", "NPC_Фрай_Вход" },
            { "Npc_弗莱_入口_1", "NPC_Фрай_Вход_1" },
            { "Npc_Frye_Entrance_1", "NPC_Фрай_Вход_1" },

            // Puzzles & Mechanics
            { "Tri3_谜题1", "Tri3_Загадка 1" },
            { "Tri3_Puzzle 1", "Tri3_Загадка 1" },
            { "Tri3_谜题2", "Tri3_Загадка 2" },
            { "Tri3_Puzzle 2", "Tri3_Загадка 2" },
            { "Tri3_谜题3", "Tri3_Загадка 3" },
            { "Tri3_Puzzle 3", "Tri3_Загадка 3" },
            { "Tri3_谜题4", "Tri3_Загадка 4" },
            { "Tri3_Puzzle 4", "Tri3_Загадка 4" },
            { "wave1_假", "волна1_ложная" },
            { "wave1_fake", "волна1_ложная" },
            { "wave2_假", "волна2_ложная" },
            { "wave2_fake", "волна2_ложная" },
            { "“剥面人”挑战%s第%s%s", "Испытание «Сдирателя лиц» %s № %s %s" },
            { "Face-Ripper Challenge %s No. %s %s", "Испытание «Сдирателя лиц» %s № %s %s" },
            { "副本_五月庄园_Boss战流程_先祖骑士_困难", "Подземелье_Поместье Мэй_Бой с боссом_Рыцарь Предков_Сложно" },
            { "Dungeon_May Manor_Boss Fight Flow_Ancestor Knight_Hard", "Подземелье_Поместье Мэй_Бой с боссом_Рыцарь Предков_Сложно" },
            { "功能_运动会发动攻击_召唤物_顺序1", "Функция_Атака состязания_Призыв_Очерёдность 1" },
            { "Function_Sports Meet attack launch_Summon_Sequence 1", "Функция_Атака состязания_Призыв_Очерёдность 1" },
            { "多萝西_捉迷藏藏", "Дороти_Прятки_Спрятаться" },
            { "Dorothy_Hide and Seek Hide", "Дороти_Прятки_Спрятаться" },
            { "对话_左手伸出", "Диалог_Протянуть левую руку" },
            { "Dialogue_LeftHandExtended", "Диалог_Протянуть левую руку" },
            { "机制_祭坛_法阵", "Механика_Алтарь_Магический круг" },
            { "Mechanic_Altar_Magic Circle", "Механика_Алтарь_Магический круг" },
            { "梦境_空间", "Пространство сновидений" },
            { "Dream_Space", "Пространство сновидений" },
            { "海精灵-黑动态", "Морской дух — Тёмная анимация" },
            { "Sea Spirit-Black Dynamic", "Морской дух — Тёмная анимация" },
            { "缩壳回护", "Защитный панцирь" },
            { "Shell Guard", "Защитный панцирь" },

            // Values & Stats
            { "召唤物压制_数值", "Подавление призванных: значение" },
            { "Summon Suppression_Value", "Подавление призванных: значение" },
            { "护甲_数值", "Броня: значение" },
            { "Armor_Value", "Броня: значение" },
            { "敏捷_数值", "Ловкость: значение" },
            { "Agility_Value", "Ловкость: значение" },
            { "眩晕命中_数值", "Оглушение: шанс попадания" },
            { "Stun Hit_Value", "Оглушение: шанс попадания" },
            { "眩晕增强_数值", "Оглушение: усиление" },
            { "Stun Enhancement_Value", "Оглушение: усиление" },
            { "眩晕抵挡_数值", "Оглушение: сопротивление" },
            { "Stun Block_Value", "Оглушение: сопротивление" },
            { "眩晕闪避_数值", "Оглушение: уклонение" },
            { "Stun Dodge_Value", "Оглушение: уклонение" },
            { "睡眠闪避_数值", "Усыпление: уклонение" },
            { "Sleep Dodge_Value", "Усыпление: уклонение" },

            // Effects
            { "特效_传送_一号信徒", "Эффект_Телепорт_Верующий номер один" },
            { "Effect_Teleport_Believer Number One", "Эффект_Телепорт_Верующий номер один" },
            { "特效_传送_入口", "Эффект_Телепорт_Вход" },
            { "Effect_Teleport_Entrance", "Эффект_Телепорт_Вход" },
            { "特效_传送_猎犬", "Эффект_Телепорт_Гончая" },
            { "Effect_Teleport_Hound", "Эффект_Телепорт_Гончая" },
            { "特效_解锁_01", "Эффект_Разблокировка_01" },
            { "Effect_Unlock_01", "Эффект_Разблокировка_01" },
            { "特效_锁定_01", "Эффект_Блокировка_01" },
            { "Effect_Lock_01", "Эффект_Блокировка_01" },

            // Triggers
            { "触发器_先祖骑士_加载", "Триггер_Рыцарь Предков_Загрузка" },
            { "Trigger_Ancestor Knight_Load", "Триггер_Рыцарь Предков_Загрузка" },
            { "触发器_先祖骑士_表演", "Триггер_Рыцарь Предков_Выступление" },
            { "Trigger_Ancestor Knight_Performance", "Триггер_Рыцарь Предков_Выступление" },
            { "触发器_子嗣守卫_加载", "Триггер_Страж потомства_Загрузка" },
            { "Trigger_Spawn Guard_Load", "Триггер_Страж потомства_Загрузка" },
            { "触发器_德鲁伊", "Триггер_Друид" },
            { "Trigger_Druid", "Триггер_Друид" },
            { "触发器_猎犬_加载", "Триггер_Гончая_Загрузка" },
            { "Trigger_Hound_Load", "Триггер_Гончая_Загрузка" },
            { "触发器_猎犬表演管家_开始表演", "Триггер_Выступление гончей дворецкого_Старт" },
            { "Trigger_Hound Performance Butler_Start Performance", "Триггер_Выступление гончей дворецкого_Старт" },
            { "触发器_管家_加载", "Триггер_Дворецкий_Загрузка" },
            { "Trigger_Butler_Load", "Триггер_Дворецкий_Загрузка" },

            // NetEase & System
            { "猪厂", "Компания NetEase" },
            { "网易", "NetEase" },
            { "NetEase", "NetEase" },
            { "真\r\n贪\r\n心", "Как\r\nже\r\nжадно" },
            { "So\nGreedy", "Как\r\nже\r\nжадно" }
        };

        Console.WriteLine("Updating RuntimeTextRussian.lua with " + pairs.Count + " mappings...");
        var lines = new List<string>(File.ReadAllLines(masterPath, Encoding.UTF8));
        var updated = new List<string>(lines.Count + pairs.Count);
        var keyRegex = new Regex(@"^\s*\[""(.*)""\]\s*=\s*""(.*)"",?\s*$");
        var handledKeys = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            var m = keyRegex.Match(line);
            if (m.Success)
            {
                string rawKey = m.Groups[1].Value.Replace("\\\"", "\"");
                if (pairs.ContainsKey(rawKey))
                {
                    string newRu = CleanForLua(pairs[rawKey]);
                    updated.Add(string.Format("    [\"{0}\"] = \"{1}\",", m.Groups[1].Value, newRu));
                    handledKeys.Add(rawKey);
                    continue;
                }
            }
            updated.Add(line);
        }

        // Insert unhandled before the closing brace
        int insertPos = updated.Count;
        for (int i = updated.Count - 1; i >= 0; i--)
        {
            if (updated[i].Trim().StartsWith("}"))
            {
                insertPos = i;
                break;
            }
        }

        int added = 0;
        foreach (var kvp in pairs)
        {
            if (!handledKeys.Contains(kvp.Key))
            {
                string k = CleanForLua(kvp.Key);
                string v = CleanForLua(kvp.Value);
                updated.Insert(insertPos++, string.Format("    [\"{0}\"] = \"{1}\",", k, v));
                added++;
            }
        }

        File.WriteAllLines(masterPath, updated, Encoding.UTF8);
        Console.WriteLine(string.Format("Master dictionary updated! Replaced: {0}, Added: {1}", handledKeys.Count, added));
    }
}
