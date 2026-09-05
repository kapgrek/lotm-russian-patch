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
    Dungeon = "Данж",
    PVP = "Арена",
    Equip = "Экип",
    Skill = "Навык",
    Talent = "Древо",
    Promotion = "Путь",
    Sealed = "Арты",
    SecretPartner = "Куклы",
    Fellow = "Союз",
    Paotuan = "TRPG",
    Guild = "Клуб",
    Home = "Замок",
    Task = "Квест",
    Family = "Семья",
    Qingyuan = "Связи",
    Achievement = "Слава",
    Strategy = "Гайд",
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
    ONE_CLICK_TITLE = "Помощник в один клик",
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
}

-- Прямой словарь Английский -> Русский (для текстов, уже переведенных патчем в English)
Russian.englishToRussian = {
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
    ["One-Click Assist"] = "Помощник в один клик",
    ["Auto-Dismantle Confirmation"] = "Подтверждение авто-распыления",
    ["Auto-Dismantle Settings"] = "Настройки авто-распыления",
    ["Appearance"] = "Внешний вид",

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
    ["One-Click Upgrade"] = "Быстрое улучшение",
    ["One-click Upgrade"] = "Быстрое улучшение",
    ["One-click upgrade"] = "Быстрое улучшение",
    ["Equip Skill"] = "Экипировать навык",
    ["Next-Level Effect"] = "Эффект след. уровня",
    ["Simple"] = "Кратко",
    ["Connections"] = "Связи",
    ["marionette"] = "Марионетки",
    ["Training Dummy"] = "Манекен",
    ["Single Target"] = "Одиночная цель",
    ["Area Target"] = "По области",
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
    ["Air Bullet"] = "Воздушная пуля",
    ["Air Bullets"] = "Воздушные пули",
    ["Tarot Array"] = "Таро-расклад",
    ["Miracle Card Rain"] = "Дождь карт чудес",
    ["Fool's Blessing"] = "Благословение Шута",
    ["Card Flying Dagger"] = "Карточный кинжал",
    ["Flame Jump"] = "Перемещение по пламени",
    ["Card Master"] = "Мастер карт",
    ["Realm of Mysteries"] = "Царство Тайн",
    ["Reveal Card"] = "Раскрытие карты",
    ["Paper Figurine Substitute"] = "Бумажный человечек",
    ["Fooling of Fate"] = "Одурачивание судьбы",
    ["Spirituality Burst"] = "Всплеск духовности",
    ["Shuffle Cards"] = "Тасование карт",
    ["Cut Cards"] = "Снятие карт",
    ["Open Door"] = "Открытие двери",
    ["Secret Words"] = "Тайные слова",
    ["Pendulum Divination"] = "Гадание на маятнике",
    ["Earthquake Slam"] = "Сотрясение земли",
    ["Praise the Sun"] = "Восславь Солнце",
    ["Emotional Spectrum"] = "Эмоциональный спектр",
    ["Poised to Strike"] = "Готовность к удару",
    ["Listen to Heart's Voice"] = "Глас сердца",
    ["Smiling Clown"] = "Улыбающийся клоун",
    ["War Soul Afterimage"] = "Остаточный образ души войны",
    ["Illusion Trick"] = "Иллюзорный фокус",
    ["Holy Light Favor"] = "Благосклонность Святого Света",
    ["Astrological Revelation"] = "Астрологическое откровение",
    ["Mental Comfort"] = "Психологическое утешение",
    ["Talent"] = "Таланты",
    ["Talents"] = "Таланты",
    ["Path"] = "Путь",
    ["Pathway"] = "Путь",
    ["Promotion"] = "Путь",
    ["Sequence"] = "Последовательность",
    ["Sealed"] = "Реликвии",
    ["Sealed Artifact"] = "Запечатанный артефакт",
    ["Sealed Artifacts"] = "Запечатанные артефакты",
    ["Secret Partner"] = "Марионетки",
    ["SecretPartner"] = "Марионетки",
    ["Marionette"] = "Марионетка",
    ["Marionettes"] = "Марионетки",
    ["Fellow"] = "Союзники",
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
    ["Guide"] = "Гайд",
    ["Strategy"] = "Гайд",
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
    ["Burst"] = "Взрывной урон",
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
    ["Dawn Arrival"] = "Пришествие рассвета",
    ["Arbitration Brand"] = "Клеймо арбитража",
    ["Mystery Pry Gaze"] = "Взор тайновидца",
    ["Morning Light Protection"] = "Защита утреннего света",
    ["Knight's Oath"] = "Клятва рыцаря",
    ["Butterfly Spirit Possession"] = "Одержимость духом бабочки",
    ["Death Knell Echo"] = "Эхо погребального звона",
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
    ["mystical item"] = "Арты",
    ["Mystical item"] = "Арты",
    ["Mystical Item"] = "Арты",
    ["Mystical Items"] = "Арты",
    ["Castle"] = "Замок",
    ["Home"] = "Замок",
    ["Task"] = "Квесты",
    ["Quest"] = "Квесты",
    ["Quests"] = "Квесты",
    ["Gear"] = "Экип",
    ["Equip"] = "Экип",
    ["Equipment"] = "Экип",
    ["Potion"] = "Зелья",
    ["Potions"] = "Зелья",
}

-- Китайский -> Русский (точные переопределения интерфейса и текста)
Russian.chineseToRussian = {
    ["全部"] = "Все",
    ["道具"] = "Все",
    ["非凡材料"] = "Ресурсы",
    ["非凡物质"] = "Ресурсы",
    ["神奇物品"] = "Арты",
    ["封印物"] = "Арты",
    ["家园"] = "Замок",
    ["庄园"] = "Замок",
    ["装备"] = "Экип",
    ["任务"] = "Квесты",
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
    ["秘偶"] = "Марионетка",
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
    ["一键升级"] = "Быстрое улучшение",
    ["装配技能"] = "Экипировать навык",
    ["下级效果"] = "Эффект след. уровня",
    ["空气子弹"] = "Воздушная пуля",
    ["空气弹"] = "Воздушная пуля",
    ["纸牌飞刀"] = "Карточный кинжал",
    ["塔罗牌阵"] = "Таро-расклад",
    ["奇迹牌雨"] = "Дождь карт чудес",
    ["火焰跳跃"] = "Перемещение по пламени",
    ["愚者祝福"] = "Благословение Шута",
    ["卡牌大师"] = "Мастер карт",
    ["诡秘领域"] = "Царство Тайн",
    ["开牌"] = "Раскрытие карты",
    ["纸人替身"] = "Бумажный человечек",
    ["命运愚弄"] = "Одурачивание судьбы",
    ["灵性爆发"] = "Всплеск духовности",
    ["洗牌"] = "Тасование карт",
    ["切牌"] = "Снятие карт",
    ["开门"] = "Открытие двери",
    ["秘语"] = "Тайные слова",
    ["灵摆占卜"] = "Гадание на маятнике",
    ["地震重击"] = "Сотрясение земли",
    ["赞美太阳"] = "Восславь Солнце",
    ["情绪光谱"] = "Эмоциональный спектр",
    ["蓄势待发"] = "Готовность к удару",
    ["倾听心声"] = "Глас сердца",
    ["微笑小丑"] = "Улыбающийся клоун",
    ["战魂残影"] = "Остаточный образ души войны",
    ["戏法假象"] = "Иллюзорный фокус",
    ["圣光眷顾"] = "Благосклонность Святого Света",
    ["占星启示"] = "Астрологическое откровение",
    ["心灵抚慰"] = "Психологическое утешение",
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

-- Кэш и поиск в большой базе перевода
local russianRuntimeMap = nil
local russianRuntimeUnavailable = false

function Russian.lookupRussianText(text)
    if not Russian.Enabled or type(text) ~= "string" or text == "" then
        return nil
    end

    -- 1. Сначала проверяем точный перевод с английского (UI патча)
    local ru = Russian.englishToRussian[text]
    if ru ~= nil then return ru end

    -- 2. Затем проверяем точный перевод с китайского
    ru = Russian.chineseToRussian[text]
    if ru ~= nil then return ru end

    -- 3. Затем проверяем динамическую базу RuntimeTextRussian
    if russianRuntimeMap == nil and not russianRuntimeUnavailable then
        local ok, loaded = pcall(require, "mods.cpdd_runtime_fixes.RuntimeTextRussian")
        if ok and type(loaded) == "table" then
            russianRuntimeMap = loaded
        else
            russianRuntimeUnavailable = true
        end
    end

    if russianRuntimeMap then
        local found = russianRuntimeMap[text]
        if found ~= nil then return found end
    end

    -- 4. Автоматическая обработка тегов времени (%d+Second)
    local sec = text:match("^(%d+)Second$")
    if sec ~= nil then
        return sec .. " сек."
    end

    -- 5. Контекстный перевод динамических описаний навыков и подсказок
    if #text > 20 then
        local m = text

        -- Благословение Шута (Подробнее / Next-level effect)
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of Card Energy and one Spirituality Blue Card%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко Энергии карт и одну Синюю карту духовности.")
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of <HighLight>Card Energy</> and one <HighLight>Spirituality Blue Card</>%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>.")
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds, simultaneously gaining (%d+) point of <HighLight>Card Energy</HighLight> and one <HighLight>Spirituality Blue Card</HighLight>%.", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек., одновременно даруя %4 очко <HighLight>Энергии карт</HighLight> и одну <HighLight>Синюю карту духовности</HighLight>.")

        -- Блок механики карт Шута
        m = m:gsub("Fate Yellow Card/Spirituality Blue Card: When (%d+) Fate Yellow Cards/Spirituality Blue Cards are obtained, consume all cards to cause the Finisher Skill Shuffle Cards to switch to Fooling of Fate/Spirituality Burst%.", "Жёлтая карта судьбы / Синяя карта духовности: При сборе %1 Жёлтых карт судьбы / Синих карт духовности они расходуются, переключая Добивание: Тасование карт на Одурачивание судьбы / Всплеск духовности.")
        m = m:gsub("Fate Yellow Card/Spirituality Blue Card: When three Fate Yellow Cards/Spirituality Blue Cards are collected, consume all cards to switch the Finisher Skill Shuffle to Fooling of Fate/Spirituality Burst%.", "Жёлтая карта судьбы / Синяя карта духовности: При сборе 3 карт они расходуются, переключая Добивание: Тасование на Одурачивание судьбы / Всплеск духовности.")
        m = m:gsub("<FaintYellow>Fate Yellow Card</>/<FaintYellow>Spirituality Blue Card</>: When (%d+) <HighLight>Fate Yellow Cards</>/<HighLight>Spirituality Blue Cards</> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</> to switch to <HighLight>Fooling of Fate</>/<HighLight>Spirituality Burst</>%.", "<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе %1 <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")
        m = m:gsub("<FaintYellow>Fate Yellow Card</FaintYellow>/<FaintYellow>Spirituality Blue Card</FaintYellow>: When (%d+) <HighLight>Fate Yellow Cards</HighLight>/<HighLight>Spirituality Blue Cards</HighLight> are obtained, consume all cards to cause the <HighLight>Finisher Skill Shuffle Cards</HighLight> to switch to <HighLight>Fooling of Fate</HighLight>/<HighLight>Spirituality Burst</HighLight>%.", "<FaintYellow>Жёлтая карта судьбы</FaintYellow>/<FaintYellow>Синяя карта духовности</FaintYellow>: При сборе %1 <HighLight>Жёлтых карт судьбы</HighLight>/<HighLight>Синих карт духовности</HighLight> они расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")
        m = m:gsub("<FaintYellow>Fooling of Fate Yellow Card</>/<FaintYellow>Spirituality Blue Card</>: When three <HighLight>Fooling of Fate Yellow Cards</>/<HighLight>Spirituality Blue Cards</> are collected, consume all cards to switch the <HighLight>Finisher Skill Shuffle</> to <HighLight>Fooling of Fate</>/<HighLight>Spirituality Burst</>%.", "<FaintYellow>Жёлтая карта судьбы</>/<FaintYellow>Синяя карта духовности</>: При сборе трех <HighLight>Жёлтых карт судьбы</>/<HighLight>Синих карт духовности</> все карты расходуются, переключая <HighLight>Добивание: Тасование карт</> на <HighLight>Одурачивание судьбы</>/<HighLight>Всплеск духовности</>.")

        -- Блок энергии карт
        m = m:gsub("Card Energy: Can hold up to (%d+) points; when reaching (%d+) points, the Finisher Skill switches and locks to Shuffle Cards%.", "Энергия карт: Вмещает до %1 очков; при достижении %2 очков Добивающий навык переключается и фиксируется на Тасование карт.")
        m = m:gsub("Card Energy: Can hold up to (%d+) points%. When reaching (%d+) points, the Finisher Skill switches and locks to Shuffle Cards", "Энергия карт: Вмещает до %1 очков. При %2 очках Добивающий навык переключается и фиксируется на Тасование карт.")
        m = m:gsub("<FaintYellow>Card Energy</>: Can hold up to (%d+) points; when reaching (%d+) points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>%.", "<FaintYellow>Энергия карт</>: Вмещает до %1 очков; при достижении %2 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.")
        m = m:gsub("<FaintYellow>Card Energy</FaintYellow>: Can hold up to (%d+) points; when reaching (%d+) points, the <HighLight>Finisher Skill</HighLight> switches and locks to <HighLight>Shuffle Cards</HighLight>%.", "<FaintYellow>Энергия карт</FaintYellow>: Вмещает до %1 очков; при достижении %2 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.")
        m = m:gsub("<FaintYellow>Card Energy</>: Can hold up to (%d+) points%. When reaching (%d+) points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>", "<FaintYellow>Энергия карт</>: Вмещает до %1 очков. При %2 очках <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>.")

        -- Благословение Шута (Кратко)
        m = m:gsub("Praise the Fool, increasing the user's damage for (%d+) seconds, and obtain a Spirituality Blue Card and one point of Card Energy %(used to unlock Finisher Skills%)%.", "Восславьте Шута: увеличивает урон персонажа на %1 сек., дарует Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).")
        m = m:gsub("The Fool that doesn't belong to this era; the mysterious ruler above the gray fog; the King of Yellow and Black who wields good luck%. Praise the Fool!", "Не принадлежащий этой эпохе Шут; таинственный правитель над серым туманом; Владыка Жёлтого и Чёрного, повелевающий удачей. Восславь Шута!")

        -- Воздушная пуля
        m = m:gsub("Maintain movement and continuously fire multiple Air Bullets at enemies; the user gains a Spirituality Blue Card and one point of Card Energy %(used to unlock Finisher Skills%)%.", "Позволяет двигаться и непрерывно выпускать множество Воздушных пуль во врагов; дает Синюю карту духовности и 1 очко Энергии карт (необходимо для открытия добивающих навыков).")
        m = m:gsub("Continuously fire Air Bullets at the locked target for ([%d%.]+) seconds, dealing (%d+) physical damage (%d+) times and <HyperLink stylename=\"M_Link\" u=\"2\">interrupting</> enemy monsters; you can move while casting and gain ([%d%.]+) seconds of <HyperLink stylename=\"M_Link\" u=\"11\">Super Armor</> and (%d+)%% Acceleration upon activation%. After casting, gain one point of <HighLight>Card Energy</> and one <HighLight>Spirituality Blue Card</>%.", "Непрерывно выпускайте Воздушные пули в цель в течение %1 сек., нанося %2 физ. урона %3 раз и <HyperLink stylename=\"M_Link\" u=\"2\">прерывая</> вражеских монстров; во время применения можно двигаться и получить при активации %4 сек. <HyperLink stylename=\"M_Link\" u=\"11\">Суперброни</> и %5%% ускорения. После применения дает 1 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>.")
        m = m:gsub("Continuously fire Air Bullets at the locked target for ([%d%.]+) seconds, dealing (%d+) physical damage (%d+) times and interrupting enemy monsters; you can move while casting and gain ([%d%.]+) seconds of Super Armor and (%d+)%% Acceleration upon activation%. After casting, gain one point of Card Energy and one Spirituality Blue Card%.", "Непрерывно выпускайте Воздушные пули в цель в течение %1 сек., нанося %2 физ. урона %3 раз и прерывая врагов; во время каста можно двигаться и получить %4 сек. Суперброни и %5%% ускорения. После каста дает 1 очко Энергии карт и одну Синюю карту духовности.")

        -- Таро-расклад
        m = m:gsub("Throw a Tarot card at the enemy, unfolding a Tarot Array to continuously deal damage, and knock up monster targets upon finishing%. The user obtains an identical card based on the type of the previous card obtained %(used to unlock Finisher Skills%) and the corresponding additional effect%.", "Бросает карту Таро во врага, разворачивая Таро-расклад для непрерывного нанесения урона, и подбрасывает монстров при завершении. Дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков), а также соответствующий дополнительный эффект.")
        m = m:gsub("Throw tarot cards at the target and deploy a card formation, dealing (.-) physical damage 4 times to enemy monsters within the formation and <HyperLink stylename=\"M_Link\" u=\"2\">interrupting</> them%. The formation then explodes, dealing (.-) physical damage and <HyperLink stylename=\"M_Link\" u=\"2\">launching</> enemy monsters within range, up to a maximum of <HyperLink stylename=\"M_Link\" u=\"2\">launching</> (%d+) targets%. Upon casting, gain one <HighLight>Fooling of Fate Yellow Card</> and apply one stack of <HighLight>Fooling Mark</> to enemies hit by the explosion; if the last card obtained was a <HighLight>Spirituality Blue Card</>, gain a <HighLight>Spirituality Blue Card</> instead and gain one point of <HighLight>Card Energy</> upon the explosion hit%.", "Бросает карты Таро в цель и разворачивает расклад карт, нанося вражеским монстрам в области %1 физ. урона 4 раза и <HyperLink stylename=\"M_Link\" u=\"2\">прерывая</> их. Затем расклад взрывается, нанося %2 физ. урона и <HyperLink stylename=\"M_Link\" u=\"2\">подбрасывая</> вражеских монстров в области (до %3 целей). При применении дает одну <HighLight>Жёлтую карту судьбы</> и накладывает один уровень <HighLight>Метки одурачивания</> на врагов, пораженных взрывом; если последней полученной картой была <HighLight>Синяя карта духовности</>, дает вместо этого <HighLight>Синюю карту духовности</> и 1 очко <HighLight>Энергии карт</> при попадании взрыва.")
        m = m:gsub("Throw tarot cards at the target and deploy a card formation, dealing (.-) physical damage 4 times to enemy monsters within the formation and interrupting them%. The formation then explodes, dealing (.-) physical damage and launching enemy monsters within range, up to a maximum of launching (%d+) targets%. Upon casting, gain one Fooling of Fate Yellow Card and apply one stack of Fooling Mark to enemies hit by the explosion; if the last card obtained was a Spirituality Blue Card, gain a Spirituality Blue Card instead and gain one point of Card Energy upon the explosion hit%.", "Бросает карты Таро в цель и разворачивает расклад карт, нанося врагам в области %1 физ. урона 4 раза и прерывая их. Затем расклад взрывается, нанося %2 физ. урона и подбрасывая вражеских монстров в области (до %3 целей). При применении дает одну Жёлтую карту судьбы и накладывает один уровень Метки одурачивания на врагов, пораженных взрывом; если последней полученной картой была Синяя карта духовности, дает вместо этого Синюю карту духовности и 1 очко Энергии карт при попадании взрыва.")

        -- Раскрытие карты
        m = m:gsub("Launch playing cards to attack the target%. After this skill deals damage (%d+) times, obtain an identical card based on the type of the previous card obtained %(used to unlock Finisher Skills%)%.", "Бросает игральные карты для атаки цели. После нанесения урона %1 раза дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков).")
        m = m:gsub("Performs a three%-stage attack on enemies in a frontal fan%-shaped area, with each stage dealing (%d+) physical damage and <HyperLink stylename=\"M_Link\" u=\"2\">interrupting</> enemy monsters%. After the first stage is released, you gain (.-) for (%d+) seconds%. The third stage applies one stack of <HighLight>Fooling Mark</> and has a base probability of (.-) to <HyperLink stylename=\"M_Link\" u=\"2\">knock back</> the enemy, applying a ([%d%.]+)%-second (.-) <HyperLink stylename=\"M_Link\" u=\"20\">Healing Reduction</> effect, while you gain one <HighLight>Fate Yellow Card</>%.", "Совершает трёхэтапную атаку по врагам в секторе перед собой, на каждом этапе нанося %1 физ. урона и <HyperLink stylename=\"M_Link\" u=\"2\">прерывая</> вражеских монстров. После первого этапа дает эффект %2 на %3 сек. Третий этап накладывает один уровень <HighLight>Метки одурачивания</> и с вероятностью %4 <HyperLink stylename=\"M_Link\" u=\"2\">отбрасывает</> врага, накладывая на %5 сек. эффект %6 <HyperLink stylename=\"M_Link\" u=\"20\">снижения исцеления</>, при этом вы получаете одну <HighLight>Жёлтую карту судьбы</>.")
        m = m:gsub("Performs a three%-stage attack on enemies in a frontal fan%-shaped area, with each stage dealing (%d+) physical damage and interrupting enemy monsters%. After the first stage is released, you gain (.-) for (%d+) seconds%. The third stage applies one stack of Fooling Mark and has a base probability of (.-) to knock back the enemy, applying a ([%d%.]+)%-second (.-) Healing Reduction effect, while you gain one Fate Yellow Card%.", "Совершает трёхэтапную атаку по врагам в секторе перед собой, на каждом этапе нанося %1 физ. урона и прерывая вражеских монстров. После первого этапа дает эффект %2 на %3 сек. Третий этап накладывает один уровень Метки одурачивания и с вероятностью %4 отбрасывает врага, накладывая на %5 сек. эффект %6 снижения исцеления, при этом вы получаете одну Жёлтую карту судьбы.")

        -- Дождь карт чудес
        m = m:gsub("Miraculously summon a rain of cards at the target location, dealing damage, Slow, and Stun to enemies, and additional Healing Reduction to players%. The user obtains an identical card based on the type of the previous card obtained %(used to unlock Finisher Skills%) and the corresponding additional effect%.", "Чудесным образом обрушивает дождь карт в указанную область, нанося урон, замедление и оглушение врагам, а также доп. снижение исцеления игрокам. Дает карту того же типа, что и предыдущая полученная карта (необходимо для открытия добивающих навыков), а также соответствующий дополнительный эффект.")
        m = m:gsub("Deal (.-) physical damage to enemies in range with a (.-) base chance to apply, for ([%d%.]+) seconds, the effect of (.-), and a (.-) base chance to <HyperLink stylename=\"M_Link\" u=\"6\">Stun</> enemies at the center of the range for ([%d%.]+) seconds, up to a maximum of <HyperLink stylename=\"M_Link\" u=\"6\">Stun</> (%d+) targets%. When hitting enemy players, apply ([%d%.]+) seconds of (.-) <HyperLink stylename=\"M_Link\" u=\"20\">Healing Reduction</>%. Upon casting, gain one <HighLight>Fooling of Fate Yellow Card</> and apply one stack of <HighLight>Fooling Mark</> to enemies hit; if the last card obtained was a <HighLight>Spirituality Blue Card</>, gain a <HighLight>Spirituality Blue Card</> instead and gain one point of <HighLight>Card Energy</>%.", "Наносит %1 физ. урона врагам в области с базовой вероятностью %2 наложить на %3 сек. эффект %4, а также с вероятностью %5 <HyperLink stylename=\"M_Link\" u=\"6\">оглушить</> врагов в центре области на %6 сек. (до %7 целей). При попадании по вражеским игрокам накладывает на %8 сек. эффект %9 <HyperLink stylename=\"M_Link\" u=\"20\">снижения исцеления</>. При применении дает одну <HighLight>Жёлтую карту судьбы</> и накладывает один уровень <HighLight>Метки одурачивания</> на пораженных врагов; если последней полученной картой была <HighLight>Синяя карта духовности</>, дает вместо этого <HighLight>Синюю карту духовности</> и 1 очко <HighLight>Энергии карт</>.")
        m = m:gsub("Deal (.-) physical damage to enemies in range with a (.-) base chance to apply, for ([%d%.]+) seconds, the effect of (.-), and a (.-) base chance to Stun enemies at the center of the range for ([%d%.]+) seconds, up to a maximum of Stun (%d+) targets%. When hitting enemy players, apply ([%d%.]+) seconds of (.-) Healing Reduction%. Upon casting, gain one Fooling of Fate Yellow Card and apply one stack of Fooling Mark to enemies hit; if the last card obtained was a Spirituality Blue Card, gain a Spirituality Blue Card instead and gain one point of Card Energy%.", "Наносит %1 физ. урона врагам в области с базовой вероятностью %2 наложить на %3 сек. эффект %4, а также с вероятностью %5 оглушить врагов в центре области на %6 сек. (до %7 целей). При попадании по вражеским игрокам накладывает на %8 сек. эффект %9 снижения исцеления. При применении дает одну Жёлтую карту судьбы и накладывает один уровень Метки одурачивания на пораженных врагов; если последней полученной картой была Синяя карта духовности, дает вместо этого Синюю карту духовности и 1 очко Энергии карт.")

        -- Тасование карт
        m = m:gsub("Collect Card Energy to unlock the skill%. Inject all collected Card Energy into a large number of cards, continuously pouring energy%-filled cards at enemies to deal high damage%. Other skills can be released and movement is possible during the casting process%.", "Соберите Энергию карт, чтобы открыть навык. Вливает всю собранную Энергию карт в множество карт, непрерывно осыпая врагов заряженными картами для нанесения огромного урона. Во время применения можно использовать другие навыки и двигаться.")
        m = m:gsub("Can only be released when <HighLight>Card Energy</> reaches 5 points%. Consume 5 points of <HighLight>Card Energy</> to continuously fire cards at the locked target for ([%d%.]+) seconds, dealing (.-) physical damage 12 times to the target and enemies in a small area, and <HyperLink stylename=\"M_Link\" u=\"2\">interrupting</> enemy monsters%. You can release other skills and move while casting%. \nAs long as this skill is available, the <HighLight>Finisher Skill</> will not switch to <HighLight>Fooling of Fate</> or <HighLight>Spirituality Burst</>, but they can still be unlocked by collecting <HighLight>Fooling of Fate Yellow Cards</> or <HighLight>Spirituality Blue Cards</> and will become available immediately after this skill is cast%.", "Можно применить только при 5 очках <HighLight>Энергии карт</>. Расходует 5 очков <HighLight>Энергии карт</>, чтобы непрерывно выпускать карты в цель в течение %1 сек., нанося %2 физ. урона 12 раз цели и врагам в небольшой области, <HyperLink stylename=\"M_Link\" u=\"2\">прерывая</> вражеских монстров. Во время применения можно использовать другие навыки и двигаться. \nПока навык доступен, <HighLight>Добивающий навык</> не переключится на <HighLight>Одурачивание судьбы</> или <HighLight>Всплеск духовности</>, но их все еще можно разблокировать, собирая <HighLight>Жёлтые карты судьбы</> или <HighLight>Синие карты духовности</>, и они станут доступны сразу после применения этого навыка.")
        m = m:gsub("Can only be released when Card Energy reaches 5 points%. Consume 5 points of Card Energy to continuously fire cards at the locked target for ([%d%.]+) seconds, dealing (.-) physical damage 12 times to the target and enemies in a small area, and interrupting enemy monsters%. You can release other skills and move while casting%. \nAs long as this skill is available, the Finisher Skill will not switch to Fooling of Fate or Spirituality Burst, but they can still be unlocked by collecting Fooling of Fate Yellow Cards or Spirituality Blue Cards and will become available immediately after this skill is cast%.", "Можно применить только при 5 очках Энергии карт. Расходует 5 очков Энергии карт, чтобы непрерывно выпускать карты в цель в течение %1 сек., нанося %2 физ. урона 12 раз цели и врагам в небольшой области, прерывая вражеских монстров. Во время применения можно использовать другие навыки и двигаться. \nПока навык доступен, Добивающий навык не переключится на Одурачивание судьбы или Всплеск духовности, но их все еще можно разблокировать, собирая Жёлтые карты судьбы или Синие карты духовности, и они станут доступны сразу после применения этого навыка.")

        -- Одурачивание судьбы
        m = m:gsub("Collect Fate Yellow Cards to unlock the skill%. Fool enemies within range, dealing massive damage%. Afterward, for a period of time, the user's normal attacks will apply a Fooling Mark that deals additional damage, and attack speed is increased%. Using it grants one point of Card Energy%.", "Соберите Жёлтые карты судьбы, чтобы открыть навык. Одурачивает врагов в области, нанося огромный урон. Затем в течение некоторого времени ваши обычные атаки будут накладывать Метку одурачивания, наносящую дополнительный урон, а скорость атаки повысится. Использование дает 1 очко Энергии карт.")
        m = m:gsub("Collect three <HighLight>Fooling of Fate Yellow Cards</> to unlock this skill%. Deal (.-) physical damage to enemies in range and apply one stack of <HighLight>Fooling Mark</>; enemy monsters are <HyperLink stylename=\"M_Link\" u=\"2\">interrupted</>%. Then, for ([%d%.]+) seconds, your Normal Attacks have a (.-) chance to apply one stack of <HighLight>Fooling Mark</> to the target, and you gain (.-)%. Gain one point of <HighLight>Card Energy</> after casting%. \nCasting Fooling of Fate will clear the <HighLight>Vulnerability</> effect of Spirituality Burst%.", "Соберите три <HighLight>Жёлтые карты судьбы</>, чтобы разблокировать этот навык. Наносит %1 физ. урона врагам в области и накладывает один уровень <HighLight>Метки одурачивания</>; монстры получают <HyperLink stylename=\"M_Link\" u=\"2\">прерывание</>. Затем в течение %2 сек. ваши Обычные атаки имеют шанс %3 наложить один уровень <HighLight>Метки одурачивания</> на цель, и вы получаете эффект %4. Дает 1 очко <HighLight>Энергии карт</> после применения. \nПрименение Одурачивания судьбы снимет эффект <HighLight>Уязвимости</> от Всплеска духовности.")
        m = m:gsub("Collect three Fooling of Fate Yellow Cards to unlock this skill%. Deal (.-) physical damage to enemies in range and apply one stack of Fooling Mark; enemy monsters are interrupted%. Then, for ([%d%.]+) seconds, your Normal Attacks have a (.-) chance to apply one stack of Fooling Mark to the target, and you gain (.-)%. Gain one point of Card Energy after casting%. \nCasting Fooling of Fate will clear the Vulnerability effect of Spirituality Burst%.", "Соберите три Жёлтые карты судьбы, чтобы разблокировать этот навык. Наносит %1 физ. урона врагам в области и накладывает один уровень Метки одурачивания; монстры получают прерывание. Затем в течение %2 сек. ваши Обычные атаки имеют шанс %3 наложить один уровень Метки одурачивания на цель, и вы получаете эффект %4. Дает 1 очко Энергии карт после применения. \nПрименение Одурачивания судьбы снимет эффект Уязвимости от Всплеска духовности.")

        -- Всплеск духовности
        m = m:gsub("Collect Spirituality Blue Cards to unlock the skill%. Release the user's spiritual power, dealing massive damage to enemies within range and applying Vulnerability%. Using it grants one point of Card Energy%.", "Соберите Синие карты духовности, чтобы открыть навык. Высвобождает духовную силу заклинателя, нанося огромный урон врагам в области и накладывая Уязвимость. Использование дает 1 очко Энергии карт.")
        m = m:gsub("Collect three <HighLight>Spirituality Blue Cards</> to unlock this skill%. Deal (.-) physical damage to enemies in range and apply <HighLight>Vulnerability</> for ([%d%.]+) seconds, <HyperLink stylename=\"M_Link\" u=\"2\">launching</> enemy monsters%. Gain one point of <HighLight>Card Energy</> after casting%. \nCasting Spirituality Burst will clear the self%-buff effects of Fooling of Fate%.", "Соберите три <HighLight>Синие карты духовности</>, чтобы разблокировать этот навык. Наносит %1 физ. урона врагам в области и накладывает <HighLight>Уязвимость</> на %2 сек., <HyperLink stylename=\"M_Link\" u=\"2\">подбрасывая</> вражеских монстров. Дает 1 очко <HighLight>Энергии карт</> после применения. \nПрименение Всплеска духовности снимет собственные усиления от Одурачивания судьбы.")
        m = m:gsub("Collect three Spirituality Blue Cards to unlock this skill%. Deal (.-) physical damage to enemies in range and apply Vulnerability for ([%d%.]+) seconds, launching enemy monsters%. Gain one point of Card Energy after casting%. \nCasting Spirituality Burst will clear the self%-buff effects of Fooling of Fate%.", "Соберите три Синие карты духовности, чтобы разблокировать этот навык. Наносит %1 физ. урона врагам в области и накладывает Уязвимость на %2 сек., подбрасывая вражеских монстров. Дает 1 очко Энергии карт после применения. \nПрименение Всплеска духовности снимет собственные усиления от Одурачивания судьбы.")

        -- Прыжок пламени
        m = m:gsub("Using Flame Jump also grants (%d+) fate cards and one point of Card Energy%.", "Использование Прыжка пламени также дает %1 карты судьбы и 1 очко Энергии карт.")

        -- Замена бумажного человечка
        m = m:gsub("Cleanse yourself of <HyperLink stylename=\"M_Link\" u=\"3\">control effects</>, leave a Paper Figurine Substitute at your position that inherits your locked marks, and leap in the direction of the joystick %(or backwards by default%)%. Gain ([%d%.]+) seconds of <HyperLink stylename=\"M_Link\" u=\"11\">Super Armor</> and, for ([%d%.]+) seconds, the effect of (.-) upon release%. The Paper Figurine Substitute will automatically perform three basic attacks on enemies, dealing (.-) physical damage respectively, then explode to deal (%d+) physical damage to surrounding enemies%.", "Снимает с себя <HyperLink stylename=\"M_Link\" u=\"3\">эффекты контроля</>, оставляет на своей позиции Замену бумажного человечка, перенимающую метки захвата цели, и совершает прыжок по направлению джойстика (по умолчанию назад). При применении дает %1 сек. <HyperLink stylename=\"M_Link\" u=\"11\">Суперброни</> и на %2 сек. эффект %3. Бумажный человечек автоматически проводит три базовые атаки по врагам, нанося соответственно %4 физ. урона, после чего взрывается, нанося %5 физ. урона окружающим врагам.")
        m = m:gsub("Cleanse yourself of control effects, leave a Paper Figurine Substitute at your position that inherits your locked marks, and leap in the direction of the joystick %(or backwards by default%)%. Gain ([%d%.]+) seconds of Super Armor and, for ([%d%.]+) seconds, the effect of (.-) upon release%. The Paper Figurine Substitute will automatically perform three basic attacks on enemies, dealing (.-) physical damage respectively, then explode to deal (%d+) physical damage to surrounding enemies%.", "Снимает с себя эффекты контроля, оставляет на своей позиции Замену бумажного человечка, перенимающую метки захвата цели, и совершает прыжок по направлению джойстика (по умолчанию назад). При применении дает %1 сек. Суперброни и на %2 сек. эффект %3. Бумажный человечек автоматически проводит три базовые атаки по врагам, нанося соответственно %4 физ. урона, после чего взрывается, нанося %5 физ. урона окружающим врагам.")

        -- Метка одурачивания и Уязвимость
        m = m:gsub("<FaintYellow>Fooling Mark</>: After stacking (%d+) times, it detonates and is removed, dealing (.-) physical damage to the target from the caster%.", "<FaintYellow>Метка одурачивания</>: При %1 уровнях детонирует и снимается, нанося цели физ. урон от заклинателя.")
        m = m:gsub("Fooling Mark: After stacking (%d+) times, it detonates and is removed, dealing (.-) physical damage to the target from the caster%.", "Метка одурачивания: При %1 уровнях детонирует и снимается, нанося цели физ. урон от заклинателя.")
        m = m:gsub("<FaintYellow>Vulnerability</>: Damage taken from the caster is increased by (%d+)%%%.?", "<FaintYellow>Уязвимость</>: Получаемый от заклинателя урон увеличен на %1%%.")
        m = m:gsub("Vulnerability: Damage taken from the caster is increased by (%d+)%%%.?", "Уязвимость: Получаемый от заклинателя урон увеличен на %1%%.")

        -- Специфические фразы механик Шута и Провидца
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points%., lasts for (%d+) seconds", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед., длится %3 сек.")
        m = m:gsub("Increase your Physical Damage Boost by ([%d%.]+)%% and Pierce by (%d+) points", "Повышает ваш физ. урон на %1%% и пробивание на %2 ед.")
        m = m:gsub("lasts for (%d+) seconds", "длится %1 сек.")
        m = m:gsub("simultaneously gaining (%d+) point of Card Energy and one Spirituality Blue Card", "одновременно даруя %1 очко Энергии карт и одну Синюю карту духовности")
        m = m:gsub("simultaneously gaining (%d+) point of <HighLight>Card Energy</> and one <HighLight>Spirituality Blue Card</>", "одновременно даруя %1 очко <HighLight>Энергии карт</> и одну <HighLight>Синюю карту духовности</>")
        m = m:gsub("simultaneously gaining (%d+) point of <HighLight>Card Energy</HighLight> and one <HighLight>Spirituality Blue Card</HighLight>", "одновременно даруя %1 очко <HighLight>Энергии карт</HighLight> и одну <HighLight>Синюю карту духовности</HighLight>")
        m = m:gsub("Gain one point of <HighLight>Card Energy</> after casting", "Дает 1 очко <HighLight>Энергии карт</> после применения")
        m = m:gsub("Gain one point of Card Energy after casting", "Дает 1 очко Энергии карт после применения")
        m = m:gsub("Fate Yellow Card/Spirituality Blue Card", "Жёлтая карта судьбы / Синяя карта духовности")
        m = m:gsub("Fate Yellow Cards/Spirituality Blue Cards", "Жёлтых карт судьбы / Синих карт духовности")
        m = m:gsub("Fooling of Fate Yellow Card/Spirituality Blue Card", "Жёлтая карта судьбы / Синяя карта духовности")
        m = m:gsub("Fooling of Fate Yellow Cards/Spirituality Blue Cards", "Жёлтых карт судьбы / Синих карт духовности")
        m = m:gsub("Fooling of Fate Yellow Card", "Жёлтая карта судьбы")
        m = m:gsub("Fooling of Fate Yellow Cards", "Жёлтые карты судьбы")
        m = m:gsub("Spirituality Blue Card", "Синяя карта духовности")
        m = m:gsub("Spirituality Blue Cards", "Синие карты духовности")
        m = m:gsub("Card Energy: Can hold up to (%d+) points", "Энергия карт: Вмещает до %1 очков")
        m = m:gsub("when reaching (%d+) points, the Finisher Skill switches and locks to Shuffle Cards", "при достижении %1 очков Добивающий навык переключается и фиксируется на Тасование карт")
        m = m:gsub("When reaching (%d+) points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>", "При %1 очках <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>")
        m = m:gsub("when reaching (%d+) points, the <HighLight>Finisher Skill</> switches and locks to <HighLight>Shuffle Cards</>", "при достижении %1 очков <HighLight>Добивающий навык</> переключается и фиксируется на <HighLight>Тасование карт</>")
        m = m:gsub("Finisher Skill Shuffle Cards", "Добивание: Тасование карт")
        m = m:gsub("Finisher Skill Shuffle", "Добивание: Тасование карт")
        m = m:gsub("Fooling of Fate/Spirituality Burst", "Одурачивание судьбы / Всплеск духовности")
        m = m:gsub("Fooling Mark", "Метка одурачивания")
        m = m:gsub("Spirituality Burst", "Всплеск духовности")
        m = m:gsub("Fooling of Fate", "Одурачивание судьбы")
        m = m:gsub("Shuffle Cards", "Тасование карт")
        m = m:gsub("Super Armor", "Суперброня")
        m = m:gsub("Air Bullet", "Воздушная пуля")
        m = m:gsub("Air Bullets", "Воздушные пули")
        m = m:gsub("Tarot Array", "Таро-расклад")
        m = m:gsub("Reveal Card", "Раскрытие карты")
        m = m:gsub("Miracle Card Rain", "Дождь карт чудес")
        m = m:gsub("Flame Jump", "Прыжок пламени")
        m = m:gsub("Paper Figurine Substitute", "Замена бумажного человечка")
        m = m:gsub("All Путь skills level up together%. Gain extra Skill Points through %[Sequence Advancement%]%.", "Все навыки Пути прокачиваются вместе. Получайте доп. очки навыков за [Продвижение по Последовательностям].")
        m = m:gsub("Путь skills level up together%. Gain extra Skill Points through %[Sequence Advancement%]%.", "Все навыки Пути прокачиваются вместе. Получайте доп. очки навыков за [Продвижение по Последовательностям].")
        m = m:gsub("All Path skills level up together%. Gain extra Skill Points through %[Sequence Advancement%]%.", "Все навыки Пути прокачиваются вместе. Получайте доп. очки навыков за [Продвижение по Последовательностям].")

        if m ~= text then
            return m
        end
    end

    return nil
end

return Russian