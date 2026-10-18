using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
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
            // Death and revive
            { "Revive", "Воскресить" },
            { "Reviving...", "Воскрешение..." },
            { "Respawn", "Возродиться" },
            { "You need an emergency stim to revive this player", "Чтобы воскресить игрока, нужен шприц первой помощи" },
            { "You have been revived", "Вас воскресили" },
            { "You are down", "Вы ранены" },
            { "Give up", "Сдаться" },
            { "You died", "Вы погибли" },
            { "Stats", "Статистика" },
            { "Your items fell to the floor where you gave up.\nYou will respawn where you last slept indoors.", "Ваши вещи упали на пол там, где вы сдались.\nВы возродитесь там, где последний раз спали в помещении." },
            { "Wait", "Ждать" },
            { "Wait for a teammate to revive you with an emergency stim.\nPress Esc to open this window again.", "Ждите, пока союзник воскресит вас шприцем первой помощи.\nНажмите Esc, чтобы снова открыть это окно." },
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
            { "Sorry nickname should not contain non latin symbols", "Ник не должен содержать нелатинских символов" },
            { "It should be seed_name format!", "Нужен формат seed_name!" },
            { "Server is no more available to join", "Сервер больше недоступен для подключения" },
            { "You already on the server, restart the game if you want to join to another server", "Вы уже на сервере. Перезапустите игру, чтобы подключиться к другому серверу" },
            { "Connect", "Подключиться" },
            { "Next", "Далее" },
            { "Cancel", "Отмена" },
            { "Confirm", "Подтвердить" },
            { "Something wrong", "Что-то пошло не так" },
            { "Waiting for server restart", "Ожидание перезапуска сервера" },
            { "INVALID CONTAINER DATA", "НЕВЕРНЫЕ ДАННЫЕ КОНТЕЙНЕРА" },
            { "Host did not responced after 10 Attempts, you can safly exit the game.\nGame won't be saves to keep save valid for host on reconnection!", "Хост не ответил после 10 попыток, можно спокойно выйти из игры.\nИгра не будет сохранена, чтобы сохранение осталось пригодным для хоста при переподключении!" },
            { "You has been kicked from the server.", "Вы были исключены с сервера." },
            { "You has been banned from the server.", "Вы были заблокированы на сервере." },
            { "The host has disconnected you from the server due to a long period without receiving data from you.", "Хост отключил вас от сервера: от вас долго не поступали данные." },
            { "Your RCON session is over, please reconnect.", "Ваш сеанс RCON завершён, подключитесь заново." },
            { "Outdated RCON protocol!", "Устаревший протокол RCON!" },
            { "Wrong mods installed! You have that different from server's mods, or you missing some mods that installed on the server, check logs!", "Установлены неверные моды! Ваши моды отличаются от модов сервера или у вас не хватает модов, установленных на сервере. Проверьте логи!" },
            { "Connection failed", "Не удалось подключиться" },
            { "Joined Expedition", "Присоединился к экспедиции" },
            { "Expedition Invite", "Приглашение в экспедицию" },
            { "Story multiplayer: server started", "Сюжетный мультиплеер: сервер запущен" },
            { "YOU GOT A NEW FLAIR!", "ВЫ ПОЛУЧИЛИ НОВОЕ УКРАШЕНИЕ!" },
            { "YOU GOT NEW FLAIRS!", "ВЫ ПОЛУЧИЛИ НОВЫЕ УКРАШЕНИЯ!" },
            { "Open flairs customization menu?", "Открыть меню украшений?" },
            { "Found animal starting tracking", "Животное найдено, начинаем слежку" },
            { "and", "и" },
            { "Player", "Игрок" },
            { "Interacting...", "Взаимодействие..." },
            { "Lockpick", "Взломать" },
            { "Lockpicking...", "Взлом..." },
            { "Excision", "Вырезать" },
            { "Excision...", "Вырезание..." },
            { "Sawing", "Распил" },
            { "Sawing...", "Распил..." },
            { "Grinding", "Заточка" },
            { "Grinding...", "Заточка..." },
            { "Casting", "Отливка" },
            { "Casting...", "Отливка..." },
            { "Inject", "Сделать укол" },
            { "Injecting...", "Укол..." },
            { "Invite to expedition", "Пригласить в экспедицию" },
            { "Inviting...", "Приглашение..." },
            { "Ignite from player's fire", "Зажечь от огня игрока" },
            { "Examine", "Осмотреть" },
            { "Diagnosing...", "Осмотр..." },
            { "Borrow", "Одолжить" },
            { "Borrowing...", "Заимствование..." },
            { "Pickup", "Подобрать" },
            { "(Loading)", "(Загрузка)" },
            { "Stay(Seconds)", "Остаться (секунды)" },
            { "Wait(Seconds)", "Ждать (секунды)" },

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
            new KeyValuePair<string, string>("You has been kicked from the server.\nReason: ", "Вы были исключены с сервера.\nПричина: "),
            new KeyValuePair<string, string>("You has been banned from the server.\nReason: ", "Вы были заблокированы на сервере.\nПричина: "),
            new KeyValuePair<string, string>("Wasn't able to get all chunks of container data, please try again or cancel.", "Не удалось получить все части данных контейнера, попробуйте ещё раз или отмените."),
            new KeyValuePair<string, string>("Server sent invalid data, this can be network delay problem, please press Confirm to try load data again. If problem stays, message us about this problem.", "Сервер прислал неверные данные, возможно из-за задержки сети. Нажмите «Подтвердить», чтобы загрузить данные ещё раз. Если проблема остаётся, сообщите нам о ней."),
            new KeyValuePair<string, string>("Timed out on response from host for last request. Trying to repeat request to correct the situation, please wait...\nAttempt #", "Хост не ответил на последний запрос. Повторяем запрос, подождите...\nПопытка №"),
            new KeyValuePair<string, string>("Please wait...\nPing Attempt #", "Подождите...\nПопытка пинга №"),
            new KeyValuePair<string, string>("Players sleep ", "Спят игроков: "),
            new KeyValuePair<string, string>("Wrong mod version! Server using version ", "Неверная версия мода! Сервер использует версию "),
            new KeyValuePair<string, string>("Your version of the mod, isn't supported\nHost using version ", "Ваша версия мода не поддерживается\nХост использует версию "),
            new KeyValuePair<string, string>("Frequency ", "Частота "),
            new KeyValuePair<string, string>("Animal with GUID ", "Животное с GUID "),
        };

        private static readonly List<KeyValuePair<string, string>> Suffixes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(" IS TENDING YOU", " ЛЕЧИТ ВАС"),
            new KeyValuePair<string, string>(" can't join because has different mod version!", " не может подключиться: другая версия мода!"),
            new KeyValuePair<string, string>(" can't join because has diferent mods installed!", " не может подключиться: установлены другие моды!"),
            new KeyValuePair<string, string>(" join the server", " подключился к серверу"),
            new KeyValuePair<string, string>(" disconnected!", " отключился!"),
            new KeyValuePair<string, string>(" not exist!", " не существует!"),
            new KeyValuePair<string, string>(" INTERACTING WITH THIS!", " УЖЕ С ЭТИМ ВЗАИМОДЕЙСТВУЕТ!"),
            new KeyValuePair<string, string>(" MOVED, ACTION CANCELED", " ДВИНУЛСЯ, ДЕЙСТВИЕ ОТМЕНЕНО"),
            new KeyValuePair<string, string>(" INSIDE, CAN'T DISMANTLE THIS!", " ВНУТРИ, ЭТО НЕЛЬЗЯ РАЗОБРАТЬ!"),
            new KeyValuePair<string, string>(" IS ALREADY COLLECTING THIS", " УЖЕ ЭТО СОБИРАЕТ"),
            new KeyValuePair<string, string>(" IS BREAKING THIS!", " УЖЕ ЭТО РАЗБИРАЕТ!"),
            new KeyValuePair<string, string>(" IS BREAKING THIS", " УЖЕ ЭТО РАЗБИРАЕТ"),
            new KeyValuePair<string, string>(" IS USING THIS!", " УЖЕ ЭТИМ ПОЛЬЗУЕТСЯ!"),
            new KeyValuePair<string, string>(" IS USING THIS", " УЖЕ ЭТИМ ПОЛЬЗУЕТСЯ"),
            new KeyValuePair<string, string>(" IS BUSY WITH THIS ALREADY", " УЖЕ ЭТИМ ЗАНЯТ"),
        };

        private static readonly List<KeyValuePair<string, string>> Infixes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(", find the crash site before other players do.", ", найдите место крушения раньше других игроков."),
            new KeyValuePair<string, string>(" IS TENDING YOU", " ЛЕЧИТ ВАС"),
            new KeyValuePair<string, string>(" changed name to ", " сменил имя на "),
        };

        // Popup handlers check the title by its English text, so keep a way back from the translated one.
        private static readonly Dictionary<string, string> TitlesBack = new Dictionary<string, string>();

        private static string Title(string en)
        {
            string ru = T(en);
            if (!string.IsNullOrEmpty(ru) && ru != en)
            {
                TitlesBack[ru.Trim().ToUpper()] = en.Trim().ToUpper();
            }
            return ru;
        }

        public static string En(string shown)
        {
            if (shown != null && TitlesBack.TryGetValue(shown.Trim().ToUpper(), out string en))
            {
                return en;
            }
            return shown;
        }

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
            private static void Prefix(ref string titleLocID) { titleLocID = Title(titleLocID); }
        }

        [HarmonyPatch]
        internal static class Panel_Confirmation_AddConfirmation_Both_Loc
        {
            private static IEnumerable<MethodBase> TargetMethods() => AddConfirmationOverloads(true, true);
            private static void Prefix(ref string titleLocID, ref string messageLocID)
            {
                titleLocID = Title(titleLocID);
                messageLocID = T(messageLocID);
            }
        }

        private static string Plural(int n, string one, string few, string many)
        {
            int m10 = n % 10, m100 = n % 100;
            if (m10 == 1 && m100 != 11) return one;
            if (m10 >= 2 && m10 <= 4 && (m100 < 12 || m100 > 14)) return few;
            return many;
        }

        private static string ReplaceCount(Regex Rx, string Text, string one, string few, string many)
        {
            Match M = Rx.Match(Text);
            while (M.Success)
            {
                int n = int.Parse(M.Groups[1].Value);
                string Rep = n + " " + Plural(n, one, few, many);
                Text = Text.Substring(0, M.Index) + Rep + Text.Substring(M.Index + M.Length);
                M = Rx.Match(Text, M.Index + Rep.Length);
            }
            return Text;
        }

        private static readonly (Regex, string)[] HoverRules =
        {
            (new Regex(@"(\d+) days?\b"), null),
            (new Regex(@"(\d+) hours?\b"), null),
            (new Regex(@"(\d+) minutes?\b"), null),
            (new Regex(@"Dropped just now"), "Брошено только что"),
            (new Regex(@"Dropped long time ago"), "Брошено давно"),
            (new Regex(@"Dropped (.+) ago"), "Брошено $1 назад"),
            (new Regex(@"(.+)'s Backpack"), "Рюкзак игрока $1"),
            (new Regex(@"Fuel left for "), "Топлива хватит на "),
            (new Regex(@"No Fuel left "), "Топливо закончилось, "),
            (new Regex(@"Cannot snare here "), "Здесь нельзя ставить силки, "),
            (new Regex(@"May catch in: "), "Возможно, поймает через: "),
            (new Regex(@"Will catch in: "), "Поймает через: "),
            (new Regex(@"As soon as no one see"), "Как только никто не видит"),
            (new Regex(@"Cannot be cured here"), "Здесь нельзя обработать"),
            (new Regex(@"Cures in"), "Будет готово через"),
            (new Regex(@"Cools in"), "Остынет через"),
            (new Regex(@"\bCured\b"), "Готово"),
            (new Regex(@"\bCooled\b"), "Остыло"),
            (new Regex(@"\b[Dd]ropped by "), "Бросил: "),
            (new Regex(@"\bPlaced by "), "Поставил: "),
            (new Regex(@"\bSetup by "), "Установил: "),
        };

        // Hover texts of dropped items, backpacks, snares and so on are built by the mod from English parts.
        public static string HoverT(string s)
        {
            if (string.IsNullOrEmpty(s) || !IsRussian)
            {
                return s;
            }
            string R = s;
            R = ReplaceCount(HoverRules[0].Item1, R, "день", "дня", "дней");
            R = ReplaceCount(HoverRules[1].Item1, R, "час", "часа", "часов");
            R = ReplaceCount(HoverRules[2].Item1, R, "минута", "минуты", "минут");
            for (int i = 3; i < HoverRules.Length; i++)
            {
                R = HoverRules[i].Item1.Replace(R, HoverRules[i].Item2);
            }
            string[] Lines = R.Split('\n');
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = T(Lines[i]);
            }
            return string.Join("\n", Lines);
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
