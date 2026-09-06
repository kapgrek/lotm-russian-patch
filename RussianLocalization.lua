-- Russian Localization Module for Lord of Mysteries
-- Developed for LOM Mod Loader
local Russian = {
    Enabled = true,
    Version = "1.1.0",
}

-- Названия вкладок главного меню и навигации (компактные для предотвращения переполнения сетки)
Russian.shortMenuLabels = {
    Fashion = "Стиль",
    Pastime = "Поход",
    Dungeon = "Данжи",
    PVP = "Арена",
    Equip = "Эквип",
    Skill = "Навыки",
    Talent = "Древо",
    Promotion = "Путь",
    Sealed = "Артефакты",
    SecretPartner = "Куклы",
    Fellow = "Союз",
    Paotuan = "TRPG",
    Guild = "Клуб",
    Home = "Замок",
    Task = "Квесты",
    Family = "Семья",
    Qingyuan = "Связи",
    Achievement = "Слава",
    Strategy = "Гайды",
    VideoCreation = "Медиа",
    Friend = "Друзья",
    ShadowCity = "Тьма",
    Character = "Герой",
    HomePage = "Главная",
    Bag = "Сумка",
    Notice = "Инфо",
    Email = "Почта",
    Rank = "Топ",
    Detach = "Снять",
    Setting = "Опции",
    QuitGame = "Выход",
}

-- Имена и способности марионеток
Russian.marionetteEnglishNames = {
    [87303350] = "Пришествие рассвета",
    [87303360] = "Клеймо арбитража",
    [87303370] = "Взор тайновидца",
    [87303380] = "Защита утреннего света",
    [87303390] = "Клятва рыцаря",
    [87303400] = "Одержимость духом бабочки",
    [87303410] = "Нисходящая тень",
    [87303420] = "Эхо погребального звона",
    [87303430] = "Серия когтей вожака",
    [87303440] = "Защита бура",
}

-- Выборы при создании персонажа (вопросы Таро / характера)
Russian.creatorChoiceLabels = {
    [1] = { "Безумие", "Рассудок" },
    [2] = { "Мудрость", "Сила" },
    [3] = { "Слава", "Чувства" },
}

-- Системные константы и действия интерфейса
Russian.stringConstOverrides = {
    BAG_AUTO_AUTO_RESOLVE_TITLE = "Подтверждение авто-распыления",
    BAG_AUTO_DECOMPOSE_TITLE = "Настройки авто-распыления",
    COMMENT_PANEL_TITLE = "Комментарии",
    DIALOGUE_SKIP = "Пропустить",
    EQUIPMENT_PLAN_APPLY_CURRENT_PLAN = "Применить сборку",
    FASHION_APPEARANCE = "Внешний вид",
    FASHION_DYE_MY_PLAN = "Мои стили",
    GUILD_CARGO_HUB_REWARD_COMPLETE = "Получено",
    GVG_HONOR_CLAIMED_TEXT = "Получено",
    ITEM_GOT = "Получено",
    MONTH_CARD_MAIN_PAGE_TODAY_RECEIVED_LABEL = "(Получено сегодня)",
    FAMILY_INVITE_SHARE_TEAM = "Канал группы",
    FAMILY_INVITE_SHARE_WORLD = "Мировой канал",
    FAMILY_MEMBER_COUNT_FMT = "Члены семьи: %s/14",
    FAMILY_MEMBER_FMT = "Члены семьи (%d/%d)",
    ONE_CLICK_IN_USE = "Используется",
    ONE_CLICK_RECOMMEND_PLAN = "Рекомендованная сборка",
    ONE_CLICK_SHARE_RECOMMEND_PLAN = "Рекомендованные сборки",
    ONE_CLICK_TITLE = "Помощник",
    ONE_CLICK_USE = "Использовать",
    TRAINTRADE_ITEM_DISCOUNT_CHINESE = "Скидка %d0%%",
    MAP_PVP_LAST_HUNT_DRAGON_BOSS_BELONG_FORMAT = "Принадлежность команде: <Green>%s</>",
    MAP_PVP_LAST_HUNT_DRAGON_BOSS_NAME = "Проекция Дракона",
    MAP_PVP_LAST_HUNT_DRAGON_BOSS_NOT_BELONG_FORMAT = "Принадлежность команде: <Red>%s</>",
    PVP_LAST_HUNT_ACTIVE_TIME_FORMAT = "Активация через %M:%S",
    PVP_LAST_HUNT_ACTIVITY_NOT_OPEN_TEXT = "Используйте <Highlight>Семя Вздохов</>, чтобы пробудить Силу Вздохов, начать задание и получить ценные награды.",
    PVP_LAST_HUNT_ACTIVITY_OPEN_FORMAT = "Начало через %H ч. %M мин.",
    PVP_LAST_HUNT_ACTIVITY_OPEN_TEXT = "Время начала события",
    PVP_LAST_HUNT_ACTIVITY_REWARD_PREVIEW_FORMAT = "Награды за задание",
    PVP_LAST_HUNT_BOSS_BUTTON_DESC = "Перейти",
    PVP_LAST_HUNT_BOSS_CONTENT_DESC = "Описание Дракона Королевского Города",
    PVP_LAST_HUNT_BOSS_DETAIL_CONDITION_TITLE = "Статус возрождения",
    PVP_LAST_HUNT_BOSS_DETAIL_CONTENT = "Побеждайте элитных монстров ради ценных наград",
    PVP_LAST_HUNT_BOSS_DETAIL_NOT_SPAWNED = "Цель еще не появилась",
    PVP_LAST_HUNT_BOSS_DETAIL_SPAWNED = "Цель появилась на поле боя",
    PVP_LAST_HUNT_BOSS_DETAIL_TITLE = "Цель охоты",
    PVP_LAST_HUNT_BOSS_DRAGON_FORMAT = "Дракон появится через %M:%S",
    PVP_LAST_HUNT_BOSS_SECOND_TITLE = "Сразить Дракона Королевского Города",
    PVP_LAST_HUNT_BOSS_TAG_NAME = "Страж Королевского Города",
    PVP_LAST_HUNT_BOSS_TITLE = "Убийство Дракона",
    PVP_LAST_HUNT_CAMP_SUBMIT_FORMAT = "Точка сдачи: %s",
    PVP_LAST_HUNT_CHAT_BUTTON_TEXT = "Перейти",
    PVP_LAST_HUNT_CHAT_TITLE = "Горн",
    PVP_LAST_HUNT_CROSS_SERVER_SCORE_TITLE = "Боевые заслуги",
    PVP_LAST_HUNT_DETAIL_MY_DATA_TAB = "Мои данные",
    PVP_LAST_HUNT_DETAIL_RANK_TAB = "Рейтинг",
    PVP_LAST_HUNT_FIGHT_ASSISTANT_FORMAT = "%s потерпел поражение от %s в локации %s. Требуется поддержка!",
    PVP_LAST_HUNT_FIGHT_KILL_RESULT_FORMAT = "%s успешно одолел %s в %s!",
    PVP_LAST_HUNT_GUILD_ACTIVITY_DESC_TIPS = "Финальный рейд на Дракона <Highlight>[Аукцион команды]</>: %d/%d (в эту пятницу) в 19:10",
    PVP_LAST_HUNT_GUILD_NAME_FORMAT = "Клуб <Enemy_Name>%s</>",
    PVP_LAST_HUNT_HIGHER_DETAIL_CONTENT = "Зона повышенной сложности с множеством вышедших из-под контроля монстров",
    PVP_LAST_HUNT_HIGHER_DETAIL_NOT_OPENED_TITLE = "В данный момент закрыто",
    PVP_LAST_HUNT_HIGHER_DETAIL_OPENED_TIME = "Открыто ежедневно: 19:00-21:00\nДополнительно: суббота и воскресенье, 14:00-16:00",
    PVP_LAST_HUNT_HIGHER_DETAIL_TITLE_NAME = "Продвинутый уровень · Прилив",
    PVP_LAST_HUNT_HUD_PROGRESS_CURRENCY_FORMAT = "<Highlight>%s</>/%s",
    PVP_LAST_HUNT_HUD_PROGRESS_FORMAT = "<Highlight>%d</>/%d",
    PVP_LAST_HUNT_ITEM_CAN_NOT_USE = "Недостаточное количество",
    PVP_LAST_HUNT_LACK_USE_ITEM_PROP_COUNT_FORMAT = "Осталось попыток: %s",
    PVP_LAST_HUNT_MAIN_PROGRESS_TITLE = "Предпросмотр наград",
    PVP_LAST_HUNT_MAP_DETAIL_DESC = "Вход в телепорт зоны фракции",
    PVP_LAST_HUNT_MAP_ITEM_NAME = "Семя Вздохов · Зона Прилива Монстров",
    PVP_LAST_HUNT_MEMBER_COUNT_FORMAT = "(Члены группы: %d/%d)",
    PVP_LAST_HUNT_MONSTER_CANCEL_BUTTON_NAME = "Отмена",
    PVP_LAST_HUNT_MONSTER_DROP_REWARD_TEXT = "Шанс выпадения с побеждённых <HyperLink stylename=\"Clickable\" u=\"\">младших монстров</>",
    PVP_LAST_HUNT_MONSTER_DROP_REWARD_UNDERLINE_TEXT = "Шанс выпадения с побеждённых <HyperLink stylename=\"Underline\" u=\"\">младших монстров</>",
    PVP_LAST_HUNT_MONSTER_RECOMMEND_GROUP = "Реком. отряд",
    PVP_LAST_HUNT_MONSTER_RECOMMEND_TEAM = "Реком. группа",
    PVP_LAST_HUNT_MONSTER_SUMMON_BUTTON_NAME = "Призвать",
    PVP_LAST_HUNT_MONSTER_SUMMON_LEFT_COUNT_FORMAT = "Осталось призывов на этой неделе: %d",
    PVP_LAST_HUNT_NOT_OPENED_BUTTON_TEXT = "Доступно во время события",
    PVP_LAST_HUNT_RANK_TAB_GUILD_NAME = "Клуб",
    PVP_LAST_HUNT_RANK_TAB_PERSONAL_NAME = "Личный",
    PVP_LAST_HUNT_RESURGENCE_TIPS = "Выберите точку возрождения и нажмите «В бой»",
    PVP_LAST_HUNT_RESURGENCE_TITLE = "Выбор точки возрождения",
    PVP_LAST_HUNT_REVIVE_BUTTON_NAME = "Возродиться",
    PVP_LAST_HUNT_REWARD_PREVIEW_TITLE = "Награды задания",
    PVP_LAST_HUNT_SCORE_TITLE = "Очки ранга",
    PVP_LAST_HUNT_SEND_BUTTON_TITLE = "Сигнал",
    PVP_LAST_HUNT_SEND_CHAT_DEFAULT_TEXT = "Братья, на помощь!",
    PVP_LAST_HUNT_SEND_DEFAULT_TIP_TEXT = "Призыв до %d игроков",
    PVP_LAST_HUNT_SEND_PANEL_TIPS = "Призыв до 14 игроков",
    PVP_LAST_HUNT_SEND_PANEL_TITLE = "Сигнал горна",
    PVP_LAST_HUNT_SETTLE_MENT_ASSIST_NUM_TITLE = "Помощь",
    PVP_LAST_HUNT_SETTLE_MENT_CANCEL = "Отмена",
    PVP_LAST_HUNT_SETTLE_MENT_KILL_NUM_TITLE = "Убийства",
    PVP_LAST_HUNT_SETTLE_MENT_LEAVE = "Покинуть",
    PVP_LAST_HUNT_SETTLE_MENT_PROGRESS_NUM_TITLE = "Итоги охоты",
    PVP_LAST_HUNT_SETTLE_MENT_SCORE_NUM_TITLE = "Очки ранга",
    PVP_LAST_HUNT_SETTLE_MENT_TITLE = "Итоги охоты",
    PVP_LAST_HUNT_SUBMIT_CONTENT = "Сдайте материалы Алых реликвий в обмен на Ваучеры охоты",
    PVP_LAST_HUNT_SUBMIT_REFRESH_DESC = "Дворецкий охоты меняет позицию каждые 30 минут. При ожесточённом бое появляется больше дворецких.",
    PVP_LAST_HUNT_SUBMIT_REFRESH_TITLE = "Правила обновления",
    PVP_LAST_HUNT_SUBMIT_TITLE = "Дворецкий охоты",
    PVP_LAST_HUNT_SUMMON_AUTHOER_FORMAT = "(Призвал: %s)",
    PVP_LAST_HUNT_SUMMON_MONSTER_GET_NUM = "Получено попыток",
    PVP_LAST_HUNT_SUMMON_MONSTER_LACK_NUM = "На этой неделе не осталось попыток. Зарабатывайте ваучеры охоты для получения.",
    PVP_LAST_HUNT_TASK_BUFF_NAME = "Сила вздохов",
    PVP_LAST_HUNT_TASK_COMMIT_TEXT = "Сдать",
    PVP_LAST_HUNT_TASK_FINISH_TITLE_TEXT = "Завершено",
    PVP_LAST_HUNT_TASK_FRAGMENT_NAME = "Фрагмент добычи",
    PVP_LAST_HUNT_TASK_NOT_ACTIVE_CONTENT_TEXT = "Используйте Семя вздохов, побеждайте монстров или грабьте игроков для получения фрагментов добычи, затем сдайте Графу Порядка.",
    PVP_LAST_HUNT_TASK_NOT_ACTIVE_FINISH_TEXT = "Задание завершено. Найдите Графа Порядка, чтобы сдать фрагменты за награду.",
    PVP_LAST_HUNT_TASK_NOT_ACTIVE_TEXT = "Неактивно",
    PVP_LAST_HUNT_TASK_PROGRESS_TEXT = "Прогресс охоты",
    PVP_LAST_HUNT_TASK_PROP_TEXT = "Семя вздохов",
    PVP_LAST_HUNT_TASK_QUICK_TEAM = "Быстрая группа",
    PVP_LAST_HUNT_TASK_TITLE_NAME = "Финальная охота",
    PVP_LAST_HUNT_TITLE_DETAIL_NAME = "Подробнее",
    PVP_LAST_HUNT_TITLE_FOLD_NAME = "Свернуть",
    PVP_LAST_HUNT_USE_ITEM_NOT_ACTIVITY_OPEN_FORMAT = "Нельзя использовать вне события. Время: <highlight>%s-%s</>",
    PVP_LAST_HUNT_USE_ITEM_PROP_DESC = "Использование Семени вздохов...",
    PVP_LAST_HUNT_USE_TASK_TEXT_NAME = "Принять задание",
    RED_PACKET_ALREADY_RECEIVED = "Получено",
    SECRET_PARTNER_BTN_ALREADY_CHANGE_ACTOR_NAME = "Смена",
    SECRET_PARTNER_BTN_CHANGE_ACTOR_NAME = "Сменить",
    SECRET_PARTNER_CANCEL_CHANGE_ACTOR = "Отмена смены",
    SECRET_PARTNER_CHANGE_ACTOR_TITLE = "Сменить цель",
    SECRET_PARTNER_SKILL_TEXT = "Навык марионетки",
    SECRET_PARTNER_STAR_UP_TEXT_FORMAT = "Последовательность %d",
    SKILL_PRESET_TAB_1 = "Рекомендуемые сборки",
    TASK_TRACE_DISTANCE = "м",
    TRINITY_ALL_TREASURE_HAVE_CLAIMED = "Все награды получены",
    TEAM_INVITE_SECRET_PARTNER_TITLE = "Призыв марионетки",
    UIAPPEARANCE_USE = "Использовать",
    UIAPPEARANCE_USING = "Используется",
}

-- Прямой словарь Английский -> Русский (для текстов, уже переведенных патчем в English)
Russian.skillTags = {
    ["Finisher Skill"] = "Добив.",
    ["Finisher"] = "Добив.",
    ["Finishing Skill"] = "Добив.",
    ["Finisher Skills"] = "Добив.",
    ["Finishing"] = "Добив.",
    ["Execution Skill"] = "Добив.",
    ["终结技"] = "Добив.",
    ["终结技能"] = "Добив.",
    ["终结"] = "Добив.",
    ["Healing"] = "Лечение",
    ["Heal"] = "Лечение",
    ["Heals"] = "Лечение",
    ["治疗"] = "Лечение",
    ["Group"] = "Группа",
    ["Party"] = "Группа",
    ["群体"] = "Группа",
    ["群攻"] = "Группа",
    ["队伍"] = "Группа",
    ["Single Target"] = "Одиноч.",
    ["Single"] = "Одиноч.",
    ["单体"] = "Одиноч.",
    ["Area Target"] = "Область",
    ["Area"] = "Область",
    ["AOE"] = "Область",
    ["AoE"] = "Область",
    ["范围"] = "Область",
    ["Output"] = "Урон",
    ["Damage"] = "Урон",
    ["输出"] = "Урон",
    ["伤害"] = "Урон",
    ["Burst"] = "Взрыв",
    ["爆发"] = "Взрыв",
    ["Continuous"] = "Период.",
    ["DOT"] = "Период.",
    ["DoT"] = "Период.",
    ["持续"] = "Период.",
    ["Survival"] = "Защита",
    ["Defense"] = "Защита",
    ["Shield"] = "Щит",
    ["生存"] = "Защита",
    ["防御"] = "Защита",
    ["Displacement"] = "Рывок",
    ["Movement"] = "Рывок",
    ["位移"] = "Рывок",
    ["Control Break"] = "Снятие контр.",
    ["Cleanse"] = "Снятие контр.",
    ["解控"] = "Снятие контр.",
    ["Hard Crowd Control"] = "Жёстк. контр.",
    ["硬控"] = "Жёстк. контр.",
    ["Soft Crowd Control"] = "Мягк. контр.",
    ["软控"] = "Мягк. контр.",
    ["Crowd Control"] = "Контроль",
    ["Control"] = "Контроль",
    ["控制"] = "Контроль",
    ["Strengthening"] = "Усиление",
    ["Enhance"] = "Усиление",
    ["强化"] = "Усиление",
    ["Receive Blue Card"] = "+Синяя карта",
    ["Receive Yellow Card"] = "+Жёлт. карта",
    ["Self"] = "Себя",
    ["自身"] = "Себя",
    ["Target"] = "Цель",
    ["目标"] = "Цель",
    ["Passive"] = "Пассив.",
    ["被动"] = "Пассив.",
    ["Normal"] = "Обычный",
    ["普通"] = "Обычный",
    ["Special"] = "Особый",
    ["特殊"] = "Особый",
    ["Roleplay"] = "Роль",
    ["扮演"] = "Роль",
    ["Puppets"] = "Куклы",
    ["Puppet"] = "Куклы",
    ["Marionette"] = "Куклы",
    ["Marionettes"] = "Куклы",
    ["Fellows"] = "Связи",
    ["Fellow"] = "Связи",
    ["Bond"] = "Связи",
    ["One-click Assist"] = "Помощник",
    ["One-Click Assist"] = "Помощник",
    ["一键辅助"] = "Помощник",
    ["Weaken"] = "Ослаб.",
    ["Weakening"] = "Ослабление",
    ["削弱"] = "Ослаб.",
    ["Debuff"] = "Дебафф",
    ["Buff"] = "Бафф",
    ["Stun"] = "Оглуш.",
    ["Silence"] = "Безмолв.",
    ["Slow"] = "Замедл.",
    ["Knockdown"] = "Сбивание",
    ["Airborne"] = "Подброс",
    ["Taunt"] = "Провок.",
    ["Stealth"] = "Скрытн.",
    ["Invisibility"] = "Незрим.",
    ["Immunity"] = "Иммунитет",
    ["Invincibility"] = "Неуязвим.",
    ["Bleed"] = "Кровотеч.",
    ["Bleeding"] = "Кровотеч.",
    ["Burn"] = "Горение",
    ["Ignite"] = "Горение",
    ["Freeze"] = "Заморозка",
    ["Frozen"] = "Заморозка",
    ["Poison"] = "Отравление",
    ["Vulnerability"] = "Уязвим.",
    ["易伤"] = "Уязвим.",
}

Russian.englishToRussian = {
    ["Level cap reached"] = "Макс. ур.",
    ["(Level cap reached)"] = "(Макс. ур.)",
    ["Character ID: %s"] = "ID: %s",
    ["Character ID:"] = "ID:",
    ["Character ID"] = "ID",
    ["Placeholder"] = "Инфо",
    ["Poetic Aura"] = "Поэтическая\nаура",
    ["Tranquility Aura"] = "Аура\nпокоя",
    ["Spirit Mediumship Ritual"] = "Спиритический\nритуал",
    ["Necromancer Mark"] = "Метка\nнекроманта",
    ["Law Edict"] = "Приговор\nзакона",
    ["Trick Performance."] = "Шоу\nфокусов",
    ["Good Luck Ritual"] = "Ритуал\nудачи",
    ["Fortune-Turning Ritual."] = "Ритуал\nудачи",
    ["Mystery Peeping Ritual"] = "Ритуал\nтайн",
    ["Curtain Call"] = "Конец\nспектакля",
    ["Devout Prayer"] = "Истовая\nмолитва",
    ["Pendulum Dowsing"] = "Гадание на\nмаятнике",
    ["Door Opening"] = "Открытие\nдвери",
    ["Mind Soothing"] = "Утешение\nразума",
    ["Trick Illusion"] = "Иллюзорный\nфокус",
    ["Spirit Remnant"] = "Остаточный\nобраз",
    ["Listen to Heart"] = "Глас\nсердца",
    ["Ready to Strike"] = "Готовность\nк удару",
    ["Historical Void"] = "Пустота\nистории",
    ["Historical void"] = "Пустота\nистории",
    ["Marionette Manipulation"] = "Управление\nкуклами",
    ["Puppet Manipulation"] = "Управление\nкуклами",
    ["Card Storm"] = "Дождь\nкарт",
    ["Rain of Cards"] = "Дождь\nкарт",
    ["Throw Cards"] = "Бросок\nкарт",
    ["Air Missile"] = "Воздушная\nракета",
    ["Fate Yellow Card"] = "Жёлтая карта\nсудьбы",
    ["Spirituality Blue Card"] = "Синяя карта\nдуховности",
    ["Finisher Skill Shuffle"] = "Тасование\nкарт",
    ["Finisher Skill Shuffle Cards"] = "Тасование\nкарт",
    ["Mystic Illusion"] = "Таинственная\nиллюзия",
    ["Torrent of Information"] = "Поток\nзнаний",
    ["Holy Flame"] = "Святое\nпламя",
    ["Sanctuary Protection"] = "Защита\nСвятилища",
    ["Sanctuary Blessing"] = "Защита\nСвятилища",
    ["Combat Stance"] = "Боевая\nстойка",
    ["Battle Stance Mark"] = "Боевая\nстойка",
    ["Honor Slash"] = "Удар\nславы",
    ["Hurricane"] = "Ураган",
    ["Dusk Hurricane"] = "Закатный\nураган",
    ["Decay Blade"] = "Клинок\nувядания",
    ["Sea of Subconsciousness"] = "Море\nподсознания",
    ["Psychological Suggestion"] = "Психологич.\nвнушение",
    ["Necrotic Mark"] = "Метка\nнекроманта",
    ["Magic Mushroom"] = "Волшебный\nгриб",
    ["Wave Riding"] = "Покорение\nволн",
    ["Law Judgment"] = "Приговор\nзакона",
    ["Requiem Poem"] = "Упокойная\nпоэма",
    ["Ensemble Poem"] = "Созвучие\nстихов",
    ["Midnight Poem"] = "Полуночная\nпоэма",
    ["Fortune-Turning Ritual"] = "Ритуал\nудачи",
    ["Gray Fog Suppression"] = "Подавление\nтумана",
    ["gray fog Suppression"] = "Подавление\nтумана",
    ["Gray Fog Blessing"] = "Благословение\nтумана",
    ["gray fog Blessing"] = "Благословение\nтумана",
    ["Historical Beacon"] = "Исторический\nмаяк",
    ["Air Cannon"] = "Воздушная\nпушка",
    ["Mind Insight"] = "Взор\nразума",
    ["Drill Guard"] = "Защита\nбура",
    ["Leader Claw Combo"] = "Когти\nвожака",
    ["Descending Shadow"] = "Нисходящая\nтень",
    ["Knight's Vow"] = "Клятва\nрыцаря",
    ["Dawn Light Guard"] = "Защита\nрассвета",
    ["Vision of Mystery"] = "Взор\nтайновидца",
    ["Insight Gaze"] = "Взор\nтайновидца",
    ["Mystery Ritual"] = "Ритуал\nтайн",
    ["Sword Master"] = "Мастер\nмеча",
    ["Performance Ends"] = "Конец\nспектакля",
    ["Pious Prayer"] = "Истовая\nмолитва",
    ["Witchcraft"] = "Колдовство",
    ["Mysterious Illusion"] = "Таинственная\nиллюзия",
    ["Spear of Longinus"] = "Копьё\nЛонгина",
    ["Information Torrent"] = "Поток\nзнаний",
    ["Vortex of Knowledge"] = "Поток\nзнаний",
    ["Star Sand"] = "Звёздный\nпесок",
    ["Nebula Chant"] = "Ария\nтуманности",
    ["Nebula Aria"] = "Ария\nтуманности",
    ["Footprints"] = "Следы",
    ["Footprint Reappearance"] = "Следы",
    ["Flashback"] = "Возврат",
    ["Folding Screen"] = "Ширма\nпространств",
    ["Space Cage"] = "Клетка\nпространства",
    ["Starlight Cage"] = "Звёздная\nклетка",
    ["Trick Performance"] = "Шоу\nфокусов",
    ["Finisher Skill Layered Gate"] = "Врата\nпространств",
    ["Layered Gate"] = "Врата\nпространств",
    ["Enveloped in Holy Flames"] = "Святое\nпламя",
    ["Holy Flames"] = "Святое\nпламя",
    ["Refraction of Light"] = "Преломление\nсвета",
    ["Sun Oath"] = "Солнечная\nклятва",
    ["Solar Oath"] = "Солнечная\nклятва",
    ["Punishment"] = "Кара",
    ["God's Punishment"] = "Божья\nкара",
    ["Divine Punishment"] = "Божья\nкара",
    ["Holy Light Protection"] = "Защита\nСвета",
    ["Sun Chaser"] = "Ловец\nСолнца",
    ["Unshadowed Spear"] = "Копьё\nСвета",
    ["Soul of the Blazing Sun"] = "Душа\nСолнца",
    ["Blazing Sun"] = "Яркое\nСолнце",
    ["Dawn Armor"] = "Доспех\nрассвета",
    ["Combat Stance Imprint"] = "Боевая\nстойка",
    ["Slash of Glory"] = "Удар\nславы",
    ["Silver Rapier"] = "Серебряная\nрапира",
    ["Sunset Hurricane"] = "Закатный\nураган",
    ["Royal Court Command"] = "Приказ\nдвора",
    ["Exorcism Slash"] = "Удар\nэкзорцизма",
    ["Hunter Instant Slash"] = "Быстрый\nвыпад",
    ["Demon Hunter Instant Slash"] = "Быстрый\nвыпад",
    ["Blade of Withering"] = "Клинок\nувядания",
    ["Greatsword Slash"] = "Удар\nдвуручником",
    ["Angry Slam"] = "Яростный\nудар",
    ["Plague"] = "Чума",
    ["Mental Nightmare"] = "Ментальный\nкошмар",
    ["Mind Reading"] = "Чтение\nмыслей",
    ["Psychotherapy"] = "Психо-\nтерапия",
    ["Pacify"] = "Умиро-\nтворение",
    ["Dream Rebirth"] = "Перерож-\nдение",
    ["Dream Recovery"] = "Возврат\nсна",
    ["Mind Fire"] = "Пламя\nразума",
    ["Psychological Invisibility"] = "Незри-\nмость",
    ["Consciousness Shock"] = "Удар\nсознания",
    ["Mental Guidance"] = "Ментальное\nвнушение",
    ["Deterrence"] = "Устра-\nшение",
    ["Mental Plague"] = "Ментальная\nчума",
    ["Consciousness Manipulation"] = "Контроль\nсознания",
    ["Dream Analysis"] = "Анализ\nснов",
    ["Dream Weaving"] = "Плетение\nснов",
    ["Frenzy"] = "Бешенство",
    ["Insight"] = "Прозре-\nние",
    ["Circular area with a radius of 12 meters"] = "Круглая область радиусом 12 м",
    ["Circular area with a radius of 10 meters"] = "Круглая область радиусом 10 м",
    ["Circular area with a radius of 8 meters"] = "Круглая область радиусом 8 м",
    ["Circular area with a radius of 6 meters"] = "Круглая область радиусом 6 м",
    ["Circular area with a radius of 4 meters"] = "Круглая область радиусом 4 м",
    ["Circular area with a radius of 5 meters"] = "Круглая область радиусом 5 м",
    ["Continuous"] = "Период.",
    ["Bond"] = "Связи",
    ["Fellows"] = "Связи",
    ["Rd. 1 talent not enabled"] = "Не активно",
    ["talent not enabled"] = "Не активно",
    ["Beyonder talent not enabled"] = "Не активно",
    ["Imagination Stance"] = "Стойка\nфантазии",
    ["Nightmare Stance"] = "Стойка\nкошмара",
    ["In Offensive Stance"] = "В атакующей\nстойке",
    ["Offensive Stance"] = "Атакующая\nстойка",
    ["In Defense Form"] = "В защитной\nстойке",
    ["Defense Form"] = "Защитная\nстойка",
    ["Output"] = "Урон",
    ["Healing"] = "Исцеление",
    ["Puppets"] = "Куклы",
    ["Puppet"] = "Куклы",
    ["Passive"] = "Пассив.",
    ["Normal"] = "Обычный",
    ["Roleplay"] = "Роль",
    ["Special"] = "Особый",
    ["Displacement"] = "Рывок",
    ["Survival"] = "Защита",
    ["Control Break"] = "Снятие контр.",
    ["Hard Crowd Control"] = "Жёстк. контр.",
    ["Soft Crowd Control"] = "Мягк. контр.",
    ["Group"] = "Группа",
    ["One-click Assist"] = "Помощник",
    -- Главный экран, логин и сервер
    ["Start Game"] = "Начать игру",
    ["Start"] = "Начать",
    ["Enter Game"] = "Войти в игру",
    ["Tap to Start"] = "Нажмите для входа",
    ["Tap Anywhere to Start"] = "Нажмите в любом месте для входа",
    ["Click Anywhere to Start"] = "Нажмите в любом месте для входа",
    ["Click blank area to close"] = "Нажмите в любом месте, чтобы закрыть",
    ["Click anywhere to skip"] = "Нажмите в любом месте, чтобы пропустить",
    ["Select Server"] = "Выбрать сервер",
    ["Server List"] = "Список серверов",
    ["Current Server"] = "Текущий сервер",
    ["Recommend"] = "Рекомендуемый",
    ["Recommended"] = "Рекомендуемый",
    ["Maintain"] = "Техработы",
    ["Maintenance"] = "Техработы",
    ["Smooth"] = "Стабильно",
    ["Crowded"] = "Загружен",
    ["Full"] = "Заполнен",
    ["Login"] = "Войти",
    ["Logout"] = "Выйти",
    ["User Agreement"] = "Пользовательское соглашение",
    ["Privacy Policy"] = "Политика конфиденциальности",
    ["Version"] = "Версия",

    -- Стандартные кнопки и действия
    ["Confirm"] = "Подтвердить",
    ["Cancel"] = "Отмена",
    ["OK"] = "ОК",
    ["Yes"] = "Да",
    ["No"] = "Нет",
    ["Back"] = "Назад",
    ["Return"] = "Назад",
    ["Close"] = "Закрыть",
    ["Exit"] = "Выход",
    ["Quit"] = "Выйти",
    ["Quit Game"] = "Выйти из игры",
    ["Settings"] = "Настройки",
    ["Setting"] = "Настройки",
    ["Save"] = "Сохранить",
    ["Delete"] = "Удалить",
    ["Apply"] = "Применить",
    ["Apply Build"] = "Применить сборку",
    ["Claim"] = "Забрать",
    ["Claim All"] = "Забрать всё",
    ["Claimed"] = "Получено",
    ["Claimed Today"] = "Получено сегодня",
    ["Reward"] = "Награда",
    ["Rewards"] = "Награды",
    ["Reward Preview"] = "Предпросмотр наград",
    ["Receive"] = "Получить",
    ["Received"] = "Получено",
    ["Completed"] = "Завершено",
    ["Complete"] = "Завершить",
    ["In Progress"] = "В процессе",
    ["Go"] = "Перейти",
    ["Use"] = "Использовать",
    ["In Use"] = "Используется",
    ["Buy"] = "Купить",
    ["Sell"] = "Продать",
    ["Shop"] = "Магазин",
    ["Store"] = "Магазин",
    ["Price"] = "Цена",
    ["Total"] = "Всего",
    ["Quantity"] = "Количество",
    ["Amount"] = "Количество",
    ["Skip"] = "Пропустить",
    ["Review"] = "Обзор",
    ["Screenshot"] = "Скриншот",
    ["Auto"] = "Авто",
    ["Details"] = "Подробнее",
    ["Detail"] = "Подробнее",
    ["Level"] = "Уровень",
    ["Notice"] = "Новости",
    ["Announcement"] = "Объявление",
    ["Loading..."] = "Загрузка...",
    ["Connecting..."] = "Подключение...",
    ["Congratulations"] = "Поздравляем",
    ["Convert"] = "Преобразовать",
    ["Not Owned"] = "Не получено",
    ["Owned"] = "Получено",
    ["Target Score"] = "Целевой счет",
    ["Official Recommended Build"] = "Официальная сборка",
    ["Recommended Builds"] = "Рекомендованные сборки",
    ["My Builds"] = "Мои сборки",
    ["Improve"] = "Усиление",
    ["Easy Wins"] = "Легкие победы",
    ["Deduction Check"] = "Проверка дедукции",
    ["Comments"] = "Комментарии",
    ["One-Click Assist"] = "Помощник",
    ["Auto-Dismantle Confirmation"] = "Подтверждение авто-распыления",
    ["Appearance"] = "Внешний вид",

    -- Развитие последовательностей и Древо Путей
    ["Pathway Conversion"] = "Смена пути",
    ["Pathway of God"] = "Путь Бога",
    ["God's Pathway"] = "Путь Бога",
    ["Potion Formula"] = "Рецепты",
    ["Clown"] = "Клоун",
    ["Seer"] = "Провидец",
    ["Magician"] = "Фокусник",
    ["Faceless"] = "Безликий",
    ["Marionettist"] = "Марионеточник",
    ["Bizarro Sorcerer"] = "Маг Непостижимого",
    ["Scholar of Yore"] = "Учёный Прошлого",
    ["Miracle Invoker"] = "Творец Чудес",
    ["Attendant of Mysteries"] = "Служитель Тайн",

    -- Персонаж, создание и выбор личности
    ["Character"] = "Персонаж",
    ["Role"] = "Персонаж",
    ["Create Character"] = "Создать персонажа",
    ["Select Character"] = "Выбрать персонажа",
    ["Create Role"] = "Создать персонажа",
    ["Select Role"] = "Выбрать персонажа",
    ["Male"] = "Мужской",
    ["Female"] = "Женский",
    ["Name"] = "Имя",
    ["Input Name"] = "Введите имя",
    ["Random Name"] = "Случайное имя",
    ["Madness"] = "Безумие",
    ["Sanity"] = "Рассудок",
    ["Wisdom"] = "Мудрость",
    ["Power"] = "Сила",
    ["Glory"] = "Слава",
    ["Emotion"] = "Чувства",

    -- Меню и вкладки
    ["Bag"] = "Инвентарь",
    ["Inventory"] = "Инвентарь",
    ["Gear"] = "Снаряжение",
    ["Equipment"] = "Снаряжение",
    ["Equip"] = "Экипировать",
    ["Unequip"] = "Снять",
    ["Detach"] = "Снять",
    ["Dungeon"] = "Подземелье",
    ["Dungeons"] = "Подземелья",
    ["Arena"] = "Арена",
    ["PVP"] = "Арена",
    ["Mail"] = "Почта",
    ["Email"] = "Почта",
    ["Ranking"] = "Рейтинг",
    ["Rank"] = "Рейтинг",
    ["Leaderboard"] = "Таблица лидеров",
    ["Skills"] = "Навыки",
    ["Skill"] = "Навыки",
    ["Normal Skill"] = "Обычные навыки",
    ["Special Skill (No Equipment Required)"] = "Особые навыки (без экипировки)",
    ["Roleplay Skill"] = "Навыки роли",
    ["One-Click Upgrade"] = "Прокачать",
    ["One-click Upgrade"] = "Прокачать",
    ["One-click upgrade"] = "Прокачать",
    ["Equip Skill"] = "Экипировать",
    ["Next-Level Effect"] = "Эффект след. уровня",
    ["Simple"] = "Кратко",
    ["Connections"] = "Связи",
    ["marionette"] = "Куклы",
    ["Training Dummy"] = "Манекен",
    ["Single Target"] = "Одиноч.",
    ["Area Target"] = "Область",
    ["Area of Effect"] = "По области",
    ["AOE"] = "По области",
    ["Super Armor"] = "Суперброня",
    ["Strengthening"] = "Усиление",
    ["Receive Blue Card"] = "Синяя карта",
    ["Receive Yellow Card"] = "Жёлтая карта",
    ["Self"] = "На себя",
    ["Card Energy"] = "Энергия карт",
    ["10Second"] = "10 сек.",
    ["25Second"] = "25 сек.",
    ["Praise the Fool, increasing the user's damage for 10 seconds, and obtain a Spirituality Blue Card and one point of Card Energy (used to unlock Finisher Skills).\n\nThe Fool that doesn't belong to this era; the mysterious ruler above the gray fog; the King of Yellow and Black who wields good luck. Praise the Fool!"] = "Восславьте Шута: увеличивает урон персонажа на 10 сек., дарует Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).\n\nНе принадлежащий этой эпохе Шут; таинственный правитель над серым туманом; Владыка Жёлтого и Чёрного, повелевающий удачей. Восславь Шута!",
    ["Praise the Fool, increasing the user's damage for 10 seconds, and obtain a Spirituality Blue Card and one point of Card Energy (used to unlock Finisher Skills).\n\nThe Fool that doesn't belong to this era; the mysterious ruler above the gray fog; the King of Yellow and Black who wields good luck. Praise the Fool!\n"] = "Восславьте Шута: увеличивает урон персонажа на 10 сек., дарует Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).\n\nНе принадлежащий этой эпохе Шут; таинственный правитель над серым туманом; Владыка Жёлтого и Чёрного, повелевающий удачей. Восславь Шута!",
    ["Praise the Fool, increasing the user's damage for 10 seconds, and obtain a Spirituality Blue Card and one point of Card Energy (used to unlock Finisher Skills)."] = "Восславьте Шута: увеличивает урон персонажа на 10 сек., дарует Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).",
    ["The Fool that doesn't belong to this era; the mysterious ruler above the gray fog; the King of Yellow and Black who wields good luck. Praise the Fool!"] = "Не принадлежащий этой эпохе Шут; таинственный правитель над серым туманом; Владыка Жёлтого и Чёрного, повелевающий удачей. Восславь Шута!",
    ["Increase your Physical Damage Boost by *f** and Pierce by *d points."] = "Повышает ваш физ. урон на *f** и пробивание на *d ед.",
    ["提高自身*f**的物理增伤和*d点穿刺"] = "Повышает ваш физ. урон на *f** и пробивание на *d ед.",
    ["buffdisc(*id), lasts for *f seconds, simultaneously gaining 1 point of <HighLight>Card Energy</> and one <HighLight>Spirituality Blue Card</>. \n\n<FaintYellow>Fate Yellow Card</>/<FaintYellow>Spirituality Blue Card</>: When 3 <HighLight>Fate Yellow Cards</>/<HighLight>Spirituality Blue Cards</> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</> to switch to <HighLight>Fooling of Fate</>/<HighLight>Spirituality Burst</>. \n<FaintYellow>Card Energy</>: Can hold up to 5 points; when reaching 5 points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>."] = "buffdisc(*id), длится *f сек., одновременно даруя 1 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>. \n\n<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе 3 <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>. \n<FaintYellow>Энергия карт</>: Вмещает до 5 очков; при достижении 5 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.",
    ["buffdisc(*id), lasts for *f seconds, simultaneously gaining 1 point of <HighLight>Card Energy</HighLight> and one <HighLight>Spirituality Blue Card</HighLight>. \n\n<FaintYellow>Fate Yellow Card</FaintYellow>/<FaintYellow>Spirituality Blue Card</FaintYellow>: When 3 <HighLight>Fate Yellow Cards</HighLight>/<HighLight>Spirituality Blue Cards</HighLight> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</HighLight> to switch to <HighLight>Fooling of Fate</HighLight>/<HighLight>Spirituality Burst</HighLight>. \n<FaintYellow>Card Energy</FaintYellow>: Can hold up to 5 points; when reaching 5 points, the <HighLight>Finisher Skill</HighLight> switches and locks to <HighLight>Shuffle Cards</HighLight>."] = "buffdisc(*id), длится *f сек., одновременно даруя 1 очко <HighLight>Энергии карт</HighLight> и одну <HighLight>Синюю карту духовности</HighLight>. \n\n<FaintYellow>Жёлтая карта судьбы</FaintYellow>/<FaintYellow>Синяя карта духовности</FaintYellow>: При сборе 3 <HighLight>Жёлтых карт судьбы</HighLight>/<HighLight>Синих карт духовности</HighLight> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>. \n<FaintYellow>Энергия карт</>: Вмещает до 5 очков; при достижении 5 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.",
    ["buffdisc(*id), lasts for *f seconds, simultaneously gaining 1 point of Card Energy and one Spirituality Blue Card. \n\nFate Yellow Card/Spirituality Blue Card: When 3 Fate Yellow Cards/Spirituality Blue Cards are obtained, consume all cards to cause the Finisher Skill Shuffle Cards to switch to Fooling of Fate/Spirituality Burst. \nCard Energy: Can hold up to 5 points; when reaching 5 points, the Finisher Skill switches and locks to Shuffle Cards."] = "buffdisc(*id), длится *f сек., одновременно даруя 1 очко Энергии карт и одну Синюю карту духовности. \n\nЖёлтая карта судьбы / Синяя карта духовности: При сборе 3 Жёлтых карт судьбы / Синих карт духовности они расходуются, переключая Добивание: Тасование карт на Одурачивание судьбы / Всплеск духовности. \nЭнергия карт: Вмещает до 5 очков; при достижении 5 очков Добивающий навык переключается и фиксируется на Тасование карт.",
    ["buffdisc(*id)，持续*f秒，同时获得一点<HighLight>卡牌能量</>和一张<HighLight>灵性蓝牌</>。\n\n<FaintYellow>命运黄牌</>/<FaintYellow>灵性蓝牌</>：获得三张<HighLight>命运黄牌</>/<HighLight>灵性蓝牌</>时消耗所有卡牌，使<HighLight>终结技能洗牌</>切换至<HighLight>命运愚弄</>/<HighLight>灵性爆发</>。\n<FaintYellow>卡牌能量</>：最多持有5点，达到5点时<HighLight>终结技能</>切换并锁定为<HighLight>洗牌</>。"] = "buffdisc(*id), длится *f сек., одновременно даруя 1 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>. \n\n<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе 3 <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>. \n<FaintYellow>Энергия карт</>: Вмещает до 5 очков; при достижении 5 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.",
    ["Mr. Fool has grafted a destiny from the future for you, allowing you to use powers of higher Sequences. As you grow stronger, the variety and power of the skills you learn will also increase. Skills are divided into three categories: Normal Skills, Special Skills, and Roleplay Skills. You can equip up to four Normal Skills or Roleplay Skills at once. Special Skills do not need to be equipped and include Basic Attacks, Control Break Skills, and Finisher Skills."] = "Мистер Шут привил вам судьбу из будущего, позволив использовать силу высших Последовательностей. По мере того как вы становитесь сильнее, разнообразие и мощь осваиваемых навыков будут расти. Навыки делятся на три категории: Обычные, Особые и Навыки роли. Можно экипировать до четырёх обычных навыков или навыков роли одновременно. Особые навыки не требуют экипировки и включают базовые атаки, снятие контроля и добивающие навыки.",
    ["Use Fate Yellow Card to enhance the Seer's ability to deal damage via Normal Attacks"] = "Используйте Жёлтые карты судьбы, чтобы усилить урон Провидца от обычных атак.",
    ["Use Spirituality Blue Card to enhance the Seer's ability to deal damage via Finisher Skills"] = "Используйте Синие карты духовности, чтобы усилить урон Провидца от добивающих навыков.",
    ["Maintain movement and continuously fire multiple Air Bullets at enemies; the user gains a Spirituality Blue Card and one point of Card Energy (used to unlock Finisher Skills)."] = "Позволяет двигаться и непрерывно выпускать множество Воздушных пуль во врагов; дает Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).",
    ["Launch playing cards to attack the target. After this skill deals damage 4 times, obtain an identical card based on the type of the previous card obtained (used to unlock Finisher Skills)."] = "Бросает игральные карты для атаки цели. После нанесения урона 4 раза дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков).",
    ["Borrow power from history, immediately filling the user's Card Energy, and obtain cards based on the cards currently held (used to unlock Finisher Skills)."] = "Заимствует силу у истории, мгновенно заполняя Энергию карт заклинателя и даруя карты в зависимости от имеющихся (необходимо для открытия добивающих навыков).",
    ["Throw a Tarot card at the enemy, unfolding a Tarot Array to continuously deal damage, and knock up monster targets upon finishing. The user obtains an identical card based on the type of the previous card obtained (used to unlock Finisher Skills) and the corresponding additional effect."] = "Бросает карту Таро во врага, разворачивая Таро-расклад для непрерывного нанесения урона, и подбрасывает монстров при завершении. Дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков), а также соответствующий дополнительный эффект.",
    ["Miraculously summon a rain of cards at the target location, dealing damage, Slow, and Stun to enemies, and additional Healing Reduction to players. The user obtains an identical card based on the type of the previous card obtained (used to unlock Finisher Skills) and the corresponding additional effect."] = "Чудесным образом обрушивает дождь карт в указанную область, нанося урон, замедление и оглушение врагам, а также доп. снижение исцеления игрокам. Дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков), а также соответствующий дополнительный эффект.",
    ["Fool Resource (Card Energy)"] = "Ресурс Шута (Энергия карт)",
    ["Manipulate fate and spirituality, manifesting them in the form of cards to fool enemies. Collect Fate Yellow Cards and Spirituality Blue Cards to unlock the finisher skills Fooling of Fate and Spirituality Burst, and collect card energy to unlock the finisher skill Shuffle Cards."] = "Манипулируйте судьбой и духовностью, воплощая их в форме карт для одурачивания врагов. Собирайте Жёлтые карты судьбы и Синие карты духовности, чтобы открывать добивающие навыки Одурачивание судьбы и Всплеск духовности, а также накапливайте Энергию карт для открытия Тасования карт.",
    ["Collect Card Energy to unlock the skill. Inject all collected Card Energy into a large number of cards, continuously pouring energy-filled cards at enemies to deal high damage. Other skills can be released and movement is possible during the casting process."] = "Соберите Энергию карт, чтобы открыть навык. Вливает всю собранную Энергию карт в множество карт, непрерывно осыпая врагов заряженными картами для нанесения огромного урона. Во время применения можно использовать другие навыки и двигаться.",
    ["Collect Fate Yellow Cards to unlock the skill. Fool enemies within range, dealing massive damage. Afterward, for a period of time, the user's normal attacks will apply a Fooling Mark that deals additional damage, and attack speed is increased. Using it grants one point of Card Energy."] = "Соберите Жёлтые карты судьбы, чтобы открыть навык. Одурачивает врагов в области, нанося огромный урон. Затем в течение некоторого времени ваши обычные атаки будут накладывать Метку одурачивания, наносящую дополнительный урон, а скорость атаки повысится. Использование дает 1 очко Энергии карт.",
    ["Collect Spirituality Blue Cards to unlock the skill. Release the user's spiritual power, dealing massive damage to enemies within range and applying Vulnerability. Using it grants one point of Card Energy."] = "Соберите Синие карты духовности, чтобы открыть навык. Высвобождает духовную силу заклинателя, нанося огромный урон врагам в области и накладывая Уязвимость. Использование дает 1 очко Энергии карт.",
    ["Using Flame Jump also grants 3 fate cards and one point of Card Energy."] = "Использование Прыжка пламени также дает 3 карты судьбы и 1 очко Энергии карт.",
    ["Obtaining a fate card also grants one point of Card Energy."] = "Получение карты судьбы также дает 1 очко Энергии карт.",
    ["Praise the Fool, increasing the user's damage for 10 seconds, and obtain a Spirituality Blue Card and one point of Card Energy (used to unlock Finisher Skills)."] = "Восславьте Шута: увеличивает урон заклинателя на 10 сек., дарует одну Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).",
    ["Air Bullet"] = "Воздушная\nпуля",
    ["Air Bullets"] = "Воздушные\nпули",
    ["Tarot Array"] = "Таро-\nрасклад",
    ["Miracle Card Rain"] = "Дождь\nкарт чудес",
    ["Fool's Blessing"] = "Благословение\nШута",
    ["Card Flying Dagger"] = "Карточный\nкинжал",
    ["Flame Jump"] = "Прыжок\nпламени",
    ["Card Master"] = "Мастер\nкарт",
    ["Realm of Mysteries"] = "Царство\nТайн",
    ["Reveal Card"] = "Раскрытие\nкарт",
    ["Paper Figurine Substitute"] = "Бумажный\nзаменитель",
    ["Fooling of Fate"] = "Одурачивание\nсудьбы",
    ["Spirituality Burst"] = "Всплеск\nдуховности",
    ["Shuffle Cards"] = "Тасование\nкарт",
    ["Cut Cards"] = "Снятие\nкарт",
    ["Open Door"] = "Открытие\nдвери",
    ["Secret Words"] = "Тайные\nслова",
    ["Pendulum Divination"] = "Гадание на\nмаятнике",
    ["Earthquake Slam"] = "Сотрясение\nземли",
    ["Praise the Sun"] = "Восславь\nСолнце",
    ["Emotional Spectrum"] = "Эмоциональный\nспектр",
    ["Poised to Strike"] = "Готовность\nк удару",
    ["Listen to Heart's Voice"] = "Глас\nсердца",
    ["Smiling Clown"] = "Улыбающийся\nклоун",
    ["War Soul Afterimage"] = "Остаточный\nобраз",
    ["Illusion Trick"] = "Иллюзорный\nфокус",
    ["Holy Light Favor"] = "Святой\nСвет",
    ["Astrological Revelation"] = "Откровение\nзвёзд",
    ["Mental Comfort"] = "Утешение\nразума",
    ["Talent"] = "Таланты",
    ["Talents"] = "Таланты",
    ["Path"] = "Путь",
    ["Pathway"] = "Путь",
    ["Promotion"] = "Путь",
    ["Sequence"] = "Последовательность",
    ["Sealed"] = "Реликвии",
    ["Sealed Artifact"] = "Запечатанный артефакт",
    ["Sealed Artifacts"] = "Запечатанные артефакты",
    ["Secret Partner"] = "Куклы",
    ["SecretPartner"] = "Куклы",
    ["Marionette"] = "Куклы",
    ["Marionettes"] = "Куклы",
    ["Fellow"] = "Связи",
    ["Ally"] = "Союзник",
    ["Allies"] = "Союзники",
    ["Club"] = "Клуб",
    ["Guild"] = "Клуб",
    ["Task"] = "Задания",
    ["Tasks"] = "Задания",
    ["Quest"] = "Задание",
    ["Quests"] = "Задания",
    ["Mission"] = "Миссия",
    ["Style"] = "Гардероб",
    ["Fashion"] = "Гардероб",
    ["Explore"] = "Исследование",
    ["Pastime"] = "Исследование",
    ["Home"] = "Поместье",
    ["Manor"] = "Поместье",
    ["Family"] = "Семья",
    ["Party"] = "Группа",
    ["Team"] = "Команда",
    ["Friend"] = "Друзья",
    ["Friends"] = "Друзья",
    ["Chat"] = "Чат",
    ["World"] = "Мир",
    ["Nearby"] = "Рядом",
    ["System"] = "Система",
    ["Help"] = "Помощь",
    ["Guide"] = "Гайды",
    ["Strategy"] = "Гайды",
    ["News"] = "Новости",
    ["Notice"] = "Новости",
    ["Achievement"] = "Достижения",
    ["Achievements"] = "Достижения",

    -- Характеристики и атрибуты
    ["Attributes"] = "Атрибуты",
    ["Attribute"] = "Атрибут",
    ["Stats"] = "Характеристики",
    ["Basic Attributes"] = "Базовые атрибуты",
    ["Advanced Attributes"] = "Дополнительные атрибуты",
    ["HP"] = "ОЗ",
    ["Max HP"] = "Макс. ОЗ",
    ["MP"] = "ОМ",
    ["Max MP"] = "Макс. ОМ",
    ["Attack"] = "Атака",
    ["Physical Attack"] = "Физ. атака",
    ["Magical Attack"] = "Маг. атака",
    ["Defense"] = "Защита",
    ["Physical Defense"] = "Физ. защита",
    ["Magical Defense"] = "Маг. защита",
    ["Crit"] = "Крит",
    ["Crit Rate"] = "Шанс крита",
    ["Crit Damage"] = "Крит. урон",
    ["Speed"] = "Скорость",
    ["Move Speed"] = "Скорость бега",
    ["Attack Speed"] = "Скорость атаки",
    ["Cast Speed"] = "Скорость применения",
    ["Block"] = "Блок",
    ["Block Rate"] = "Шанс блока",
    ["Penetration"] = "Пробивание",
    ["Accuracy"] = "Меткость",
    ["Dodge"] = "Уклонение",
    ["Damage"] = "Урон",
    ["Mobility"] = "Подвижность",
    ["Range"] = "Дальность",
    ["Survivability"] = "Живучесть",
    ["Difficulty"] = "Сложность",
    ["Control"] = "Контроль",
    ["Burst"] = "Взрыв",
    ["Support"] = "Поддержка",

    -- Канон Повелителя Тайн (English -> Russian)
    ["Lord of Mysteries"] = "Повелитель Тайн",
    ["Lord of the Mysteries"] = "Повелитель Тайн",
    ["The Fool"] = "Шут",
    ["\"The Fool\""] = "«Шут»",
    ["Door"] = "Дверь",
    ["Error"] = "Ошибка",
    ["Visionary"] = "Визионер",
    ["Hanged Man"] = "Повешенный",
    ["Sun"] = "Солнце",
    ["Tyrant"] = "Тиран",
    ["White Tower"] = "Белая Башня",
    ["Darkness"] = "Тьма",
    ["Death"] = "Смерть",
    ["Twilight Giant"] = "Гигант Сумерек",
    ["Red Priest"] = "Красный Жрец",
    ["Demoness"] = "Демонесса",
    ["Black Emperor"] = "Чёрный Император",
    ["Justiciar"] = "Арбитр",
    ["Wheel of Fortune"] = "Колесо Фортуны",
    ["Moon"] = "Луна",
    ["Mother"] = "Мать",
    ["Chained"] = "Скованный",
    ["Abyss"] = "Бездна",
    ["Paragon"] = "Образцовый",
    ["Hermit"] = "Отшельник",
    ["Seer"] = "Провидец",
    ["Clown"] = "Клоун",
    ["Magician"] = "Фокусник",
    ["Faceless"] = "Безликий",
    ["Marionettist"] = "Марионеточник",
    ["Bizarro Sorcerer"] = "Ловкий Маг",
    ["Scholar of Yore"] = "Учёный Прошлого",
    ["Miracle Invoker"] = "Творец Чудес",
    ["Attendant of Mysteries"] = "Слуга Тайн",
    ["Apprentice"] = "Ученик",
    ["Trickmaster"] = "Мастер Уловок",
    ["Astrologer"] = "Астролог",
    ["Traveler"] = "Путешественник",
    ["Scribe"] = "Летописец",
    ["Marauder"] = "Мародёр",
    ["Swindler"] = "Мошенник",
    ["Cryptologist"] = "Криптолог",
    ["Spectator"] = "Зритель",
    ["Telepathist"] = "Телепат",
    ["Psyche Analyst"] = "Психиатр",
    ["Hypnotist"] = "Гипнотизёр",
    ["Dreamwalker"] = "Сноходец",
    ["Manipulator"] = "Манипулятор",
    ["Warrior"] = "Воин",
    ["Hunter"] = "Охотник",
    ["Provoker"] = "Провокатор",
    ["Pyromaniac"] = "Пироман",
    ["Reaper"] = "Жнец",
    ["Assassin"] = "Ассасин",
    ["Instigator"] = "Подстрекатель",
    ["Witch"] = "Ведьма",
    ["Lawyer"] = "Адвокат",
    ["Barbarian"] = "Варвар",
    ["Arbiter"] = "Арбитр",
    ["Sheriff"] = "Шериф",
    ["Interrogator"] = "Дознаватель",
    ["Judge"] = "Судья",
    ["Monster"] = "Монстр",
    ["Lucky One"] = "Счастливчик",
    ["Corpse Collector"] = "Сборщик Трупов",
    ["Gravedigger"] = "Могильщик",
    ["Spirit Guide"] = "Проводник Духов",
    ["Sleepless"] = "Бессонный",
    ["Midnight Poet"] = "Полуночный Поэт",
    ["Nightmare"] = "Кошмар",
    ["Sailor"] = "Моряк",
    ["Folk of Rage"] = "Гневный",
    ["Seafarer"] = "Мореплаватель",
    ["Reader"] = "Чтец",
    ["Mystery Pryer"] = "Тайновидец",
    ["Apothecary"] = "Аптекарь",
    ["Beast Tamer"] = "Укротитель Зверей",
    ["Vampire"] = "Вампир",
    ["Planter"] = "Садовник",
    ["Doctor"] = "Врач",
    ["Harvest Priest"] = "Жрец Урожая",
    ["Prisoner"] = "Узник",
    ["Lunatic"] = "Безумец",
    ["Werewolf"] = "Оборотень",
    ["Zombie"] = "Зомби",
    ["Wraith"] = "Призрак",
    ["Puppeteer"] = "Кукловод",
    ["Criminal"] = "Преступник",
    ["Devil"] = "Дьявол",
    ["Beyonder"] = "Потусторонний",
    ["Beyonders"] = "Потусторонние",
    ["Beyonder Rating"] = "Рейтинг Потустороннего",
    ["Recommended Beyonder Rating"] = "Рек. рейтинг Потустороннего",
    ["Spirit Body"] = "Духовное тело",
    ["Spirit Body Threads"] = "Нити духовного тела",
    ["Potion"] = "Зелье",
    ["Potions"] = "Зелья",
    ["Tingen"] = "Тинген",
    ["Backlund"] = "Бэкланд",
    ["Tarot Club"] = "Клуб Таро",
    ["Nighthawks"] = "Ночные Ястребы",
    ["Klein Moretti"] = "Клейн Моретти",
    ["Leonard Mitchell"] = "Леонард Митчелл",
    ["Audrey Hall"] = "Одри Холл",
    ["Alger Wilson"] = "Элджер Уилсон",
    ["Derrick Berg"] = "Деррик Берг",
    ["Fors Wall"] = "Форс Уолл",
    ["Xio Derecha"] = "Сио Дереча",
    ["Dunn Smith"] = "Данн Смит",
    ["Frye"] = "Фрай",
    ["Rozanne"] = "Розанна",
    ["Old Neil"] = "Старина Нил",
    ["Amon"] = "Амон",
    ["Adam"] = "Адам",
    ["Antigonus"] = "Антигон",
    ["Zaratul"] = "Заратул",
    ["Sebastian"] = "Себастьян",
    ["Man"] = "Мужчина",
    ["Ugly Man"] = "Уродец",
    ["Dawn Arrival"] = "Пришествие\nрассвета",
    ["Arbitration Brand"] = "Клеймо\nарбитража",
    ["Mystery Pry Gaze"] = "Взор тайновидца",
    ["Morning Light Protection"] = "Защита утреннего света",
    ["Knight's Oath"] = "Клятва рыцаря",
    ["Butterfly Spirit Possession"] = "Дух\nбабочки",
    ["Death Knell Echo"] = "Погребальный\nЗвон",
    ["Alpha Wolf Claw Combo"] = "Серия когтей вожака",
    ["Drill Protection"] = "Защита бура",
    -- Вкладки инвентаря
    ["Item"] = "Все",
    ["Items"] = "Все",
    ["All"] = "Все",
    ["Beyonder material"] = "Ресурсы",
    ["Beyonder materials"] = "Ресурсы",
    ["Beyonder Material"] = "Ресурсы",
    ["Beyonder Materials"] = "Ресурсы",
    ["mystical item"] = "Артефакты",
    ["Mystical item"] = "Артефакты",
    ["Mystical Item"] = "Артефакты",
    ["Mystical Items"] = "Артефакты",
    ["Castle"] = "Замок",
    ["Home"] = "Замок",
    ["Task"] = "Квесты",
    ["Quest"] = "Квесты",
    ["Quests"] = "Квесты",
    ["Gear"] = "Эквип",
    ["Equip"] = "Эквип",
    ["Equipment"] = "Эквип",
    ["Potion"] = "Зелья",
    ["Potions"] = "Зелья",
}

local okEng, EnglishMod = pcall(require, "mods.cpdd_runtime_fixes.EnglishToRussian")
if okEng and type(EnglishMod) == "table" and type(EnglishMod.exact) == "table" then
    for k, v in pairs(EnglishMod.exact) do
        if Russian.englishToRussian[k] == nil then
            Russian.englishToRussian[k] = v
        end
    end
end

-- Китайский -> Русский (точные переопределения интерфейса и текста)
Russian.chineseToRussian = {
    ["途径转换"] = "Смена пути",
    ["神之途径"] = "Путь Бога",
    ["魔药配方"] = "Рецепты",
    ["小丑"] = "Клоун",
    ["占卜家"] = "Провидец",
    ["魔术师"] = "Фокусник",
    ["无面人"] = "Безликий",
    ["秘偶大师"] = "Марионеточник",
    ["诡法师"] = "Маг Непостижимого",
    ["古代学者"] = "Учёный Прошлого",
    ["奇迹师"] = "Творец Чудес",
    ["诡秘侍者"] = "Служитель Тайн",
    ["已达到等级上限"] = "Макс. ур.",
    ["已达成等级上限"] = "Макс. ур.",
    ["(已达到等级上限)"] = "(Макс. ур.)",
    ["(已达成等级上限)"] = "(Макс. ур.)",
    ["角色编号:"] = "ID:",
    ["角色编号：%s"] = "ID: %s",
    ["角色编号: %s"] = "ID: %s",
    ["角色编号"] = "ID",
    ["占位"] = "Инфо",
    ["诗意光环"] = "Поэтическая\nаура",
    ["宁静光环"] = "Аура\nпокоя",
    ["通灵仪式"] = "Спиритический\nритуал",
    ["历史孔隙"] = "Пустота\nистории",
    ["操纵秘偶"] = "Управление\nкуклами",
    ["卡牌风暴"] = "Дождь\nкарт",
    ["塔罗法阵"] = "Таро-\nрасклад",
    ["飞牌"] = "Бросок\nкарт",
    ["空气飞弹"] = "Воздушная\nракета",
    ["命运黄牌"] = "Жёлтая карта\nсудьбы",
    ["灵性蓝牌"] = "Синяя карта\nдуховности",
    ["终结技-洗牌"] = "Тасование\nкарт",
    ["圣域庇佑"] = "Защита\nСвятилища",
    ["潜意识海"] = "Море\nподсознания",
    ["威慑"] = "Устра-\nшение",
    ["心理暗示"] = "Психологич.\nвнушение",
    ["死灵印记"] = "Метка\nнекроманта",
    ["神奇蘑菇"] = "Волшебный\nгриб",
    ["驭浪"] = "Покорение\nволн",
    ["律令裁决"] = "Приговор\nзакона",
    ["戏法表演"] = "Шоу\nфокусов",
    ["安魂诗篇"] = "Упокойная\nпоэма",
    ["合奏诗篇"] = "Созвучие\nстихов",
    ["午夜诗篇"] = "Полуночная\nпоэма",
    ["转运仪式"] = "Ритуал\nудачи",
    ["灰雾压制"] = "Подавление\nтумана",
    ["灰雾加持"] = "Благословение\nтумана",
    ["历史明灯"] = "Исторический\nмаяк",
    ["空气炮"] = "Воздушная\nпушка",
    ["心灵洞察"] = "Взор\nразума",
    ["窥秘仪式"] = "Ритуал\nтайн",
    ["剑术大师"] = "Мастер\nмеча",
    ["演出落幕"] = "Конец\nспектакля",
    ["虔诚祷祝"] = "Истовая\nмолитва",
    ["巫术"] = "Колдовство",
    ["神秘幻象"] = "Таинственная\nиллюзия",
    ["朗基努斯之枪"] = "Копьё\nЛонгина",
    ["信息洪流"] = "Поток\nзнаний",
    ["星沙"] = "Звёздный\nпесок",
    ["星云咏叹"] = "Ария\nтуманности",
    ["足迹"] = "Следы",
    ["足迹再现"] = "Следы",
    ["闪回"] = "Возврат",
    ["折幕"] = "Ширма\nпространств",
    ["空间牢笼"] = "Клетка\nпространства",
    ["星光囚笼"] = "Звёздная\nклетка",
    ["戏法演绎"] = "Шоу\nфокусов",
    ["层叠之门"] = "Врата\nпространств",
    ["圣焰"] = "Святое\nпламя",
    ["光之折射"] = "Преломление\nсвета",
    ["太阳誓约"] = "Солнечная\nклятва",
    ["惩戒"] = "Кара",
    ["神罚"] = "Божья\nкара",
    ["圣光庇护"] = "Защита\nСвета",
    ["逐日"] = "Ловец\nСолнца",
    ["无暗之枪"] = "Копьё\nСвета",
    ["烈阳之魂"] = "Душа\nСолнца",
    ["烈阳"] = "Яркое\nСолнце",
    ["黎明铠甲"] = "Доспех\nрассвета",
    ["战姿留痕"] = "Боевая\nстойка",
    ["荣耀之斩"] = "Удар\nславы",
    ["银白细剑"] = "Серебряная\nрапира",
    ["日暮飓风"] = "Закатный\nураган",
    ["王庭号令"] = "Приказ\nдвора",
    ["驱魔卫斩"] = "Удар\nэкзорцизма",
    ["猎魔瞬斩"] = "Быстрый\nвыпад",
    ["凋零之刃"] = "Клинок\nувядания",
    ["巨剑斩击"] = "Удар\nдвуручником",
    ["怒意猛击"] = "Яростный\nудар",
    ["噩梦"] = "Кошмар",
    ["瘟疫"] = "Чума",
    ["精神噩梦"] = "Ментальный\nкошмар",
    ["读心"] = "Чтение\nмыслей",
    ["心理治疗"] = "Психо-\nтерапия",
    ["安抚"] = "Умиро-\nтворение",
    ["梦境重生"] = "Перерож-\nдение",
    ["梦境复苏"] = "Возврат\nсна",
    ["心灵之火"] = "Пламя\nразума",
    ["心理学隐身"] = "Незри-\nмость",
    ["意识冲击"] = "Удар\nсознания",
    ["心灵引导"] = "Ментальное\nвнушение",
    ["震慑"] = "Устра-\nшение",
    ["精神瘟疫"] = "Ментальная\nчума",
    ["意识操纵"] = "Контроль\nсознания",
    ["梦境分析"] = "Анализ\nснов",
    ["梦境编织"] = "Плетение\nснов",
    ["狂乱"] = "Бешенство",
    ["洞悉"] = "Проница-\nтельность",
    ["洞察"] = "Прозре-\nние",
    ["被动"] = "Пассив.",
    ["普通"] = "Обычный",
    ["伤害"] = "Урон",
    ["控制"] = "Контроль",
    ["持续"] = "Период.",
    ["羁绊"] = "Связи",
    ["伙伴"] = "Связи",
    ["非凡天赋未启用"] = "Не активно",
    ["空想姿态"] = "Стойка\nфантазии",
    ["噩梦姿态"] = "Стойка\nкошмара",
    ["输出形态"] = "Атакующая\nстойка",
    ["防御形态"] = "Защитная\nстойка",
    ["输出"] = "Урон",
    ["治疗"] = "Исцеление",
    ["诡秘之境"] = "Царство\nТайн",
    ["扮演"] = "Роль",
    ["特殊"] = "Особый",
    ["位移"] = "Рывок",
    ["生存"] = "Защита",
    ["爆发"] = "Взрыв",
    ["范围"] = "Область",
    ["单体"] = "Одиноч.",
    ["解控"] = "Снятие контр.",
    ["硬控"] = "Жёстк. контр.",
    ["软控"] = "Мягк. контр.",
    ["群体"] = "Группа",
    ["群攻"] = "Группа",
    ["一键辅助"] = "Помощник",
    ["全部"] = "Все",
    ["道具"] = "Все",
    ["非凡材料"] = "Ресурсы",
    ["非凡物质"] = "Ресурсы",
    ["神奇物品"] = "Артефакты",
    ["封印物"] = "Артефакты",
    ["家园"] = "Замок",
    ["庄园"] = "Замок",
    ["装备"] = "Эквип",
    ["任务"] = "Квесты",
    ["攻略"] = "Гайды",
    ["魔药"] = "Зелья",
    ["机动"] = "Подвижность",
    ["射程"] = "Дальность",
    ["跳过"] = "Пропустить",
    ["回顾"] = "Обзор",
    ["截图"] = "Скриншот",
    ["点击空白区域关闭"] = "Нажмите в любом месте, чтобы закрыть",
    ["点击任意区域跳过"] = "Нажмите в любом месте, чтобы пропустить",
    ["恭喜获得"] = "Поздравляем",
    ["转化"] = "Преобразовать",
    ["男子"] = "Мужчина",
    ["丑人"] = "Уродец",
    ["愚者"] = "Шут",
    ["“愚者”"] = "«Шут»",
    ["塞巴斯蒂安"] = "Себастьян",
    ["寒巴斯蒂安"] = "Себастьян",
    ["非凡评分"] = "Рейтинг Потустороннего",
    ["推荐非凡评分"] = "Рек. рейтинг Потустороннего",
    ["奖励预览"] = "Предпросмотр наград",
    ["目标点数"] = "Целевой счет",
    ["未拥有"] = "Не получено",
    ["推荐方案"] = "Рекомендованные сборки",
    ["官方推荐方案"] = "Официальная сборка",
    ["我的方案"] = "Мои сборки",
    ["我要变强"] = "Усиление",
    ["进入游戏"] = "Войти в игру",
    ["开始游戏"] = "Начать игру",
    ["点击屏幕开始"] = "Нажмите для входа",
    ["选择服务器"] = "Выбрать сервер",
    ["确认"] = "Подтвердить",
    ["确定"] = "Подтвердить",
    ["取消"] = "Отмена",
    ["返回"] = "Назад",
    ["关闭"] = "Закрыть",
    ["设置"] = "Настройки",
    ["退出"] = "Выход",
    ["退出游戏"] = "Выйти из игры",
    ["背包"] = "Инвентарь",
    ["装备"] = "Снаряжение",
    ["技能"] = "Навыки",
    ["天赋"] = "Таланты",
    ["途径"] = "Путь",
    ["序列"] = "Последовательность",
    ["封印物"] = "Запечатанный артефакт",
    ["秘偶"] = "Куклы",
    ["副本"] = "Подземелье",
    ["竞技场"] = "Арена",
    ["邮件"] = "Почта",
    ["排行榜"] = "Рейтинг",
    ["任务"] = "Задания",
    ["公会"] = "Клуб",
    ["家园"] = "Поместье",
    ["好友"] = "Друзья",
    ["聊天"] = "Чат",
    ["世界"] = "Мир",
    ["附近"] = "Рядом",
    ["队伍"] = "Группа",
    ["升级"] = "Улучшить",
    ["强化"] = "Усилить",
    ["分解"] = "Распылить",
    ["合成"] = "Синтез",
    ["购买"] = "Купить",
    ["出售"] = "Продать",
    ["使用"] = "Использовать",
    ["领取"] = "Получить",
    ["已领取"] = "Получено",
    ["生命"] = "ОЗ",
    ["法力"] = "ОМ",
    ["攻击"] = "Атака",
    ["防御"] = "Защита",
    ["暴击"] = "Крит",
    ["速度"] = "Скорость",
    ["非凡"] = "Потусторонний",
    ["非凡者"] = "Потусторонний",
    ["魔药"] = "Зелье",
    ["廷根"] = "Тинген",
    ["贝克兰德"] = "Бэкланд",
    ["塔罗会"] = "Клуб Таро",
    ["值夜者"] = "Ночные Ястребы",
    ["克莱恩"] = "Клейн",
    ["伦纳德"] = "Леонард",
    ["奥黛丽"] = "Одри",
    ["阿尔杰"] = "Элджер",
    ["戴里克"] = "Деррик",
    ["佛尔思"] = "Форс",
    ["休"] = "Сио",
    ["邓恩"] = "Данн",
    ["弗莱"] = "Фрай",
    ["罗珊"] = "Розанна",
    ["老尼尔"] = "Старина Нил",
    ["阿蒙"] = "Амон",
    ["亚当"] = "Адам",
    ["安提哥努斯"] = "Антигон",
    ["查拉图"] = "Заратул",
    ["普通技能"] = "Обычные навыки",
    ["特殊技能（无需装配）"] = "Особые навыки (без экипировки)",
    ["特殊技能(无需装配)"] = "Особые навыки (без экипировки)",
    ["扮演技能"] = "Навыки роли",
    ["一键升级"] = "Прокачать",
    ["装配技能"] = "Экипировать",
    ["下级效果"] = "Эффект след. уровня",
    ["空气子弹"] = "Воздушная\nпуля",
    ["空气弹"] = "Воздушная\nпуля",
    ["纸牌飞刀"] = "Карточный\nкинжал",
    ["塔罗牌阵"] = "Таро-\nрасклад",
    ["奇迹牌雨"] = "Дождь\nкарт чудес",
    ["火焰跳跃"] = "Прыжок\nпламени",
    ["愚者祝福"] = "Благословение\nШута",
    ["卡牌大师"] = "Мастер\nкарт",
    ["诡秘领域"] = "Царство\nТайн",
    ["开牌"] = "Раскрытие\nкарт",
    ["纸人替身"] = "Бумажный\nзаменитель",
    ["命运愚弄"] = "Одурачивание\nсудьбы",
    ["灵性爆发"] = "Всплеск\nдуховности",
    ["洗牌"] = "Тасование\nкарт",
    ["切牌"] = "Снятие\nкарт",
    ["开门"] = "Открытие\nдвери",
    ["秘语"] = "Тайные\nслова",
    ["灵摆占卜"] = "Гадание на\nмаятнике",
    ["地震重击"] = "Сотрясение\nземли",
    ["赞美太阳"] = "Восславь\nСолнце",
    ["情绪光谱"] = "Эмоциональный\nспектр",
    ["蓄势待发"] = "Готовность\nк удару",
    ["倾听心声"] = "Глас\nсердца",
    ["微笑小丑"] = "Улыбающийся\nклоун",
    ["战魂残影"] = "Остаточный\nобраз",
    ["戏法假象"] = "Иллюзорный\nфокус",
    ["圣光眷顾"] = "Святой\nСвет",
    ["占星启示"] = "Откровение\nзвёзд",
    ["心灵抚慰"] = "Утешение\nразума",
}

-- Подмена фрагментов текста внутри фраз
Russian.visibleTextReplacements = {
    { "Click blank area to close", "Нажмите в любом месте, чтобы закрыть" },
    { "Click anywhere to skip", "Нажмите в любом месте, чтобы пропустить" },
    { "点击空白区域关闭", "Нажмите в любом месте, чтобы закрыть" },
    { "点击任意区域跳过", "Нажмите в любом месте, чтобы пропустить" },
    { "跳过", "Пропустить" },
    { "推荐非凡评分", "Рек. рейтинг Потустороннего" },
    { "非凡评分", "Рейтинг Потустороннего" },
    { "塞巴斯蒂安", "Себастьян" },
    { "寒巴斯蒂安", "Себастьян" },
    { "男子：", "Мужчина: " },
    { "丑人：", "Уродец: " },
    { "“愚者”：", "«Шут»: " },
    { "愚者", "Шут" },
    { "（癫狂）", "(В безумии) " },
    { "万物的“母亲”", "«Мать» всего сущего" },
    { "赐予我们新生", "даруй нам перерождение" },
    { "拿上这个", "Возьми это" },
    { "背包", "Инвентарь" },
    { "设置", "Настройки" },
    { "确定", "Подтвердить" },
    { "取消", "Отмена" },
}

function Russian.lookupRussianText(text)
    if not Russian.Enabled or type(text) ~= "string" or text == "" then
        return nil
    end

    -- 0. Компактные теги навыков (гарантированное совпадение без вылезания за экран)
    local trimmed = text:match("^%s*(.-)%s*$")
    if trimmed ~= nil and trimmed ~= "" then
        local tag = Russian.skillTags[trimmed]
        if tag ~= nil then return tag end

        local sec = trimmed:match("^(%d+)%s*[Ss]econds?$") or trimmed:match("^(%d+)%s*秒$") or trimmed:match("^(%d+)Second$")
        if sec ~= nil then
            return sec .. " сек."
        end
    end

    -- 1. Сначала проверяем точный перевод с английского (UI патча)
    local ru = Russian.englishToRussian[text]
    if ru ~= nil then return ru end
    if EnglishMod and type(EnglishMod.translate) == "function" then
        local ruEng = EnglishMod.translate(text)
        if ruEng ~= nil then return ruEng end
    end

    -- 2. Затем проверяем точный перевод с китайского
    ru = Russian.chineseToRussian[text]
    if ru ~= nil then return ru end

    -- 2.5. Точные оверрайды видимого текста
    if Russian.visibleTextExactOverrides then
        ru = Russian.visibleTextExactOverrides[text]
        if ru ~= nil then return ru end
    end

    -- 3. Быстрый поиск в шардированной базе перевода (1024 шарда RuntimeTextGemini_000..3ff)
    local lookupGemini = Russian.lookupGeminiText
    if lookupGemini then
        local found = lookupGemini(text)
        if found ~= nil then return found end
        if trimmed and trimmed ~= text and trimmed ~= "" then
            found = lookupGemini(trimmed)
            if found ~= nil then return found end
        end
    end

    -- 3.5. Поддержка составных многострочных описаний навыков (краткое описание + худ. текст через \n)
    if text:find("\n", 1, true) then
        local parts = {}
        local anyFound = false
        local startIdx = 1
        while startIdx <= #text do
            local nlStart, nlEnd = text:find("\r?\n+", startIdx)
            local chunk, sep
            if nlStart then
                chunk = text:sub(startIdx, nlStart - 1)
                sep = text:sub(nlStart, nlEnd)
                startIdx = nlEnd + 1
            else
                chunk = text:sub(startIdx)
                sep = ""
                startIdx = #text + 1
            end

            local trimmedChunk = chunk:match("^%s*(.-)%s*$")
            local trChunk = nil
            if trimmedChunk and trimmedChunk ~= "" then
                trChunk = Russian.englishToRussian[chunk] or Russian.chineseToRussian[chunk]
                    or (Russian.visibleTextExactOverrides and (Russian.visibleTextExactOverrides[chunk] or Russian.visibleTextExactOverrides[trimmedChunk]))
                    or Russian.englishToRussian[trimmedChunk] or Russian.chineseToRussian[trimmedChunk]
                if not trChunk and lookupGemini then
                    trChunk = lookupGemini(chunk) or lookupGemini(trimmedChunk)
                end
            end

            if trChunk then
                parts[#parts + 1] = trChunk .. sep
                anyFound = true
            else
                parts[#parts + 1] = chunk .. sep
            end
        end
        if anyFound then
            return table.concat(parts)
        end
    end

    -- 4. Автоматическая обработка тегов времени (%d+Second)
    local sec = text:match("^(%d+)Second$")
    if sec ~= nil then
        return sec .. " сек."
    end

    -- 5. Контекстный перевод динамических описаний навыков и подсказок
    if #text > 8 then
        local m = text

        -- ----------------------------------------------------
        -- УНИВЕРСАЛЬНЫЙ ПЕРЕКЛЮЧАТЕЛЬ СТОЕК ДЛЯ ВСЕХ НАВЫКОВ
        -- ----------------------------------------------------
        m = m:gsub("Only available in%s*(.-)%s*;%s*switches to%s*(.-)%s*in%s*(.-)%s*%.", function(st1, sk, st2)
            local function trStance(s)
                s = s:gsub("Imagination Stance", "Стойке фантазии")
                s = s:gsub("Nightmare Stance", "Стойке кошмара")
                s = s:gsub("Offensive Stance", "Атакующей стойке")
                s = s:gsub("Defense Form", "Защитной стойке")
                return s
            end
            local function trSkill(s)
                s = s:gsub("Mind Fire", "Пламя разума")
                s = s:gsub("Psychotherapy", "Психотерапию")
                s = s:gsub("Consciousness Shock", "Удар сознания")
                s = s:gsub("Pacify", "Умиротворение")
                s = s:gsub("Psychological Suggestion", "Внушение")
                s = s:gsub("Angry Slam", "Яростный удар")
                s = s:gsub("Greatsword Slash", "Удар двуручником")
                s = s:gsub("Blade of Withering", "Клинок увядания")
                return s
            end
            return string.format("Доступно только в %s; в %s переключается на %s.", trStance(st1), trStance(st2), trSkill(sk))
        end)

        -- ----------------------------------------------------
        -- ПУТЬ ЗРИТЕЛЯ / СНОВИДЕЦ (Spectator)
        -- ----------------------------------------------------
        -- Психотерапия (Psychotherapy)
        m = m:gsub("Continuously heals allies within range, restoring%s+(.-)%s+Health to allies within range every ([%d%.]+) seconds, up to (%d+) times%.", "Непрерывно исцеляет союзников в области, восстанавливая %1 ед. здоровья союзникам в области каждые %2 сек. (до %3 раз).")
        m = m:gsub("You can move and cast other skills while this skill is active%.", "Во время действия навыка можно двигаться и применять другие навыки.")
        m = m:gsub("You can move and cast other skills while casting%.", "Во время применения можно двигаться и применять другие навыки.")
        m = m:gsub("You can release other skills and move while casting%.", "Во время применения можно двигаться и применять другие навыки.")

        -- Ментальная чума (Mental Plague)
        m = m:gsub("Deals (.-) magic damage to enemies in the area and applies (.-) for ([%d%.]+) seconds%.", "Наносит %1 маг. урона врагам в области и накладывает %2 на %3 сек.")
        m = m:gsub("When dealing damage to targets affected by (.-)Mental Plague(.-), an additional coordinated attack is triggered, dealing magic damage equal to (.-)%% of the base effect%.", "При нанесении урона целям под действием Ментальной чумы срабатывает совместная атака, наносящая маг. урон в размере %3%% от базового эффекта.")
        m = m:gsub("Spread a Mental Plague in the target area, dealing damage and infecting enemy units within range%.", "Распространяет Ментальную чуму в выбранной области, нанося урон и заражая врагов.")
        m = m:gsub("Deals (.-) magic damage to the locked target and surrounding enemies, and (.-) enemy monsters in the (.-)Plague(.-) state%.", "Наносит %1 маг. урона захваченной цели и окружающим врагам, подбрасывая монстров в состоянии Чумы.")
        m = m:gsub("Deals (.-) magic damage to the locked target and surrounding enemies", "Наносит %1 маг. урона захваченной цели и окружающим врагам")

        -- Стойка кошмара и Стойка фантазии
        m = m:gsub("A Nightmare Stance specialized in combat%. When using combat skills to deal damage, the user heals themselves, but the healing applied by the user is significantly reduced%.", "Боевая стойка Кошмара. При нанесении урона боевыми навыками исцеляет заклинателя, но эффективность применяемого им лечения сильно снижена.")
        m = m:gsub("In Nightmare Stance, when dealing damage, your self%-healing is increased by (%d+)%%%.", "В стойке Кошмара при нанесении урона самоисцеление повышено на %1%%.")
        m = m:gsub("In Nightmare Stance, healing ability is no longer reduced%.", "В стойке Кошмара эффективность лечения больше не снижается.")

        -- Удар сознания, Умиротворение, Внушение
        m = m:gsub("Consciousness Shock applies (.-) Nightmare (.-) upon hit%. After (%d+) stacks, the Nightmare explosion deals magic damage to the target and enemies within (%d+) meters equal to (.-)%% of the base effect%.", "Удар сознания накладывает уровень Кошмара при попадании. При накоплении %3 уровней взрыв Кошмара наносит цели и врагам в радиусе %4 м маг. урон в размере %5%% от базового эффекта.")
        m = m:gsub("A three%-stage combo that launches mental energy attacks, each stage dealing (.-) magic damage to enemies%.", "Серия из 3 ударов ментальной энергией, каждый этап наносит %1 маг. урона врагам.")
        m = m:gsub("A three%-stage combo that pacifies the minds of allies, each stage restoring (.-) Health to allied targets%.", "Серия из 3 этапов, успокаивающая разум союзников и восстанавливающая %1 ед. здоровья союзным целям.")
        m = m:gsub("Causes party members within (%d+) meters to gain a <HighLight>Heart Healing Mark</>, lasting for ([%d%.]+) seconds%..-", "Накладывает на членов группы в радиусе %1 м <HighLight>Метку исцеления</> на %2 сек.")
        m = m:gsub("Causes a target party member to gain a <HighLight>Mind Reading Mark</>, lasting for ([%d%.]+) seconds%..-", "Накладывает на члена группы <HighLight>Метку чтения мыслей</> на %1 сек.")
        m = m:gsub("Creates a field that deals (.-) magic damage to enemies within range every second for (%d+) seconds%.", "Создает поле, наносящее врагам внутри %1 маг. урона каждую секунду в течение %2 сек.")

        -- ----------------------------------------------------
        -- ПУТЬ ВОИНА / ГИГАНТ (Warrior / Giant)
        -- ----------------------------------------------------
        m = m:gsub("As a natural Weapon Master, the Warrior can skillfully use a variety of weapons%. Use dual swords in Offensive Stance to increase your damage; use a greatsword in Defense Form to gain damage reduction and more easily attract monster aggro%.", "Прирожденный Мастер оружия, Воин искусно владеет разными клинками. В атакующей стойке парные мечи увеличивают урон; в защитной стойке двуручник снижает урон и привлекает внимание врагов.")
        m = m:gsub("Dash forward with the sword at high speed to slash, attacking units along the path and gaining a Dusk Mark %(used to strengthen Angry Slam and finisher skills%)%. Applies decay %(continuous damage%) in Offensive Stance, and increases your block in Defense Form%.", "Совершает стремительный выпад вперед с мечом, атакуя врагов на пути и получая Метку сумерек (для усиления Яростного удара и добиваний). В атакующей стойке накладывает увядание, в защитной стойке повышает блок.")
        m = m:gsub("Wield dual swords to launch a storm%-like offensive, dealing massive damage and controlling enemies with continuous slashes%. You gain a Dusk Mark %(used to strengthen Angry Slam and finisher skills%)%. Applies decay %(continuous damage%) in Offensive Stance%.", "Яростно обрушивает вихрь ударов парными клинками, нанося огромный урон и контролируя врагов сериями взмахов. Дает Метку сумерек. В атакующей стойке накладывает увядание.")
        m = m:gsub("Dash in a designated direction, dealing (.-) physical damage to enemies along the path%. Upon hitting an enemy, gain (.-) Twilight Mark.-", "Совершает рывок в выбранном направлении, нанося %1 физ. урона врагам на пути. При попадании дает %2 Метку сумерек.")
        m = m:gsub("Dash rapidly toward an enemy target, dealing (.-) physical damage to enemies in the destination area.-", "Совершает стремительный рывок к цели, нанося %1 физ. урона врагам в точке прибытия.")
        m = m:gsub("Perform three consecutive attacks on enemies in a small range in front of you, dealing (.-) physical damage respectively%..-", "Совершает три последовательных удара по врагам перед собой, нанося %1 физ. урона.")
        m = m:gsub("Let out a stunning roar, dealing (.-) physical damage to surrounding enemies.-Then, summon a giant phantom shadow to slam the ground, dealing (.-) physical damage and knocking down monsters.-", "Издает оглушительный рев, нанося %1 физ. урона врагам вокруг. Затем призывает фантом гиганта, сотрясающий землю, нанося %2 физ. урона и сбивая монстров с ног.")
        m = m:gsub("Continuously strike surrounding enemies, dealing (.-) physical damage to enemies in range every (.-) seconds for (.-) seconds, dragging enemy monsters toward you.-", "Непрерывно сокрушает врагов вокруг, нанося %1 физ. урона каждые %2 сек. в течение %3 сек. и притягивая монстров к себе.")
        m = m:gsub("Deploy a domain, (.-), while dealing (.-) physical damage to enemies in range; then, continue to charge and gain Super Armor, dealing (.-) physical damage to enemies in range upon completion%..-", "Разворачивает поле боя, даруя союзникам защиту и нанося %2 физ. урона врагам вокруг. Затем накапливает силы в Суперброне и наносит %3 физ. урона.")
        m = m:gsub("Summon a giant sword formation at the locked target's location, dealing (.-) physical damage%. Gain (.-) Twilight Mark.-", "Призывает строй гигантских мечей в точке цели, нанося %1 физ. урона и получая Метку сумерек.")
        m = m:gsub("Gain (.-)Silver Rapier(.-) for (.-) seconds; gain (%d+) stacks of (.-)Dawn(.-)%. When casting other skills, tap this skill to directly gain all buff effects%.", "Дарует Серебряную рапиру на %3 сек. и %4 ур. Рассвета. При применении других навыков нажмите этот навык, чтобы сразу получить все эффекты.")
        m = m:gsub("In Offensive Stance, each hit on an enemy applies (.-) seconds of (.-)Decay(.-)%.", "В атакующей стойке каждое попадание накладывает Увядание на %1 сек.")
        m = m:gsub("In Defense Form, (.-)%.", "В защитной стойке: %1.")

        -- ----------------------------------------------------
        -- ПУТЬ ЖРЕЦА / СОЛНЦЕ (Bard / Sun)
        -- ----------------------------------------------------
        m = m:gsub("Solar Energy must be at least (%d+) points to cast%. Consumes all Solar Energy, dealing (.-) physical damage to the locked target and triggering (.-)Unshadowed(.-)%..-", "Требуется минимум %1 ед. Солнечной энергии. Поглощает всю энергию, нанося %2 физ. урона цели и активируя Бестеневой эффект.")
        m = m:gsub("Can only be cast when Spirituality is at least (%d+) points%. Consumes (%d+) points of Spirituality.-dealing (.-) physical damage to enemies in a frontal range and (.-)Knocking Down(.-) enemy monsters%..-", "Требуется минимум %1 ед. энергии. Поглощает %2 ед., нанося %3 физ. урона врагам в секторе перед собой и сбивая монстров с ног.")
        m = m:gsub("Condenses a short axe to continuously attack surrounding enemies for (.-) seconds, dealing (.-) physical damage every (.-) seconds.-", "Призывает священный топор, непрерывно атакуя врагов вокруг в течение %1 сек. и нанося %2 физ. урона каждые %3 сек.")
        m = m:gsub("Summons Holy Light to strengthen yourself%. Gain (.-) points of Solar Energy immediately upon casting.-", "Призывает Святой Свет для усиления. Мгновенно дает %1 ед. Солнечной энергии и увеличивает наносимый урон.")
        m = m:gsub("Establish a Solar Oath at the designated location lasting (.-) seconds, dealing (.-) physical damage to enemies within the area and applying continuous Stagnation%..-", "Создает Солнечную клятву в выбранной точке на %1 сек., нанося %2 физ. урона врагам в области и накладывая непрерывную Тягучесть.")
        m = m:gsub("In the (.-)Blazing Sun(.-) state, you will leap in front of the enemy and then slash forward with full force, dealing (.-) physical damage and (.-) knocking down (.-) enemy monsters, and gain (%d+) point of (.-)Solar Energy(.-) upon hitting%.", "В состоянии Яркого Солнца совершает выпад к врагу и рубит изо всех сил, нанося %3 физ. урона, сбивая монстров с ног и получая %7 ед. Солнечной энергии.")

        -- ----------------------------------------------------
        -- ПУТЬ УЧЕНИКА / ДВЕРЬ / ТАЙНОВЕДЕЦ (Apprentice / Door)
        -- ----------------------------------------------------
        m = m:gsub("Fire a beam at the locked target to deal (.-) magic damage%. The beam will refract to nearby enemy targets up to (%d+) times.-", "Выпускает луч света в захваченную цель, нанося %1 маг. урона. Луч преломляется во врагов рядом до %2 раз, накладывая Тягучесть и даруя Тайные знания.")
        m = m:gsub("Summon a Starlight Cage to deal (.-) magic damage to the locked target with a (.-) base probability of Stun for (.-) seconds%..-", "Призывает Звёздную клетку, нанося %1 маг. урона цели и с вероятностью %2 оглушая ее на %3 сек.")
        m = m:gsub("Rain down Star Sand on the target area, dealing (.-) magic damage to enemies within the range and applying.-", "Обрушивает Звёздный песок на выбранную область, нанося %1 маг. урона врагам в зоне действия и подбрасывая монстров.")
        m = m:gsub("Release a Spatial Cage to trap the target area, dealing (.-) magic damage to enemies within range and applying Imprisonment for (.-) seconds%..-", "Создает Клетку пространства, нанося %1 маг. урона врагам в зоне и накладывая Заточение на %2 сек.")
        m = m:gsub("Unfold a Folding Screen of spaces to block attacks and deal (.-) magic damage to enemies passing through%..-", "Разворачивает Ширму пространств, блокируя атаки и нанося %1 маг. урона проходящим врагам.")

        -- ----------------------------------------------------
        -- ПУТЬ ШУТА / ПРОВИДЕЦ (Seer / Fool)
        -- ----------------------------------------------------
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of Card Energy and one Spirituality Blue Card%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко Энергии карт и одну Синюю карту духовности.")
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of <HighLight>Card Energy</> and one <HighLight>Spirituality Blue Card</>%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>.")
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of <HighLight>Card Energy</HighLight> and one <HighLight>Spirituality Blue Card</HighLight>%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко <HighLight>Энергии карт</HighLight> и одну <HighLight>Синюю карту духовности</HighLight>.")
        m = m:gsub("Manipulate the target's spirit body threads to apply (.-), while deploying a Realm of Mysteries around the target, causing enemies in range to suffer (.-) for ([%d%.]+) seconds, dealing (.-) physical damage 6 times; then, a final strike deals (.-) physical damage%. If the target is a monster, (.-) the target; if the target is a player, (.-) the target for ([%d%.]+) seconds%. Upon casting, gain one (.-); if the last card obtained was a (.-), gain a (.-) instead%.", function(slow, stag, t1, dmg1, dmg2, launch, stun, t2, c1, c2, c3)
            slow = slow:gsub("Slow", "Замедление")
            stag = stag:gsub("Stagnation", "Тягучесть")
            launch = launch:gsub("launch", "подбрасывает")
            stun = stun:gsub("Stun", "оглушает")
            return string.format("Управляет нитями духовного тела цели, накладывая %s, и разворачивает вокруг цели Царство тайн, накладывая на врагов в области %s на %s сек. и нанося %s физ. урона 6 раз. Затем финальный удар наносит %s физ. урона. Если цель — монстр, %s цель; если цель — игрок, %s цель на %s сек. При применении дает одну %s; если последней полученной картой была %s, дает вместо этого %s.",
                slow, stag, t1, dmg1, dmg2, launch, stun, t2, c1, c2, c3)
        end)
        m = m:gsub("Manipulate the target's spirit body threads to apply (.-), while deploying a Realm of Mysteries around the target, causing enemies in range to suffer (.-) for ([%d%.]+) seconds, dealing (.-) physical damage 6 times; then, a final strike deals (.-) physical damage%.", function(slow, stag, t1, dmg1, dmg2)
            slow = slow:gsub("Slow", "Замедление")
            stag = stag:gsub("Stagnation", "Тягучесть")
            return string.format("Управляет нитями духовного тела цели, накладывая %s, и разворачивает вокруг цели Царство тайн, накладывая на врагов в области %s на %s сек. и нанося %s физ. урона 6 раз. Затем финальный удар наносит %s физ. урона.",
                slow, stag, t1, dmg1, dmg2)
        end)
        m = m:gsub("If the target is a monster, (.-) the target; if the target is a player, (.-) the target for ([%d%.]+) seconds%.", function(launch, stun, t)
            launch = launch:gsub("launch", "подбрасывает")
            stun = stun:gsub("Stun", "оглушает")
            return string.format("Если цель — монстр, %s цель; если цель — игрок, %s цель на %s сек.", launch, stun, t)
        end)
        m = m:gsub("Upon casting, gain one (.-); if the last card obtained was a (.-), gain a (.-) instead%.", "При применении дает одну %1; если последней полученной картой была %2, дает вместо этого %3.")
        m = m:gsub("Upon casting, gain one (.-) and apply one stack of (.-) to enemies hit; if the last card obtained was a (.-), gain a (.-) instead and gain one point of (.-)%.", "При применении дает одну %1 и накладывает один уровень %2 на пораженных врагов; если последней полученной картой была %3, дает вместо этого %4 и 1 очко %5.")
        m = m:gsub("Upon casting, gain one (.-) and apply one stack of (.-) to enemies hit by the explosion; if the last card obtained was a (.-), gain a (.-) instead and gain one point of (.-) upon the explosion hit%.", "При применении дает одну %1 и накладывает один уровень %2 на врагов, пораженных взрывом; если последней полученной картой была %3, дает вместо этого %4 и 1 очко %5 при попадании взрыва.")

        -- Карты Шута
        m = m:gsub("Fate Yellow Card/Spirituality Blue Card: When (%d+) Fate Yellow Cards/Spirituality Blue Cards are obtained, consume all cards to cause the Finisher Skill Shuffle Cards to switch to Fooling of Fate/Spirituality Burst%.", "Жёлтая карта судьбы / Синяя карта духовности: При сборе %1 Жёлтых карт судьбы / Синих карт духовности они расходуются, переключая Добивание: Тасование карт на Одурачивание судьбы / Всплеск духовности.")
        m = m:gsub("Fate Yellow Card/Spirituality Blue Card: When three Fate Yellow Cards/Spirituality Blue Cards are collected, consume all cards to switch the Finisher Skill Shuffle to Fooling of Fate/Spirituality Burst%.", "Жёлтая карта судьбы / Синяя карта духовности: При сборе 3 карт они расходуются, переключая Добивание: Тасование на Одурачивание судьбы / Всплеск духовности.")
        m = m:gsub("<FaintYellow>Fate Yellow Card</>/<FaintYellow>Spirituality Blue Card</>: When (%d+) <HighLight>Fate Yellow Cards</>/<HighLight>Spirituality Blue Cards</> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</> to switch to <HighLight>Fooling of Fate</>/<HighLight>Spirituality Burst</>%.", "<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе %1 <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")
        m = m:gsub("<FaintYellow>Fate Yellow Card</FaintYellow>/<FaintYellow>Spirituality Blue Card</FaintYellow>: When (%d+) <HighLight>Fate Yellow Cards</HighLight>/<HighLight>Spirituality Blue Cards</HighLight> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</HighLight> to switch to <HighLight>Fooling of Fate</HighLight>/<HighLight>Spirituality Burst</HighLight>%.", "<FaintYellow>Жёлтая карта судьбы</FaintYellow>/<FaintYellow>Синяя карта духовности</FaintYellow>: При сборе %1 <HighLight>Жёлтых карт судьбы</HighLight>/<HighLight>Синих карт духовности</HighLight> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")
        m = m:gsub("<FaintYellow>Fooling of Fate Yellow Card</>/<FaintYellow>Spirituality Blue Card</>: When three <HighLight>Fooling of Fate Yellow Cards</>/<HighLight>Spirituality Blue Cards</> are collected, consume all cards to switch the <HighLight>Finisher Skill Shuffle</> to <HighLight>Fooling of Fate</>/<HighLight>Spirituality Burst</>%.", "<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе трех <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> все карты расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")
        m = m:gsub("Card Energy: Can hold up to (%d+) points; when reaching (%d+) points, the Finisher Skill switches and locks to Shuffle Cards%.", "Энергия карт: Вмещает до %1 очков; при накоплении %2 очков Добивание переключается и блокируется на Тасовании карт.")
        m = m:gsub("<FaintYellow>Card Energy</>: Can hold up to (%d+) points%. When reaching (%d+) points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>%.", "<FaintYellow>Энергия карт</>: Вмещает до %1 очков. При накоплении %2 очков <HighLight>Добивание</> переключается и блокируется на <HighLight>Тасовании карт</>.")
        m = m:gsub("<FaintYellow>Card Energy</>: Can hold up to (%d+) points%. When reaching (%d+) points, the <HighLight>Finisher Skill</HighLight> switches and locks to <HighLight>Shuffle Cards</HighLight>%.", "<FaintYellow>Энергия карт</>: Вмещает до %1 очков. При накоплении %2 очков <HighLight>Добивание</HighLight> переключается и блокируется на <HighLight>Тасовании карт</>.")
        m = m:gsub("<FaintYellow>Card Energy</FaintYellow>: Can hold up to (%d+) points%. When reaching (%d+) points, the <HighLight>Finisher Skill</HighLight> switches and locks to <HighLight>Shuffle Cards</HighLight>%.", "<FaintYellow>Энергия карт</FaintYellow>: Вмещает до %1 очков. При накоплении %2 очков <HighLight>Добивание</HighLight> переключается и блокируется на <HighLight>Тасовании карт</>.")

        -- ----------------------------------------------------
        -- КУКЛЫ И СВЯЗИ (Puppets & Fellows)
        -- ----------------------------------------------------
        m = m:gsub("The doll awakens as its master enters combat%. When the master's skill hits the primary enemy target, the doll attaches itself to the target, causing them to suffer from the (.-)Beset by Misfortune(.-) effect%..-", "Кукла пробуждается при вступлении хозяина в бой. При попадании навыка хозяина по цели кукла прикрепляется к ней, накладывая эффект «Обречённость на несчастье».")
        m = m:gsub("Allies and self within the (.-)Poetic aura(.-) gain increased Critical Hit rate%..-", "Союзники и заклинатель в Поэтической ауре получают повышенный шанс крит. удара.")
        m = m:gsub("Allies and self within the (.-)Tranquility aura(.-) can recover a small amount of Health with each attack%..-", "Союзники и заклинатель в Ауре покоя восстанавливают здоровье при каждой атаке.")
        m = m:gsub("Allies can retain the Acceleration effect for an additional (%d+) seconds after leaving the Spirit Mediumship Ritual range%.", "Союзники сохраняют ускорение ещё %1 сек. после выхода из зоны Спиритического ритуала.")
        m = m:gsub("Perform a Fortune%-Turning Ritual, increasing the luck of all team members and granting (.-)%.", "Проводит Ритуал удачи, увеличивая удачу всех членов группы и даруя %1.")
        m = m:gsub("Recite a Midnight Poem, putting surrounding enemies to Sleep for (.-) seconds%..-", "Декламирует Полуночную поэму, усыпляя врагов вокруг на %1 сек.")
        m = m:gsub("Chant an Ensemble Poem to grant allies continuous healing and Damage Reduction for (.-) seconds%..-", "Исполняет Созвучие стихов, даруя союзникам периодическое исцеление и снижение урона на %1 сек.")
        m = m:gsub("Deliver a Law Judgment upon enemies, dealing (.-) physical damage and applying (.-)%.", "Выносит Приговор закона врагам, нанося %1 физ. урона и накладывая %2.")

        -- ----------------------------------------------------
        -- ОБЩИЕ МЕХАНИКИ, ЗОНЫ, СТАТУСЫ И ПОДПИСИ
        -- ----------------------------------------------------
        m = m:gsub("Circular area with a radius of ([%d%.]+) meters", "Круглая область радиусом %1 м")
        m = m:gsub("within a ([%d%.]+)%-meter radius of the target", "в радиусе %1 м от цели")
        m = m:gsub("within a ([%d%.]+)%-meter radius of yourself", "в радиусе %1 м вокруг себя")
        m = m:gsub("fan%-shaped area in front of you", "сектор перед собой")
        m = m:gsub("within a circular area in front of you", "в круглой области перед собой")
        m = m:gsub("All .-skills level up together%. Gain extra Skill Points through %[Sequence Advancement%]%.", "Все навыки Пути прокачиваются вместе. Получайте доп. очки навыков за [Продвижение по Последовательностям].")
        m = m:gsub("All .-Skills level up together%. Gain extra Skill Points through %[Sequence Advancement%]%.", "Все навыки Пути прокачиваются вместе. Получайте доп. очки навыков за [Продвижение по Последовательностям].")
        m = m:gsub("Gain extra Skill Points through %[Sequence Advancement%]%.", "Получайте доп. очки навыков за [Продвижение по Последовательностям].")

        -- Интерфейс описания навыков (заголовки)
        m = m:gsub("Next Level Effect", "Эффект след. уровня")
        m = m:gsub("Current Level Effect", "Эффект тек. уровня")
        m = m:gsub("Base Effect", "Базовый эффект")

        -- Баффы и статусы
        m = m:gsub("Super Armor", "Суперброня")
        m = m:gsub("Slow ([%d%.]+)%%", "Замедление %1%%")
        m = m:gsub("Stagnation", "Тягучесть")
        m = m:gsub("Grievous Injury", "Тяжёлая рана")
        m = m:gsub("launch the target", "подбрасывает цель")
        m = m:gsub("Stun the target for ([%d%.]+) seconds", "оглушает цель на %1 сек.")
        m = m:gsub("Knocking Down", "Сбивание с ног")
        m = m:gsub("knocking down", "сбивание с ног")
        m = m:gsub("knock down", "сбить с ног")
        m = m:gsub("Imprisonment", "Заточение")
        m = m:gsub("Healing Reduction", "Снижение лечения")
        m = m:gsub("Damage Reduction", "Снижение урона")
        m = m:gsub("Physical Damage Boost", "Усиление физ. урона")
        m = m:gsub("Magic Damage Boost", "Усиление маг. урона")
        m = m:gsub("Skill Enhancement", "Усиление навыков")
        m = m:gsub("Skill Block", "Блок навыков")
        m = m:gsub("Pierce", "Пробивание")

        -- Дополнительные боевые термины способностей
        m = m:gsub("Induction Mark", "Метка внушения")
        m = m:gsub("Hypnosis", "Гипноз")
        m = m:gsub("Mind Fire", "Пламя разума")
        m = m:gsub("Psychotherapy", "Психотерапия")
        m = m:gsub("Consciousness Shock", "Удар сознания")
        m = m:gsub("Dream Weaving", "Плетение снов")
        m = m:gsub("Dream Analysis", "Анализ снов")
        m = m:gsub("Mind Control", "Контроль сознания")
        m = m:gsub("Mental Suggestion", "Ментальное внушение")
        m = m:gsub("Psychological Suggestion", "Ментальное внушение")
        m = m:gsub("Dream Return", "Возврат сна")
        m = m:gsub("Rebirth", "Перерождение")
        m = m:gsub("Intimidation", "Устрашение")
        m = m:gsub("Invisibility", "Незримость")
        m = m:gsub("Insight", "Прозрение")
        m = m:gsub("Frenzy", "Бешенство")
        m = m:gsub("Spirit Body Threads", "Нити духовного тела")
        m = m:gsub("Spirit Body Thread", "Нить духовного тела")
        m = m:gsub("Historical Projection", "Историческая проекция")
        m = m:gsub("Paper Figurine Substitute", "Замена бумажным человечком")
        m = m:gsub("Flame Controlling", "Управление пламенем")

        if m ~= text then
            return m
        end
    end


    return nil
end


-- Точные текстовые оверрайды интерфейса (Visible Text Exact Overrides)
Russian.visibleTextExactOverrides = {
    ["两位先生离开了，不知何时才能看到这充满风采的照片……"] = "Джентльмены ушли. Кто знает, когда мне удастся увидеть эту великолепную фотографию...",
    ["The two gentlemen have left. Who knows when I'll get to see this splendid photograph..."] = "Джентльмены ушли. Кто знает, когда мне удастся увидеть эту великолепную фотографию...",
    ["Win by Lying Down"] = "Лёгкие победы",
    ["Easy Wins"] = "Лёгкие победы",
    ["在<h>【附近】/【世界】</>聊天栏中打字输入“<HyperLink stylename=\"h\">风暴比烈酒更烈</>”"] = "Введите «<HyperLink stylename=\"h\">Буря крепче крепкого эля</>» в строке чата <h>[Рядом]/[Мир]</>",
    ["所向披靡，无往不利！{{player.name}}在<Chat_Highlight>{{gameMode.name}}</>中获得<Chat_Highlight>{{eventMessageParams.curWinStreak}}连胜</>，战场之上，新的神话已在书写！"] = "Непобедим и неудержим! {{player.name}} одерживает серию из <Chat_Highlight>{{eventMessageParams.curWinStreak}} побед</> в <Chat_Highlight>{{gameMode.name}}</>! На поле боя пишется новая легенда!",
    ["Invincible and unstoppable! {{player.name}} has achieved a <Chat_Highlight>{{eventMessageParams.curWinStreak}} win streak in <Chat_Highlight>{{gameMode.name}}</>! On the battlefield, a new legend is being written!</>"] = "Непобедим и неудержим! {{player.name}} одерживает серию из <Chat_Highlight>{{eventMessageParams.curWinStreak}} побед в <Chat_Highlight>{{gameMode.name}}</>! На поле боя пишется новая легенда!</>",
    ["推理检定"] = "Проверка дедукции",
    ["Deduction Check"] = "Проверка дедукции",
    ["发现它变成了一份地图，还标注了奇迹降临的位置……"] = "Вы обнаруживаете, что это карта, на которой отмечено место свершения чуда...",
    ["You discover it has become a map, marking the location where the miracle will occur..."] = "Вы обнаруживаете, что это карта, на которой отмечено место свершения чуда...",
    ["沙利亚特"] = "Сариат",
    ["Sariat"] = "Сариат",
    ["迪尼特"] = "Динит",
    ["Dinit"] = "Динит",
    ["罗、罗茜！你今天过得好吗？"] = "Р-Рози! Как твои дела сегодня?",
    ["罗、罗茜！你今天过得好吗?"] = "Р-Рози! Как твои дела сегодня?",
    ["R-Rosie! How are you today?"] = "Р-Рози! Как твои дела сегодня?",
    ["啊，弗雷泽！我很好，这束花是……"] = "О, Фрейзер! Всё хорошо. Этот букет...?",
    ["啊，弗雷泽！我很好，这束花是......"] = "О, Фрейзер! Всё хорошо. Этот букет...?",
    ["啊，弗雷泽！我很好，这束花是..."] = "О, Фрейзер! Всё хорошо. Этот букет...?",
    ["Oh, Frazier! I'm doing well. Is that bouquet...?"] = "О, Фрейзер! Всё хорошо. Этот букет...?",
    ["我想把它送给你，其实，我对你……"] = "Я хотел подарить его тебе. Вообще-то, я к тебе...",
    ["我想把它送给你，其实，我对你......"] = "Я хотел подарить его тебе. Вообще-то, я к тебе...",
    ["我想把它送给你，其实，我对你..."] = "Я хотел подарить его тебе. Вообще-то, я к тебе...",
    ["I wanted to give it to you. Actually, I..."] = "Я хотел подарить его тебе. Вообще-то, я к тебе...",
    ["再战·一号信徒"] = "Реванш: Верующий №1",
    ["Rematch: Believer Number One"] = "Реванш: Верующий №1",
    ["黎明降临"] = "Пришествие рассвета",
    ["Dawn Arrival"] = "Пришествие рассвета",
    ["仲裁烙印"] = "Клеймо арбитража",
    ["Arbitration Brand"] = "Клеймо арбитража",
    ["窥秘凝视"] = "Взор тайновидца",
    ["Mystery Pry Gaze"] = "Взор тайновидца",
    ["晨曦守护"] = "Защита утреннего света",
    ["Morning Light Protection"] = "Защита утреннего света",
    ["骑士誓约"] = "Клятва рыцаря",
    ["Knight's Oath"] = "Клятва рыцаря",
    ["蝶灵附身"] = "Одержимость духом бабочки",
    ["Butterfly Spirit Possession"] = "Одержимость духом бабочки",
    ["丧钟回响"] = "Эхо погребального звона",
    ["Death Knell Echo"] = "Эхо погребального звона",
    ["头狼连爪"] = "Серия когтей вожака",
    ["Alpha Wolf Claw Combo"] = "Серия когтей вожака",
    ["钻头守护"] = "Защита бура",
    ["Drill Protection"] = "Защита бура",
    ["要变强"] = "Стать сильнее",
    ["Improve"] = "Стать сильнее",
    ["今日已领取"] = "Получено сегодня",
    ["Claimed Today"] = "Получено сегодня",
    ["奖励已领取"] = "Награда получена",
    ["Reward Claimed"] = "Награда получена",
    ["已领取全部奖励"] = "Все награды получены",
    ["All Rewards Claimed"] = "Все награды получены",
    ["使用中"] = "Используется",
    ["In Use"] = "Используется",
    ["图鉴"] = "Атлас",
    ["Codex"] = "Атлас",
    ["跟随卡萝，来到了工厂区。"] = "Следуйте за Кэрол в Заводской район.",
    ["Follow Carol to the Factory District."] = "Следуйте за Кэрол в Заводской район.",
    ["全部重置"] = "Сбросить всё",
    ["Reset All"] = "Сбросить всё",
    ["装备方案"] = "Сборки экипировки",
    ["Equipment Builds"] = "Сборки экипировки",
    ["自动分解"] = "Авто-распыление",
    ["Auto-Dismantle"] = "Авто-распыление",
    ["获得方式"] = "Способ получения",
    ["How to Obtain"] = "Способ получения",
    ["下一级效果"] = "Эффект след. уровня",
    ["Next-Level Effect"] = "Эффект след. уровня",
    ["在目标位置召唤窥秘之眼，对目标造成持续伤害和减速。"] = "Призывает Око Тайны в указанную точку, нанося периодический урон и замедляя цель.",
    ["Summon an Eye of Mystery at the target location, dealing continuous damage and slowing the target."] = "Призывает Око Тайны в указанную точку, нанося периодический урон и замедляя цель.",
    ["感知灵界，观测星空，通过灵性物品启示的命运变化，解读其映射的现实空间异动、事态发展走向与潜在未知危险。"] = "Ощутите Мир Духов и наблюдайте за звёздами. Толкуйте изменения судьбы, открываемые духовными предметами, чтобы распознать отражаемые ими искажения реальности, развитие событий и скрытые угрозы.",
    ["Sense the spirit world and observe the stars. Interpret the changes in fate revealed by spiritual items to discern the real-world disturbances they reflect, how events may unfold, and potential unknown dangers."] = "Ощутите Мир Духов и наблюдайте за звёздами. Толкуйте изменения судьбы, открываемые духовными предметами, чтобы распознать отражаемые ими искажения реальности, развитие событий и скрытые угрозы.",
    ["占星启示期间，周围的玩家可以获得临时技能来获取占星指引。"] = "Во время Астрологического откровения союзники поблизости получают временный навык для астрологических указаний.",
    ["During Astrological Revelation, nearby players can gain a temporary skill to receive astrological guidance."] = "Во время Астрологического откровения союзники поблизости получают временный навык для астрологических указаний.",
    ["使自身获得武力加4，直觉加2。使用临时技能获取占星指引的玩家也可以获得武力加4，直觉加2。"] = "Дарует владельцу +4 к Силе и +2 к Интуиции. Игроки, использующие временный навык астрологических указаний, также получают +4 к Силе и +2 к Интуиции.",
    ["Gain +4 Might and +2 Intuition. Players who use the temporary skill to receive astrological guidance also gain +4 Might and +2 Intuition."] = "Дарует владельцу +4 к Силе и +2 к Интуиции. Игроки, использующие временный навык астрологических указаний, также получают +4 к Силе и +2 к Интуиции.",
    ["木桩训练"] = "Тренировочный манекен",
    ["Training Dummy"] = "Тренировочный манекен",
    ["家族任务"] = "Семейные задания",
    ["Family Quests"] = "Семейные задания",
    ["你尚未加入任何家族"] = "Вы ещё не вступили в семью.",
    ["You haven't joined a Family."] = "Вы ещё не вступили в семью.",
    ["[队伍]"] = "[Группа]",
    ["[Team]"] = "[Группа]",
    ["【附身能力】"] = "【Способность одержимости】",
    ["[Possession Ability]"] = "【Способность одержимости】",
    ["请选择要使用【灵体之线】的对象"] = "Выберите цель для [Нитей духовного тела]",
    ["Select a target for [Spirit Body Threads]"] = "Выберите цель для [Нитей духовного тела]",
    ["附身剩余时间"] = "Время одержимости",
    ["Possession Time Remaining"] = "Время одержимости",
    ["秘偶属性生效总览"] = "Параметры марионетки",
    ["Marionette Attribute Effects Overview"] = "Параметры марионетки",
    ["本频道可用传音发言"] = "В этом канале можно отправлять вещания",
    ["Broadcasts can be sent in this channel"] = "В этом канале можно отправлять вещания",
    ["本频道无法发言"] = "В этом канале нельзя писать",
    ["You cannot speak in this channel"] = "В этом канале нельзя писать",
    ["但我们的数据——"] = "Но наши данные...",
    ["But our data—"] = "Но наши данные...",
    ["哼唧！哼唧……"] = "Хрю-хрю! Хрю...",
    ["Oink! Oink..."] = "Хрю-хрю! Хрю...",
    ["嗯……都行。"] = "Хм... без разницы.",
    ["Hmm... anything's fine."] = "Хм... без разницы.",
    ["坏了，我可能把<P_Yellow>生物催长剂</>当成椰蓉洒在蛋糕上了！"] = "О нет, кажется, я по ошибке насыпал на торт <P_Yellow>стимулятор роста биомассы</> вместо кокосовой стружки!",
    ["Oh no, I may have sprinkled the <P_Yellow>bio-growth stimulant</> on the cake instead of shredded coconut!"] = "О нет, кажется, я по ошибке насыпал на торт <P_Yellow>стимулятор роста биомассы</> вместо кокосовой стружки!",
    ["培根……你真是救我于水火啊……"] = "Бэкон... ты буквально спас меня...",
    ["Bacon... you really saved me..."] = "Бэкон... ты буквально спас меня...",
    ["培根……培根怎么回事？？"] = "Бэкон... что случилось с Бэконом?!",
    ["Bacon... what's wrong with Bacon??"] = "Бэкон... что случилось с Бэконом?!",
    ["太好了！拿到数据了！"] = "Отлично! Данные получены!",
    ["Great! We got the data!"] = "Отлично! Данные получены!",
    ["好了。下次再来！"] = "Готово. Заходите ещё!",
    ["There you go. Come again!"] = "Готово. Заходите ещё!",
    ["情况有点失控了！跑啊！"] = "Ситуация выходит из-под контроля! Бежим!",
    ["This is getting out of control! Run!"] = "Ситуация выходит из-под контроля! Бежим!",
    ["第一次来吗？要什么口味？"] = "Впервые здесь? Какой вкус предпочитаете?",
    ["First time here? What flavor would you like?"] = "Впервые здесь? Какой вкус предпочитаете?",
    ["等下我就买一大堆小蛋糕给你！"] = "Чуть позже я куплю тебе целую кучу пирожных!",
    ["I'll buy you a whole bunch of cupcakes later!"] = "Чуть позже я куплю тебе целую кучу пирожных!",
    ["那就给你最经典的那种吧。"] = "Тогда держи классический вариант.",
    ["Then I'll give you the classic one."] = "Тогда держи классический вариант.",
    ["霍伊大学赛艇队招新！"] = "Набор в команду по гребле Университета Хой!",
    ["Hoy University Rowing Team is recruiting!"] = "Набор в команду по гребле Университета Хой!",
    ["成员列表"] = "Список участников",
    ["Member List"] = "Список участников",
    ["俱乐部会长"] = "Глава клуба",
    ["Club President"] = "Глава клуба",
    ["正式成员"] = "Действительный член",
    ["Full Member"] = "Действительный член",
    ["候补成员"] = "Кандидат",
    ["Reserve Member"] = "Кандидат",
    ["可预存"] = "Можно отложить",
    ["Can Pre-store"] = "Можно отложить",
    ["已预存"] = "Отложено",
    ["Pre-stored"] = "Отложено",
    ["新手"] = "Новичок",
    ["Beginner"] = "Новичок",
    ["赛季剧情"] = "Сюжет сезона",
    ["Season Story"] = "Сюжет сезона",
    ["提交可获得猎杀进度"] = "Сдайте для получения прогресса охоты",
    ["Submit to earn Hunt Progress"] = "Сдайте для получения прогресса охоты",
    ["可获得猎杀进度"] = "Прогресс охоты",
    ["Earn Hunt Progress"] = "Прогресс охоты",
    ["当前进度："] = "Текущий прогресс:",
    ["当前进度:"] = "Текущий прогресс:",
    ["Current Progress:"] = "Текущий прогресс:",
    ["击杀"] = "Убийства",
    ["Kills"] = "Убийства",
    ["助攻"] = "Помощь",
    ["Assists"] = "Помощь",
    ["技能名称"] = "Название навыка",
    ["Skill Name"] = "Название навыка",
    ["次数"] = "Количество",
    ["Count"] = "Количество",
    ["伤害量"] = "Урон",
    ["Damage"] = "Урон",
    ["伤害来源"] = "Источник урона",
    ["Damage Source"] = "Источник урона",
    ["寄售"] = "Аукцион",
    ["Consignment"] = "Аукцион",
    ["终末猎杀"] = "Финальная охота",
    ["Final Hunt"] = "Финальная охота",
    ["主宰争锋"] = "Битва Владык",
    ["主宰之战"] = "Битва Владык",
    ["Dominator's Clash"] = "Битва Владык",
    ["秩序世界"] = "Мир Порядка",
    ["World of Order"] = "Мир Порядка",
    ["本周获取上限"] = "Лимит на неделю",
    ["Weekly Limit"] = "Лимит на неделю",
    ["城市暗面"] = "Тёмная сторона города",
    ["Dark City"] = "Тёмная сторона города",
    ["新"] = "Новое",
    ["New"] = "Новое",
    ["同家族/俱乐部队员达到3人及以上"] = "3+ члена из одной семьи/клуба в группе",
    ["3+ Party Members From the Same Family/Club"] = "3+ члена из одной семьи/клуба в группе",
    ["对比"] = "Сравнить",
    ["Compare"] = "Сравнить",
    ["进攻模式·PVP"] = "Атакующий режим · PvP",
    ["Offensive Mode · PvP"] = "Атакующий режим · PvP",
    ["随机获得2-4个词条"] = "Даёт 2–4 случайных свойства",
    ["Grants 2-4 Random Affixes"] = "Даёт 2–4 случайных свойства",
    ["神圣之杖"] = "Священный посох",
    ["Holy Staff"] = "Священный посох",
    ["线索"] = "Улика",
    ["Clue"] = "Улика",
    ["组队跟随中..."] = "Следование за группой...",
    ["组队跟随中…"] = "Следование за группой...",
    ["Following Party..."] = "Следование за группой...",
    ["你在廷根的集体意识中失去了形态，意识正在退回现实..."] = "Вы потеряли форму в коллективном сознании Тингена. Ваше сознание возвращается в реальность...",
    ["You lost form within Tingen's collective consciousness. Your consciousness is returning to reality..."] = "Вы потеряли форму в коллективном сознании Тингена. Ваше сознание возвращается в реальность...",
    ["界面返回"] = "Назад",
    ["Back"] = "Назад",
    ["灵体之线玩法"] = "Нити духовного тела",
    ["Spirit Body Threads"] = "Нити духовного тела",
    ["廷根第一市民"] = "Первый гражданин Тингена",
    ["Tingen's First Citizen"] = "Первый гражданин Тингена",
    ["[封]"] = "[Запечатано]",
    ["[Sealed]"] = "[Запечатано]",
    ["狂袭式"] = "Стойка яростной атаки",
    ["Frenzied Assault"] = "Стойка яростной атаки",
    ["廷根守墓人"] = "Могильщик Тингена",
    ["Tingen Gravekeeper"] = "Могильщик Тингена",
    ["机器加工厂坊"] = "Мастерская механической обработки",
    ["Machine Processing Workshop"] = "Мастерская механической обработки",
    ["非凡材料每有1条词条格挡 +200"] = "Каждое свойство потустороннего материала даёт Блок +200",
    ["Each Beyonder Material affix grants Block +200"] = "Каждое свойство потустороннего материала даёт Блок +200",
    ["总探索度"] = "Общий прогресс исследования",
    ["Total Exploration"] = "Общий прогресс исследования",
    ["上限可累计至下周"] = "Неиспользованный лимит переносится на следующую неделю",
    ["Unused Limit Carries Over to Next Week"] = "Неиспользованный лимит переносится на следующую неделю",
    ["安迪哥努斯笔记"] = "Записная книжка семьи Антигон",
    ["Antigonus Notebook"] = "Записная книжка семьи Антигон",
    ["首通队伍"] = "Команда первого прохождения",
    ["First-Clear Team"] = "Команда первого прохождения",
    ["男子：（癫狂）万物的“母亲”，赐予我们新生！"] = "Мужчина: (Безумно) «Мать» всего сущего, даруй нам новую жизнь!",
    ["Man: (Manically) \"Mother\" of all things, grant us rebirth!"] = "Мужчина: (Безумно) «Мать» всего сущего, даруй нам новую жизнь!",
    ["“愚者”：拿上这个。"] = "«Шут»: Возьми это.",
    ["\"The Fool\": Take this."] = "«Шут»: Возьми это.",
    ["丑人：（有效期十四年？为什么要签这么久的合同……）"] = "Уродливый человек: (Срок действия — четырнадцать лет? Зачем подписывать контракт на столь долгий срок...)",
    ["Ugly Man: (Valid for fourteen years? Why would I need to sign such a long contract...)"] = "Уродливый человек: (Срок действия — четырнадцать лет? Зачем подписывать контракт на столь долгий срок...)",
    ["没有太大危险了，不用特别在意。"] = "Особой опасности больше нет, не стоит волноваться.",
    ["There's no real danger anymore, so you don't need to worry."] = "Особой опасности больше нет, не стоит волноваться.",
    ["罗珊小姐，这个铃铛是用来做什么的？"] = "Мисс Розанна, для чего нужен этот колокольчик?",
    ["Miss Rozanne, what is this bell for?"] = "Мисс Розанна, для чего нужен этот колокольчик?",
    ["三律之背反"] = "Антиномия трёх законов",
    ["Antinomy of the Three Laws"] = "Антиномия трёх законов",
    ["镜像之自我"] = "Зеркальное «я»",
    ["Mirrored Self"] = "Зеркальное «я»",
    ["技能增强提高<Mark>30</>。"] = "Усиление навыков повышено на <Mark>30</>.",
    ["Skill Enhancement increased by <Mark>30</>."] = "Усиление навыков повышено на <Mark>30</>.",
    ["技能增强提高<Mark>30</>。\n激活套装<Mark>灵与知回响</>时不生效。"] = "Усиление навыков повышено на <Mark>30</>.\nНе действует при активном комплекте <Mark>Эхо Духа и Знания</>.",
    ["Skill Enhancement increased by <Mark>30</>.\nDoes not take effect while the <Mark>Echo of Spirit and Knowledge</> set is active."] = "Усиление навыков повышено на <Mark>30</>.\nНе действует при активном комплекте <Mark>Эхо Духа и Знания</>.",
    ["<CostRed>{1,2,（烙印已失效）}</>技能增强提高<Mark>30</>。\n激活套装<Mark>灵与知回响</>时不生效。"] = "<CostRed>{1,2,(Клеймо неактивно)}</>Усиление навыков повышено на <Mark>30</>.\nНе действует при активном комплекте <Mark>Эхо Духа и Знания</>.",
    ["<CostRed>{1,2,(Brand inactive)}</>Skill Enhancement increased by <Mark>30</>.\nDoes not take effect while the <Mark>Echo of Spirit and Knowledge</> set is active."] = "<CostRed>{1,2,(Клеймо неактивно)}</>Усиление навыков повышено на <Mark>30</>.\nНе действует при активном комплекте <Mark>Эхо Духа и Знания</>.",
    ["技能增强提高30。\n激活套装灵与知回响时不生效。"] = "Усиление навыков повышено на 30.\nНе действует при активном комплекте Эхо Духа и Знания.",
    ["Skill Enhancement increased by 30.\nDoes not take effect while the Echo of Spirit and Knowledge set is active."] = "Усиление навыков повышено на 30.\nНе действует при активном комплекте Эхо Духа и Знания.",
}

return Russian
