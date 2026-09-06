-- English to Russian UI and System translation module for Lord of Mysteries (C7)
-- Matches static English text from BakedText/cpdd_translation and live UMG widgets.

local M = {}

local exact = {
    -- System & Main Menu
    ["Return to Game"] = "Вернуться в игру",
    ["Back to Game"] = "Вернуться в игру",
    ["Resume Game"] = "Продолжить игру",
    ["Resume"] = "Продолжить",
    ["Quit Game"] = "Выйти из игры",
    ["Exit Game"] = "Выйти из игры",
    ["Leave Game"] = "Выйти из игры",
    ["Quit"] = "Выход",
    ["Exit"] = "Выход",
    ["Settings"] = "Настройки",
    ["Setting"] = "Настройки",
    ["Options"] = "Настройки",
    ["System Settings"] = "Системные настройки",
    ["Game Settings"] = "Настройки игры",
    ["Display Settings"] = "Настройки экрана",
    ["Graphics Settings"] = "Настройки графики",
    ["Audio Settings"] = "Настройки звука",
    ["Sound Settings"] = "Настройки звука",
    ["Control Settings"] = "Настройки управления",
    ["Language Settings"] = "Настройки языка",
    ["Account Settings"] = "Настройки аккаунта",
    ["Switch Account"] = "Сменить аккаунт",
    ["Change Account"] = "Сменить аккаунт",
    ["Switch Character"] = "Сменить персонажа",
    ["Change Character"] = "Сменить персонажа",
    ["Log Out"] = "Выйти из аккаунта",
    ["Logout"] = "Выйти из аккаунта",
    ["Enter World"] = "Войти в мир",
    ["Start Game"] = "Начать игру",

    -- Esc Menu 4-Row confirmed items
    ["Explore"] = "Исследование",
    ["Archive"] = "Архив",
    ["Style"] = "Стиль",
    ["Puppets"] = "Марионетки",
    ["Story"] = "История",
    ["Contacts"] = "Контакты",
    ["Easy Wins"] = "Быстрые победы",
    ["Gear"] = "Снаряжение",
    ["Artifacts"] = "Артефакты",
    ["Artifact"] = "Артефакт",
    ["Warfront"] = "Фронт",
    ["Dark City"] = "Темный город",
    ["Advance"] = "Продвижение",
    ["Arts"] = "Искусства",
    ["Talents"] = "Таланты",
    ["Talent"] = "Талант",
    ["Codex"] = "Справочник",
    ["Comments"] = "Комментарии",
    ["Comment"] = "Комментарий",
    ["Skip"] = "Пропуск",
    ["Review"] = "Повтор",
    ["Beyonder Rating"] = "Рейтинг Потустороннего",
    ["Recommended Beyonder Rating"] = "Рек. рейтинг Потустороннего",
    ["Reward Preview"] = "Предпросмотр наград",
    ["Preview"] = "Предпросмотр",
    ["Claimed"] = "Получено",
    ["Claim"] = "Забрать",
    ["Claim All"] = "Забрать все",
    ["Received"] = "Получено",
    ["Receive"] = "Получить",
    ["Reward"] = "Награда",
    ["Rewards"] = "Награды",
    ["Use"] = "Использовать",
    ["In Use"] = "Используется",
    ["Auto-Dismantle Settings"] = "Настройки авторазбора",
    ["Auto-Dismantle Confirmation"] = "Подтверждение авторазбора",
    ["Official Recommended Build"] = "Официальная сборка",
    ["Recommended Builds"] = "Рекомендованные сборки",
    ["Recommended Build"] = "Рекомендованная сборка",
    ["Equipment Builds"] = "Сборки снаряжения",
    ["Equipment Build"] = "Сборка снаряжения",
    ["My Builds"] = "Мои сборки",
    ["My Build"] = "Моя сборка",
    ["Auto-Dismantle"] = "Авторазбор",

    -- Buttons & Actions
    ["Confirm"] = "Подтвердить",
    ["Cancel"] = "Отмена",
    ["Close"] = "Закрыть",
    ["Back"] = "Назад",
    ["Next"] = "Далее",
    ["Previous"] = "Назад",
    ["Prev"] = "Назад",
    ["Save"] = "Сохранить",
    ["Save Changes"] = "Сохранить изменения",
    ["Apply"] = "Применить",
    ["OK"] = "ОК",
    ["Ok"] = "ОК",
    ["Yes"] = "Да",
    ["No"] = "Нет",
    ["Auto"] = "Авто",
    ["Manual"] = "Вручную",
    ["Select"] = "Выбрать",
    ["Selected"] = "Выбрано",
    ["Select All"] = "Выбрать все",
    ["Deselect"] = "Отменить выбор",
    ["Clear"] = "Очистить",
    ["Clear All"] = "Очистить все",
    ["Delete"] = "Удалить",
    ["Remove"] = "Убрать",
    ["Discard"] = "Выбросить",
    ["Equip"] = "Экипировать",
    ["Equipped"] = "Экипировано",
    ["Unequip"] = "Снять",
    ["Replace"] = "Заменить",
    ["Swap"] = "Поменять",
    ["Upgrade"] = "Улучшить",
    ["Enhance"] = "Усилить",
    ["Refine"] = "Закалить",
    ["Promote"] = "Повысить",
    ["Ascend"] = "Возвысить",
    ["Breakthrough"] = "Прорыв",
    ["Synthesize"] = "Синтез",
    ["Craft"] = "Создать",
    ["Forge"] = "Ковка",
    ["Smelt"] = "Переплавка",
    ["Decompose"] = "Разобрать",
    ["Dismantle"] = "Разобрать",
    ["Sell"] = "Продать",
    ["Buy"] = "Купить",
    ["Purchase"] = "Купить",
    ["Refresh"] = "Обновить",
    ["Reset"] = "Сбросить",
    ["Default"] = "По умолчанию",
    ["Restore Defaults"] = "Сбросить настройки",
    ["Reset to Default"] = "По умолчанию",
    ["Lock"] = "Заблокировать",
    ["Unlock"] = "Разблокировать",
    ["Locked"] = "Заблокировано",
    ["Unlocked"] = "Разблокировано",
    ["Share"] = "Поделиться",
    ["Copy"] = "Копировать",
    ["Paste"] = "Вставить",
    ["Search"] = "Поиск",
    ["Filter"] = "Фильтр",
    ["Sort"] = "Сортировка",
    ["View"] = "Просмотр",
    ["Check"] = "Проверить",
    ["Details"] = "Подробнее",
    ["Detail"] = "Подробнее",
    ["Info"] = "Инфо",
    ["Help"] = "Помощь",
    ["Tips"] = "Советы",
    ["Tip"] = "Совет",
    ["Guide"] = "Руководство",
    ["Tutorial"] = "Обучение",
    ["Rules"] = "Правила",
    ["Rule"] = "Правило",
    ["Notice"] = "Объявление",
    ["Notices"] = "Объявления",
    ["Announcement"] = "Объявление",
    ["Announcements"] = "Объявления",
    ["Click blank area to close"] = "Нажмите на пустую область, чтобы закрыть",
    ["Click anywhere to close"] = "Нажмите в любом месте, чтобы закрыть",
    ["Tap anywhere to continue"] = "Нажмите в любом месте, чтобы продолжить",
    ["Click anywhere to continue"] = "Нажмите в любом месте, чтобы продолжить",
    ["Press any key to continue"] = "Нажмите любую клавишу, чтобы продолжить",

    -- Character & Attributes
    ["Character"] = "Персонаж",
    ["Player"] = "Игрок",
    ["Player Details"] = "Данные игрока",
    ["Profile"] = "Профиль",
    ["Attributes"] = "Характеристики",
    ["Attribute"] = "Характеристика",
    ["Stats"] = "Характеристики",
    ["Stat"] = "Характеристика",
    ["Basic Attributes"] = "Базовые характеристики",
    ["Advanced Attributes"] = "Продвинутые характеристики",
    ["Combat Attributes"] = "Боевые характеристики",
    ["Special Attributes"] = "Особые характеристики",
    ["Appearance"] = "Облик",
    ["Fashion"] = "Стиль",
    ["Costume"] = "Костюм",
    ["Dye"] = "Окраска",
    ["Title"] = "Титул",
    ["Titles"] = "Титулы",
    ["Badge"] = "Значок",
    ["Badges"] = "Значки",
    ["Honor"] = "Честь",
    ["Fame"] = "Слава",
    ["Reputation"] = "Репутация",
    ["Level"] = "Уровень",
    ["Lv."] = "Ур.",
    ["Lv"] = "Ур.",
    ["EXP"] = "Опыт",
    ["Exp"] = "Опыт",
    ["Experience"] = "Опыт",
    ["HP"] = "ОЗ",
    ["MP"] = "ОМ",
    ["SP"] = "ОД",
    ["CD"] = "КД",
    ["Max HP"] = "Макс. ОЗ",
    ["Max MP"] = "Макс. ОМ",
    ["Max SP"] = "Макс. ОД",
    ["Health"] = "Здоровье",
    ["Mana"] = "Мана",
    ["Stamina"] = "Выносливость",
    ["Energy"] = "Энергия",
    ["ATK"] = "Атака",
    ["Attack"] = "Атака",
    ["DEF"] = "Защита",
    ["Defense"] = "Защита",
    ["Physical ATK"] = "Физ. атака",
    ["Magic ATK"] = "Маг. атака",
    ["Physical Attack"] = "Физ. атака",
    ["Magic Attack"] = "Маг. атака",
    ["Physical DEF"] = "Физ. защита",
    ["Magic DEF"] = "Маг. защита",
    ["Physical Defense"] = "Физ. защита",
    ["Magic Defense"] = "Маг. защита",
    ["Crit"] = "Крит",
    ["Crit Rate"] = "Шанс крита",
    ["Critical Rate"] = "Шанс крита",
    ["Crit DMG"] = "Крит. урон",
    ["Crit Damage"] = "Крит. урон",
    ["Critical Damage"] = "Крит. урон",
    ["Armor Break"] = "Пробивание брони",
    ["Defense Break"] = "Снижение защиты",
    ["Physical Defense Break"] = "Снижение физ. защиты",
    ["Magic Defense Break"] = "Снижение маг. защиты",
    ["Skill Block"] = "Блокирование навыков",
    ["Physical Block"] = "Физ. блок",
    ["Magical Block"] = "Маг. блок",
    ["Block"] = "Блок",
    ["Block Rate"] = "Шанс блока",
    ["Accuracy"] = "Меткость",
    ["Hit Rate"] = "Шанс попадания",
    ["Hit"] = "Попадание",
    ["Evasion"] = "Уклонение",
    ["Dodge"] = "Уклонение",
    ["Dodge Rate"] = "Шанс уклонения",
    ["Speed"] = "Скорость",
    ["Move Speed"] = "Скорость бега",
    ["Movement Speed"] = "Скорость передвижения",
    ["Attack Speed"] = "Скорость атаки",
    ["Cooldown"] = "Перезарядка",
    ["Cooldown Reduction"] = "Снижение КД",
    ["Might"] = "Сила",
    ["Intuition"] = "Интуиция",
    ["Agility"] = "Ловкость",
    ["Spirituality"] = "Духовность",
    ["Physique"] = "Телосложение",
    ["Intelligence"] = "Интеллект",
    ["Cleanse"] = "Очищение",
    ["Cleanse Skill"] = "Навык снятия контроля",
    ["Crowd Control"] = "Контроль",
    ["Damage"] = "Урон",
    ["Damage Dealt"] = "Наносимый урон",
    ["Damage Taken"] = "Получаемый урон",
    ["Healing"] = "Исцеление",
    ["Heal"] = "Исцеление",
    ["Shield"] = "Щит",
    ["Immunity"] = "Иммунитет",
    ["Immune"] = "Иммунитет",
    ["Resistance"] = "Сопротивление",
    ["Resist"] = "Сопротивление",

    -- Inventory & Items
    ["Inventory"] = "Инвентарь",
    ["Bag"] = "Сумка",
    ["Backpack"] = "Рюкзак",
    ["Warehouse"] = "Склад",
    ["Storage"] = "Хранилище",
    ["Equipment"] = "Экипировка",
    ["Weapon"] = "Оружие",
    ["Weapons"] = "Оружие",
    ["Armor"] = "Броня",
    ["Accessory"] = "Аксессуар",
    ["Accessories"] = "Аксессуары",
    ["Ring"] = "Кольцо",
    ["Necklace"] = "Ожерелье",
    ["Brooch"] = "Брошь",
    ["Helmet"] = "Шлем",
    ["Chest"] = "Нагрудник",
    ["Gloves"] = "Перчатки",
    ["Boots"] = "Сапоги",
    ["Belt"] = "Пояс",
    ["Pants"] = "Штаны",
    ["Cloak"] = "Плащ",
    ["Sealed Artifact"] = "Запечатанный Артефакт",
    ["Sealed Artifacts"] = "Запечатанные Артефакты",
    ["Item"] = "Предмет",
    ["Items"] = "Предметы",
    ["Item Level"] = "Уровень предмета",
    ["Quality"] = "Качество",
    ["Rarity"] = "Редкость",
    ["Common"] = "Обычный",
    ["Uncommon"] = "Необычный",
    ["Rare"] = "Редкий",
    ["Epic"] = "Эпический",
    ["Legendary"] = "Легендарный",
    ["Mythic"] = "Мифический",
    ["Mythical"] = "Мифический",
    ["Consumables"] = "Расходники",
    ["Consumable"] = "Расходник",
    ["Materials"] = "Материалы",
    ["Material"] = "Материал",
    ["Currencies"] = "Валюты",
    ["Currency"] = "Валюта",
    ["Quest Items"] = "Квестовые предметы",
    ["Quest Item"] = "Квестовый предмет",
    ["Quantity"] = "Количество",
    ["Amount"] = "Количество",
    ["Owned"] = "В наличии",
    ["Remaining"] = "Осталось",
    ["Time Remaining"] = "Осталось времени",
    ["Gold"] = "Золото",
    ["Coins"] = "Монеты",
    ["Diamonds"] = "Алмазы",
    ["Pounds"] = "Фунты",
    ["Soles"] = "Су",
    ["Pennies"] = "Пенни",
    ["Price"] = "Цена",
    ["Cost"] = "Стоимость",
    ["Free"] = "Бесплатно",
    ["Sale"] = "Скидка",
    ["Discount"] = "Скидка",

    -- Skills & Talents
    ["Skills"] = "Навыки",
    ["Skill"] = "Навык",
    ["Combat Skills"] = "Боевые навыки",
    ["Special Skills"] = "Особые навыки",
    ["Acting Skills"] = "Навыки актерства",
    ["Passive Skill"] = "Пассивный навык",
    ["Passive Skills"] = "Пассивные навыки",
    ["Active Skill"] = "Активный навык",
    ["Active Skills"] = "Активные навыки",
    ["Basic Attack"] = "Базовая атака",
    ["Crowd-Control Break"] = "Снятие контроля",
    ["Finisher Skill"] = "Завершающий навык",
    ["Finisher Skills"] = "Завершающие навыки",
    ["Talents"] = "Таланты",
    ["Talent"] = "Талант",
    ["Talent Tree"] = "Дерево талантов",
    ["Puppets"] = "Марионетки",
    ["Marionette"] = "Марионетка",
    ["Marionettes"] = "Марионетки",
    ["Duration"] = "Длительность",
    ["Range"] = "Дальность",
    ["Radius"] = "Радиус",

    -- Map, Quests & Social
    ["Map"] = "Карта",
    ["Minimap"] = "Миникарта",
    ["Mini Map"] = "Миникарта",
    ["World Map"] = "Карта мира",
    ["Area Map"] = "Карта области",
    ["Location"] = "Местоположение",
    ["Navigation"] = "Навигация",
    ["Teleport"] = "Телепорт",
    ["Waypoint"] = "Точка перемещения",
    ["Quests"] = "Задания",
    ["Quest"] = "Задание",
    ["Tasks"] = "Задания",
    ["Task"] = "Задание",
    ["Missions"] = "Миссии",
    ["Mission"] = "Миссия",
    ["Main Quest"] = "Основное задание",
    ["Side Quest"] = "Побочное задание",
    ["Daily Quest"] = "Ежедневное задание",
    ["Weekly Quest"] = "Еженедельное задание",
    ["Complete"] = "Завершено",
    ["Completed"] = "Завершено",
    ["In Progress"] = "В процессе",
    ["Not Started"] = "Не начато",
    ["Abandon"] = "Отказаться",
    ["Track"] = "Отслеживать",
    ["Untrack"] = "Не отслеживать",
    ["Shop"] = "Магазин",
    ["Mall"] = "Магазин",
    ["Store"] = "Магазин",
    ["Market"] = "Рынок",
    ["Auction"] = "Аукцион",
    ["Auction House"] = "Аукционный дом",
    ["Black Market"] = "Черный рынок",
    ["Mail"] = "Почта",
    ["Mailbox"] = "Почтовый ящик",
    ["Inbox"] = "Входящие",
    ["Messages"] = "Сообщения",
    ["System Mail"] = "Системная почта",
    ["Friends"] = "Друзья",
    ["Friend"] = "Друг",
    ["Friend List"] = "Список друзей",
    ["Add Friend"] = "Добавить друга",
    ["Delete Friend"] = "Удалить друга",
    ["Blacklist"] = "Черный список",
    ["Block"] = "Заблокировать",
    ["Unblock"] = "Разблокировать",
    ["Team"] = "Группа",
    ["Party"] = "Отряд",
    ["Create Team"] = "Создать группу",
    ["Join Team"] = "Вступить в группу",
    ["Leave Team"] = "Покинуть группу",
    ["Disband Team"] = "Распустить группу",
    ["Kick"] = "Исключить",
    ["Invite"] = "Пригласить",
    ["Matchmaking"] = "Подбор",
    ["Auto Match"] = "Автоподбор",
    ["Guild"] = "Гильдия",
    ["Family"] = "Семья",
    ["Family Chief"] = "Глава семьи",
    ["Create Family"] = "Создать семью",
    ["Join Family"] = "Вступить в семью",
    ["Family Application"] = "Заявка в семью",
    ["Recruitment Response"] = "Отклик на набор",
    ["Chat"] = "Чат",
    ["World Chat"] = "Мировой чат",
    ["Team Chat"] = "Чат группы",
    ["Party Chat"] = "Чат отряда",
    ["Guild Chat"] = "Чат гильдии",
    ["Family Chat"] = "Чат семьи",
    ["Current Chat"] = "Текущий чат",
    ["Nearby Chat"] = "Чат рядом",
    ["Whisper"] = "Личные сообщения",
    ["System Chat"] = "Системный чат",
    ["Rankings"] = "Рейтинги",
    ["Ranking"] = "Рейтинг",
    ["Rank"] = "Ранг",
    ["Leaderboard"] = "Таблица лидеров",
    ["Leaderboards"] = "Таблицы лидеров",

    -- Settings: Display, Audio, Controls
    ["Graphics"] = "Графика",
    ["Display"] = "Экран",
    ["Audio"] = "Звук",
    ["Sound"] = "Звук",
    ["Controls"] = "Управление",
    ["Language"] = "Язык",
    ["Resolution"] = "Разрешение",
    ["Window Mode"] = "Режим экрана",
    ["Fullscreen"] = "Полноэкранный",
    ["Borderless"] = "В окне без рамки",
    ["Borderless Window"] = "В окне без рамки",
    ["Windowed"] = "Оконный",
    ["Frame Rate"] = "Частота кадров",
    ["Frame Rate Limit"] = "Лимит кадров",
    ["FPS Limit"] = "Лимит кадров",
    ["V-Sync"] = "Вертикальная синхронизация",
    ["VSync"] = "Вертикальная синхронизация",
    ["Anti-Aliasing"] = "Сглаживание",
    ["Shadow Quality"] = "Качество теней",
    ["Shadows"] = "Тени",
    ["Texture Quality"] = "Качество текстур",
    ["Textures"] = "Текстуры",
    ["Effect Quality"] = "Качество эффектов",
    ["Effects"] = "Эффекты",
    ["Post-Processing"] = "Постобработка",
    ["View Distance"] = "Дальность прорисовки",
    ["Brightness"] = "Яркость",
    ["Contrast"] = "Контраст",
    ["Volume"] = "Громкость",
    ["Master Volume"] = "Общая громкость",
    ["Music Volume"] = "Громкость музыки",
    ["BGM Volume"] = "Громкость музыки",
    ["SFX Volume"] = "Громкость эффектов",
    ["Sound Effects Volume"] = "Громкость звуковых эффектов",
    ["Voice Volume"] = "Громкость голосов",
    ["UI Volume"] = "Громкость интерфейса",
    ["Mouse Sensitivity"] = "Чувствительность мыши",
    ["Invert Y-Axis"] = "Инверсия оси Y",
    ["Invert X-Axis"] = "Инверсия оси X",
    ["Key Bindings"] = "Назначение клавиш",
    ["Keybinds"] = "Клавиши",
    ["Low"] = "Низкое",
    ["Medium"] = "Среднее",
    ["High"] = "Высокое",
    ["Very High"] = "Очень высокое",
    ["Ultra"] = "Ультра",
    ["Custom"] = "Пользовательское",
    ["On"] = "Вкл.",
    ["Off"] = "Выкл.",
    ["Enabled"] = "Включено",
    ["Disabled"] = "Отключено",

    -- Canon Glossary: Pathways, Sequences, Lore
    ["Beyonder"] = "Потусторонний",
    ["Pathway"] = "Путь",
    ["Sequence"] = "Последовательность",
    ["Acting"] = "Актерство",
    ["Digestion"] = "Усвоение",
    ["Lose Control"] = "Потеря контроля",
    ["Loss of Control"] = "Потеря контроля",
    ["Corruption"] = "Осквернение",
    ["Madness"] = "Безумие",
    ["Divination"] = "Гадание",
    ["Astrology"] = "Астрология",
    ["Spirit Vision"] = "Духовное зрение",
    ["Dream Divination"] = "Гадание во сне",
    ["Cogitation"] = "Медитация",
    ["Demigod"] = "Полубог",
    ["Saint"] = "Святой",
    ["Angel"] = "Ангел",
    ["King of Angels"] = "Король Ангелов",
    ["True God"] = "Истинный Бог",
    ["Above the Sequences"] = "Выше Последовательностей",
    ["Great Old One"] = "Великое Древнее Существо",
    ["Outer Deity"] = "Внешнее Божество",
    ["Pillar"] = "Столп",
    ["Sefirah Castle"] = "Замок Сефир",
    ["Gray Fog"] = "Серый туман",
    ["Spirit World"] = "Мир духов",
    ["Astral World"] = "Астральный мир",
    ["Underworld"] = "Подземный мир",
    ["Abyss"] = "Бездна",
    ["Cosmos"] = "Космос",
    ["Tingen"] = "Тинген",
    ["Backlund"] = "Баклунд",
    ["Blackthorn Security Company"] = "Охранная компания «Терновник»",
    ["Nighthawks"] = "Ночные Ястребы",
    ["Mandated Punishers"] = "Уполномоченные Каратели",
    ["Machinery Hivemind"] = "Механический Коллективный Разум",
    ["Tarot Club"] = "Клуб Таро",

    -- Sequences of Seer / Fool pathway
    ["The Fool"] = "Шут",
    ["\"The Fool\""] = "«Шут»",
    ["Fool"] = "Шут",
    ["Seer"] = "Провидец",
    ["Clown"] = "Клоун",
    ["Magician"] = "Фокусник",
    ["Faceless"] = "Безликий",
    ["Marionettist"] = "Марионеточник",
    ["Bizarro Sorcerer"] = "Маг Безрассудства",
    ["Scholar of Yore"] = "Знаток Прошлого",
    ["Miracle Invoker"] = "Творец Чудес",
    ["Attendant of Mysteries"] = "Повелитель Тайн",
    ["Lord of Mysteries"] = "Повелитель Тайн",
    ["Lord of the Mysteries"] = "Повелитель Тайн",

    -- Other Pathways & Tarots
    ["Door"] = "Дверь",
    ["\"Door\""] = "«Дверь»",
    ["Error"] = "Отказ",
    ["Spectator"] = "Зритель",
    ["Telepathist"] = "Телепат",
    ["Psyche Analyst"] = "Психоаналитик",
    ["Hypnotist"] = "Гипнотизер",
    ["Dreamwalker"] = "Сновидец",
    ["Manipulator"] = "Манипулятор",
    ["Justice"] = "Справедливость",
    ["Miss Justice"] = "Мисс Справедливость",
    ["The Hanged Man"] = "Повешенный",
    ["The Sun"] = "Солнце",
    ["The World"] = "Мир",
    ["The Moon"] = "Луна",
    ["The Star"] = "Звезда",
    ["The Hermit"] = "Отшельник",
    ["The Tower"] = "Башня",
    ["The Chariot"] = "Колесница",
    ["The Lovers"] = "Влюбленные",
    ["The Empress"] = "Императрица",
    ["The Emperor"] = "Император",
    ["The Hierophant"] = "Жрец",
    ["The High Priestess"] = "Верховная Жрица",
    ["Wheel of Fortune"] = "Колесо Фортуны",
    ["Sailor"] = "Моряк",
    ["Hunter"] = "Охотник",
    ["Warrior"] = "Воин",
    ["Sleepless"] = "Бессонный",
    ["Corpse Collector"] = "Сборщик Трупов",
    ["Mystery Pryer"] = "Жрец Тайн",
    ["Secret Supplicant"] = "Тайный Молитель",
    ["Monster"] = "Монстр",
    ["Apothecary"] = "Аптекарь",
    ["Planter"] = "Садовник",
    ["Assassin"] = "Ассасин",
    ["Criminal"] = "Преступник",
    ["Prisoner"] = "Узник",
    ["Arbiter"] = "Арбитр",
    ["Lawyer"] = "Адвокат",
    ["Reader"] = "Чтец",
    ["Savant"] = "Эрудит",
    ["Tinkerer"] = "Механик",

    -- Status & Messages
    ["Loading..."] = "Загрузка...",
    ["Please wait..."] = "Пожалуйста, подождите...",
    ["Success"] = "Успешно",
    ["Failed"] = "Ошибка",
    ["Error"] = "Ошибка",
    ["Warning"] = "Внимание",
    ["Connected"] = "Подключено",
    ["Connecting..."] = "Подключение...",
    ["Disconnected"] = "Отключено",
    ["Connection Failed"] = "Ошибка соединения",
    ["Connection Timeout"] = "Тайм-аут соединения",
    ["Network error"] = "Ошибка сети",
    ["Connection lost"] = "Соединение разорвано",
    ["Reconnect"] = "Переподключиться",
    ["Reconnecting..."] = "Переподключение...",
    ["Operation successful."] = "Операция выполнена успешно.",
    ["Operation failed."] = "Ошибка выполнения операции.",
    ["Purchase successful."] = "Покупка совершена успешно.",
    ["Upgrade successful!"] = "Улучшение успешно!",
    ["Upgrade failed."] = "Не удалось улучшить.",
    ["Changes saved successfully."] = "Изменения успешно сохранены.",
    ["Not enough inventory space."] = "Недостаточно места в инвентаре.",
    ["Not enough gold."] = "Недостаточно золота.",
    ["Not enough materials."] = "Недостаточно материалов.",
    ["Level requirement not met."] = "Требуемый уровень не достигнут.",
}

M.exact = exact

function M.translate(value)
    if type(value) ~= "string" or value == "" then
        return nil
    end

    local direct = exact[value]
    if direct ~= nil then
        return direct
    end

    -- Check trimmed
    local leading, body, trailing = value:match("^(%s*)(.-)(%s*)$")
    if body and body ~= "" and body ~= value then
        local directBody = exact[body]
        if directBody ~= nil then
            return leading .. directBody .. trailing
        end
    end

    local text = body or value

    -- Tip prefix: "Tip: ..." or "Tip：..."
    local tipBody = text:match("^[Tt][Ii][Pp]:%s*(.+)$") or text:match("^[Tt][Ii][Pp]：%s*(.+)$")
    if tipBody ~= nil then
        local translatedTip = M.translate(tipBody) or tipBody
        return "Подсказка: " .. translatedTip
    end

    -- Dynamic patterns
    local beyonderRating = text:match("^Beyonder Rating%s*(.*)$")
    if beyonderRating ~= nil then
        return "Рейтинг Потустороннего " .. beyonderRating
    end

    local recBeyonderRating = text:match("^Recommended Beyonder Rating%s*(.*)$")
    if recBeyonderRating ~= nil then
        return "Рек. рейтинг Потустороннего " .. recBeyonderRating
    end

    local fashionVal = text:match("^Fashion Value:%s*(%d+)$") or text:match("^Fashion Value：%s*(%d+)$")
    if fashionVal ~= nil then
        return "Очки стиля: " .. fashionVal
    end

    local obtainQty = text:match("^Obtainable Quantity:%s*(%d+)$") or text:match("^Obtainable Quantity：%s*(%d+)$")
    if obtainQty ~= nil then
        return "Доступное количество: " .. obtainQty
    end

    local itemLvl = text:match("^Item Level%s*(%d+)$")
    if itemLvl ~= nil then
        return "Уровень предмета " .. itemLvl
    end

    local lvlNum = text:match("^Lv%.?%s*(%d+)$") or text:match("^Level%s*(%d+)$")
    if lvlNum ~= nil then
        return "Ур. " .. lvlNum
    end

    local stackNum = text:match("^Stack%s*(%d+)$") or text:match("^(%d+)%s*stacks?$")
    if stackNum ~= nil then
        return stackNum .. " ур."
    end

    local timeUnlock = text:match("^Time until unlocked:%s*(.+)$")
    if timeUnlock ~= nil then
        return "До разблокировки: " .. timeUnlock
    end

    local selectedPattern = text:match("^Selected%s*(.+)$")
    if selectedPattern ~= nil then
        return "Выбрано " .. selectedPattern
    end

    local listingRem = text:match("^Listing period remaining:%s*(.+)$")
    if listingRem ~= nil then
        return "До конца огласки: " .. listingRem
    end

    local listingPeriod = text:match("^Listing period:%s*(.+)$")
    if listingPeriod ~= nil then
        return "Период огласки: " .. listingPeriod
    end

    local refreshDaily = text:match("^Refreshes daily at%s*(.+)$")
    if refreshDaily ~= nil then
        return "Ежедневное обновление в " .. refreshDaily
    end

    local complReward = text:match("^Completion Reward:%s*(.+)$")
    if complReward ~= nil then
        return "Награда за достижение: " .. complReward
    end

    local newMsgs = text:match("^(%d+)%s*new messages?$")
    if newMsgs ~= nil then
        return newMsgs .. " нов. сообщ."
    end

    local voiceChat = text:match("^<GreenVoice>(%d+)</>%s*people in voice chat%.%.%.$")
    if voiceChat ~= nil then
        return "<GreenVoice>" .. voiceChat .. "</> чел. в голосовом чате..."
    end

    local seqNum, seqName = text:match("^Sequence%s*(%d+):%s*(.+)$")
    if seqNum ~= nil then
        local trName = exact[seqName] or seqName
        return "Последовательность " .. seqNum .. ": " .. trName
    end

    local chapterNum = text:match("^Chapter%s*(%d+)$")
    if chapterNum ~= nil then
        return "Глава " .. chapterNum
    end

    local floorNum = text:match("^Floor%s*(%d+)$")
    if floorNum ~= nil then
        return "Этаж " .. floorNum
    end

    return nil
end

return M
