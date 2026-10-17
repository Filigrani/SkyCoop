using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SkyCoop
{
    // Mod UI language. Texts are written in English in code, Loc.T() swaps them for Russian when it is selected.
    // "Auto" follows the game language.
    public static class Loc
    {
        public static readonly string[] Modes = { "Auto", "English", "Russian" };

        private static MelonPreferences_Entry<string> LanguageEntry;

        public static string Mode
        {
            get
            {
                if (LanguageEntry == null)
                {
                    MelonPreferences_Category Cat = MelonPreferences.CreateCategory("SkyCoop", "Sky Co-op");
                    LanguageEntry = Cat.CreateEntry("Language", "Auto", "Mod language (Auto, English, Russian)");
                }
                return Array.IndexOf(Modes, LanguageEntry.Value) >= 0 ? LanguageEntry.Value : "Auto";
            }
            set
            {
                string unused = Mode;
                LanguageEntry.Value = value;
                MelonPreferences.Save();
            }
        }

        public static bool IsRussian
        {
            get
            {
                string M = Mode;
                if (M == "Auto")
                {
                    return PlayerPrefs.GetString("Language", "English") == "Russian";
                }
                return M == "Russian";
            }
        }

        public static void CycleMode()
        {
            Mode = Modes[(Array.IndexOf(Modes, Mode) + 1) % Modes.Length];
        }

        public static string ModeButtonText()
        {
            string M = Mode;
            string Name;
            if (M == "English")
            {
                Name = "ENGLISH";
            }
            else if (M == "Russian")
            {
                Name = "РУССКИЙ";
            }else{
                Name = IsRussian ? "АВТО (РУССКИЙ)" : "AUTO (ENGLISH)";
            }
            return (IsRussian ? "ЯЗЫК: " : "LANGUAGE: ") + Name;
        }

        public static string T(string en)
        {
            if (string.IsNullOrEmpty(en) || !IsRussian)
            {
                return en;
            }
            string Core = en.Trim();
            if (Core.Length == 0)
            {
                return en;
            }
            int Start = en.IndexOf(Core, StringComparison.Ordinal);
            string Lead = en.Substring(0, Start);
            string Tail = en.Substring(Start + Core.Length);

            if (Exact.TryGetValue(Core, out string ru))
            {
                return Lead + ru + Tail;
            }
            foreach (KeyValuePair<string, string> P in Prefixes)
            {
                if (Core.StartsWith(P.Key, StringComparison.Ordinal))
                {
                    return Lead + P.Value + Core.Substring(P.Key.Length) + Tail;
                }
            }
            foreach (KeyValuePair<string, string> P in Suffixes)
            {
                if (Core.EndsWith(P.Key, StringComparison.Ordinal))
                {
                    return Lead + Core.Substring(0, Core.Length - P.Key.Length) + P.Value + Tail;
                }
            }
            foreach (KeyValuePair<string, string> P in Infixes)
            {
                int i = Core.IndexOf(P.Key, StringComparison.Ordinal);
                if (i > 0)
                {
                    return Lead + Core.Substring(0, i) + P.Value + Core.Substring(i + P.Key.Length) + Tail;
                }
            }
            return en;
        }

        private static readonly Dictionary<string, string> Exact = new Dictionary<string, string>
        {
            // Main menu and multiplayer menus
            { "MULTIPLAYER", "МУЛЬТИПЛЕЕР" },
            { "LOBBY", "ЛОББИ" },
            { "HOST SERVER", "СОЗДАТЬ СЕРВЕР" },
            { "JOIN SERVER", "ПОДКЛЮЧИТЬСЯ" },
            { "SETTINGS", "НАСТРОЙКИ" },
            { "BROWSE SERVERS", "СПИСОК СЕРВЕРОВ" },
            { "CONNECT BY IP", "ПОДКЛЮЧИТЬСЯ ПО IP" },
            { "CONNECT TO FRIEND", "ПОДКЛЮЧИТЬСЯ К ДРУГУ" },
            { "CHANGE NICKNAME", "СМЕНИТЬ НИК" },
            { "CUSTOMIZATION", "ВНЕШНИЙ ВИД" },
            { "COPY ACCOUNT ID", "КОПИРОВАТЬ ID АККАУНТА" },
            { "START SERVER", "ЗАПУСТИТЬ СЕРВЕР" },
            { "SELECT SAVE", "ВЫБРАТЬ СОХРАНЕНИЕ" },
            { "VOTE", "ГОЛОСОВАТЬ" },
            { "INVITE FRIEND", "ПРИГЛАСИТЬ ДРУГА" },
            { "COPY INVITE LINK", "КОПИРОВАТЬ ПРИГЛАШЕНИЕ" },
            { "NEW GAME", "НОВАЯ ИГРА" },
            { "LOAD SAVE", "ЗАГРУЗИТЬ СОХРАНЕНИЕ" },
            { "FEATS", "НАВЫКИ" },
            { "START VOTE", "НАЧАТЬ ГОЛОСОВАНИЕ" },
            { "CHOOSE MYSELF", "ВЫБРАТЬ САМОМУ" },
            { "REGION", "РЕГИОН" },
            { "EXPERIENCE MODE", "РЕЖИМ СЛОЖНОСТИ" },
            { "INVITE", "ПРИГЛАСИТЬ" },
            { "Customization", "Внешний вид" },

            // Menu descriptions
            { "People who supported the project.", "Люди, которые поддержали проект." },
            { "Host or join a multiplayer session.", "Создать сетевую игру или подключиться к ней." },
            { "Show lobby you are currently in.", "Показать лобби, в котором вы находитесь." },
            { "Invite your steam friend to this P2P session. Once you invite someone, they will join the lobby first and then will be connected to your server.", "Пригласить друга из Steam в эту P2P-сессию. Приглашённый сначала попадёт в лобби, а затем подключится к вашему серверу." },
            { "Configure and host a session.", "Настроить и запустить сервер." },
            { "Find a server or join by IP address.", "Найти сервер или подключиться по IP-адресу." },
            { "Join by IP address.", "Подключиться по IP-адресу." },
            { "Change your nickname and customize your appeal.", "Сменить ник и настроить внешний вид." },
            { "Browse public servers.", "Просмотреть публичные серверы." },
            { "Connect to a server by IP address.", "Подключиться к серверу по IP-адресу." },
            { "Opens steam friends overlay.", "Открыть список друзей Steam." },
            { "Change your character name. Other players will see it ingame.", "Сменить имя персонажа. Другие игроки увидят его в игре." },
            { "Toggle supporter bonuses and select flairs.", "Включить бонусы поддержавших проект и выбрать украшения." },
            { "Copy to clipboard EGS or Steam Account ID.", "Скопировать ID аккаунта EGS или Steam в буфер обмена." },
            { "Start your session if everyone is ready, or if you do not want to wait for other players.\n(Players will be able to join, even after the session is already started).", "Начать игру, если все готовы или вы не хотите ждать остальных.\n(Игроки смогут подключиться и после начала игры)." },
            { "Create or load a sandbox game.", "Создать или загрузить игру в режиме выживания." },
            { "Vote for session settings.", "Проголосовать за настройки игры." },
            { "Invite your steam friend to this lobby.", "Пригласить друга из Steam в это лобби." },
            { "Copy invite link to clipboard.", "Скопировать ссылку-приглашение в буфер обмена." },
            { "Start vote for session settings.", "Начать голосование за настройки игры." },
            { "Create new sandbox game without voting.", "Создать новую игру без голосования." },
            { "Vote for a starting region.", "Проголосовать за стартовый регион." },
            { "Vote for experience mode.", "Проголосовать за режим сложности." },
            { "Switch the mod language. Auto follows the game language.", "Сменить язык мода. «Авто» использует язык игры." },

            // Popups
            { "HOW DO YOU WANT TO BE CALLED?", "КАК ВАС НАЗЫВАТЬ?" },
            { "Your ID was copied to clipboard", "Ваш ID скопирован в буфер обмена" },
            { "Can't detect your account ID", "Не удалось определить ID аккаунта" },
            { "Link copied to clipboard.", "Ссылка скопирована в буфер обмена." },
            { "Can't join this lobby", "Не удаётся войти в это лобби" },
            { "Input server address", "Введите адрес сервера" },
            { "Connecting", "Подключение" },
            { "Trying to join...", "Пытаемся подключиться..." },
            { "Connecting...", "Подключение..." },
            { "Please wait...", "Подождите..." },
            { "Please wait..", "Подождите.." },
            { "Please wait a moment...", "Подождите немного..." },
            { "Calling to coordinator...", "Связываемся с координатором..." },
            { "Coordinating...", "Согласование..." },
            { "Downloading container data...", "Загрузка содержимого контейнера..." },
            { "Downloading gear...", "Загрузка предмета..." },
            { "Downloading scene drops...", "Загрузка предметов на локации..." },
            { "Locking door...", "Запираем дверь..." },
            { "Looking in pockets...", "Осматриваем карманы..." },
            { "Preparing to dismantle...", "Подготовка к разборке..." },
            { "Sending container data...", "Отправка содержимого контейнера..." },
            { "Trying open this door...", "Пытаемся открыть дверь..." },
            { "Trying unlock this door...", "Пытаемся отпереть дверь..." },
            { "Requesting data about blank...", "Запрос данных о заготовке..." },
            { "Diagnosis player", "Осмотр игрока" },
            { "Diagnosis failed!", "Осмотр не удался!" },
            { "Downloading Gear", "Загрузка предмета" },
            { "Downloading animal carcass", "Загрузка туши животного" },
            { "Looking through the peephole", "Смотрим в глазок" },
            { "Trying to pickup little thing", "Пытаемся подобрать предмет" },
            { "Host received invalid data", "Хост получил неверные данные" },
            { "Trying send data again...", "Повторная отправка данных..." },
            { "Saving", "Сохранение" },
            { "Saving before exit...", "Сохранение перед выходом..." },
            { "Verifying...", "Проверка..." },
            { "Host checking my save file...", "Хост проверяет файл сохранения..." },
            { "Request failed!", "Запрос не выполнен!" },
            { "Request failed", "Запрос не выполнен" },
            { "DISCONNECTED", "ОТКЛЮЧЕНО" },
            { "DISCONNECTED FROM SERVER", "ОТКЛЮЧЕНО ОТ СЕРВЕРА" },
            { "DISCONNECT TIMEOUT", "ВРЕМЯ ОЖИДАНИЯ ИСТЕКЛО" },
            { "ERROR", "ОШИБКА" },
            { "YOU ALREADY CONNECTED!!!!", "ВЫ УЖЕ ПОДКЛЮЧЕНЫ!" },
            { "YOU ALREADY ON SERVER", "ВЫ УЖЕ НА СЕРВЕРЕ" },
            { "Feedback form opened in your browser", "Форма обратной связи открыта в браузере" },
            { "NOTE MESSAGE", "ТЕКСТ ЗАПИСКИ" },
            { "Input name for key", "Введите название ключа" },
            { "Input secret seed for key", "Введите секретный код ключа" },
            { "INPUT ID OF PLAYER TELEPORT TO", "ВВЕДИТЕ ID ИГРОКА ДЛЯ ТЕЛЕПОРТА" },
            { "Invalid ID of player!", "Неверный ID игрока!" },
            { "You can't use custom experience mode in online! This case to major desync!", "Нельзя использовать свой режим сложности в сетевой игре! Это приведёт к сильной рассинхронизации!" },
            { "Press Esc to Exit", "Нажмите Esc для выхода" },

            // HUD messages
            { "THIS ALREADY PICKED!", "ЭТО УЖЕ ПОДОБРАНО!" },
            { "YOU CAN'T DROP THIS", "ЭТО НЕЛЬЗЯ ВЫБРОСИТЬ" },
            { "Can't interact with this", "С этим нельзя взаимодействовать" },
            { "Can't pick this up", "Это нельзя подобрать" },
            { "Nothing to harvest", "Нечего собирать" },
            { "Rock cache should be empty!", "Тайник должен быть пустым!" },
            { "Someone already work on this!", "Кто-то уже этим занимается!" },
            { "No body knocking.", "Никто не стучит." },
            { "Somebody's knocking on the door", "Кто-то стучит в дверь" },
            { "Incorrect key!", "Неподходящий ключ!" },
            { "The key broke!", "Ключ сломался!" },
            { "This door can't be locked!", "Эту дверь нельзя запереть!" },
            { "This door is already locked!", "Эта дверь уже заперта!" },
            { "This key already used for other door!", "Этот ключ уже используется для другой двери!" },
            { "You don't have any keys to lock this door!", "У вас нет ключей, чтобы запереть эту дверь!" },
            { "You already have technical backpack!", "У вас уже есть технический рюкзак!" },
            { "You and player on different scenes!", "Вы с игроком на разных локациях!" },
            { "You are entering safe zone!", "Вы входите в безопасную зону!" },
            { "You are leaving safe zone!", "Вы покидаете безопасную зону!" },
            { "You cannot attack players in the safe zone!", "Нельзя атаковать игроков в безопасной зоне!" },
            { "You cannot attack when you in the safe zone!", "Нельзя атаковать, находясь в безопасной зоне!" },
            { "You need hacksaw!", "Нужна ножовка!" },
            { "You need liquid lead!", "Нужен расплавленный свинец!" },
            { "You need whetstone!", "Нужен точильный камень!" },
            { "You have not any invites.", "У вас нет приглашений." },
            { "You don't have any [50C878]special items[-] to start advanced expedition.", "У вас нет [50C878]специальных предметов[-] для начала особой экспедиции." },
            { "Youn found [50C878]special item[-] use it to start advanced expedition via handheld radio.", "Вы нашли [50C878]специальный предмет[-]. Используйте его, чтобы начать особую экспедицию через рацию." },
            { "AFFLICTION TREATED\nPATIENT NEEDS TIME TO HEAL", "НЕДУГ ВЫЛЕЧЕН\nПАЦИЕНТУ НУЖНО ВРЕМЯ НА ВОССТАНОВЛЕНИЕ" },
            { "Gives ability to know your sanity status.", "Позволяет узнать состояние вашего рассудка." },
            { "WAITING FOR OTHER PLAYERS TO SLEEP", "ОЖИДАНИЕ, ПОКА ЗАСНУТ ДРУГИЕ ИГРОКИ" },
            { "WAIT FOR EVERYONE", "ЖДАТЬ ВСЕХ" },

            // Chat and expeditions
            { "Crash site has been found!", "Место крушения найдено!" },
            { "Time is up, no one has found the crash site.", "Время вышло, никто не нашёл место крушения." },
            { "[SERVER] Server reboot in 30 seconds!", "[СЕРВЕР] Перезагрузка сервера через 30 секунд!" },
            { "[SERVER] You can stay on server to being auto-reconnect, once server is up again. Or you can quit to process auto-save.", "[СЕРВЕР] Можно остаться на сервере, чтобы автоматически переподключиться после перезапуска, или выйти, чтобы сработало автосохранение." },
            { "Expeditions Starter Guide", "Начальное руководство по экспедициям" },
            { "The [27AE60]Handheld Radio[-] can be used to start or join expeditions.\nClick [00FFFF]Right Mouse Button[-] when you holding Handheld Radio to open Expeditions Menu.\nTo start [FFB233]Random Expedition[-] click [FFB233]«Start Expedition»[-] in Expeditions Menu.\n If you found any [50C878]Special Items[-] you can start [50C878]Special Expedition[-], an advanced challenges with a much higher reward.\n[50C878]Special Items[-] can be found in any container, click [50C878]«Special Expedition»[-] in Expeditions Menu to see if you have any of them to start expedition.\nYou can also [00FFFF]Invite[-] other players to your Expedition, to do so, just click on them when you holding [27AE60]Handheld Radio[-].\nAny received [00FFFF]Invites[-] you can find in Expedition Menu.\nIf you already in Expedition, you can hold [27AE60]TAB[-] key to see Expedition Objective.",
              "Используя [27AE60]Рацию[-], вы можете начинать экспедиции и присоединяться к ним.\nНажмите [00FFFF]Правую кнопку мыши[-], держа Рацию в руках, чтобы открыть меню экспедиций.\nЧтобы начать [FFB233]Случайную экспедицию[-], нажмите [FFB233]«Start Expedition»[-] в меню экспедиций.\nЕсли вы найдёте [50C878]Специальные предметы[-], то сможете начать [50C878]Специальную экспедицию[-] — сложное испытание с более весомой наградой.\n[50C878]Специальные предметы[-] могут случайно найтись в любом контейнере. Нажмите [50C878]«Special Expedition»[-] в меню экспедиций, чтобы узнать, есть ли у вас такие предметы.\nВы также можете [00FFFF]Приглашать[-] других игроков в свою экспедицию: просто нажмите на них, держа в руках [27AE60]Рацию[-].\nВсе полученные [00FFFF]Приглашения[-] находятся в меню экспедиций.\nЕсли вы уже в экспедиции, зажмите [27AE60]TAB[-], чтобы увидеть её цель." },
            { "Please wait. If you are using Hamachi/Radmin or any other LAN emulator and have connection problems, please don't ask us for help.", "Подождите. Если вы используете Hamachi/Radmin или другой эмулятор локальной сети и у вас проблемы с подключением, пожалуйста, не обращайтесь к нам за помощью." },
            { "You can hold the [50C878]TAB[-] key to see this panel again once it closes.", "Зажмите [50C878]TAB[-], чтобы снова показать эту панель, когда она пропадёт." },
        };

        private static readonly List<KeyValuePair<string, string>> Prefixes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("PLEASE DON'T MOVE, ", "ПОЖАЛУЙСТА, НЕ ДВИГАЙТЕСЬ, "),
            new KeyValuePair<string, string>("Other player gave you ", "Другой игрок дал вам "),
            new KeyValuePair<string, string>("You gave ", "Вы дали "),
            new KeyValuePair<string, string>("You has been teleported to ", "Вы телепортированы к "),
            new KeyValuePair<string, string>("Requires ", "Требуется "),
            new KeyValuePair<string, string>("Something wrong with #", "Ошибка с #"),
            new KeyValuePair<string, string>("Flair Slot #", "Слот украшения #"),
            new KeyValuePair<string, string>("Players: ", "Игроки: "),
            new KeyValuePair<string, string>("Status: ", "Статус: "),
            new KeyValuePair<string, string>("LOBBY ", "ЛОББИ "),
            new KeyValuePair<string, string>("VOTING: ", "ГОЛОСОВАНИЕ: "),
            new KeyValuePair<string, string>("Plane with a valuable cargo crashed somewhere on ", "Самолёт с ценным грузом разбился где-то в регионе "),
            new KeyValuePair<string, string>("[SERVER] ", "[СЕРВЕР] "),
        };

        private static readonly List<KeyValuePair<string, string>> Suffixes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(" IS TENDING YOU", " ЛЕЧИТ ВАС"),
            new KeyValuePair<string, string>(" can't join because has different mod version!", " не может подключиться: другая версия мода!"),
            new KeyValuePair<string, string>(" can't join because has diferent mods installed!", " не может подключиться: установлены другие моды!"),
            new KeyValuePair<string, string>(" join the server", " подключился к серверу"),
            new KeyValuePair<string, string>(" disconnected!", " отключился!"),
        };

        private static readonly List<KeyValuePair<string, string>> Infixes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(", find the crash site before other players do.", ", найдите место крушения раньше других игроков."),
            new KeyValuePair<string, string>(" IS TENDING YOU", " ЛЕЧИТ ВАС"),
            new KeyValuePair<string, string>(" changed name to ", " сменил имя на "),
        };

        // Popups and HUD messages from the mod go through these game methods, translate them in one place.
        private static IEnumerable<MethodBase> AddConfirmationOverloads(bool title, bool message)
        {
            foreach (MethodInfo M in typeof(Panel_Confirmation).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (M.Name != "AddConfirmation")
                {
                    continue;
                }
                bool HasTitle = false, HasMessage = false;
                foreach (ParameterInfo P in M.GetParameters())
                {
                    HasTitle |= P.Name == "titleLocID";
                    HasMessage |= P.Name == "messageLocID";
                }
                if (HasTitle == title && HasMessage == message)
                {
                    yield return M;
                }
            }
        }

        [HarmonyPatch]
        internal static class Panel_Confirmation_AddConfirmation_Message_Loc
        {
            private static IEnumerable<MethodBase> TargetMethods() => AddConfirmationOverloads(false, true);
            private static void Prefix(ref string messageLocID) { messageLocID = T(messageLocID); }
        }

        [HarmonyPatch]
        internal static class Panel_Confirmation_AddConfirmation_Title_Loc
        {
            private static IEnumerable<MethodBase> TargetMethods() => AddConfirmationOverloads(true, false);
            private static void Prefix(ref string titleLocID) { titleLocID = T(titleLocID); }
        }

        [HarmonyPatch]
        internal static class Panel_Confirmation_AddConfirmation_Both_Loc
        {
            private static IEnumerable<MethodBase> TargetMethods() => AddConfirmationOverloads(true, true);
            private static void Prefix(ref string titleLocID, ref string messageLocID)
            {
                titleLocID = T(titleLocID);
                messageLocID = T(messageLocID);
            }
        }

        [HarmonyPatch]
        internal static class HUDMessage_AddMessage_Loc
        {
            private static IEnumerable<MethodBase> TargetMethods()
            {
                foreach (MethodInfo M in typeof(HUDMessage).GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (M.Name == "AddMessage" && M.GetParameters().Length > 0 && M.GetParameters()[0].Name == "message")
                    {
                        yield return M;
                    }
                }
            }
            private static void Prefix(ref string message)
            {
                message = T(message);
            }
        }
    }
}
