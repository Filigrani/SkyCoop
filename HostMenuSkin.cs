using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace SkyCoop
{
    // "Create server" screen (MP_HostSettings prefab from multiplayerstuff.unity3d). The prefab is compiled with English
    // texts and the default Unity look, so the translation and the game-like style are applied at runtime.
    public static class HostMenuSkin
    {
        static readonly Color PanelBg = new Color(0.04f, 0.05f, 0.06f, 0.90f);
        static readonly Color HintBg = new Color(0.02f, 0.03f, 0.04f, 0.97f);
        static readonly Color ControlBg = new Color(0.11f, 0.13f, 0.15f, 0.95f);
        static readonly Color ItemBg = new Color(0.14f, 0.16f, 0.18f, 1f);
        static readonly Color TextMain = new Color(0.85f, 0.87f, 0.90f, 1f);
        static readonly Color TextHint = new Color(0.76f, 0.78f, 0.82f, 1f);
        static readonly Color TextTitle = new Color(0.95f, 0.95f, 0.95f, 1f);
        static readonly Color Accent = new Color(0.92f, 0.95f, 1f, 1f);

        // English text (trimmed) -> Russian
        static readonly Dictionary<string, string> Ru = new Dictionary<string, string>
        {
            { "Items duplication", "Дублирование предметов" },
            { "Safe zone circles", "Круги безопасных зон" },
            { "Containers duplication", "Дублирование контейнеров" },
            { "Spawn style", "Появление" },
            { "Same as host", "Как у хоста" },
            { "Client's choice", "Выбор клиента" },
            { "Random", "Случайно" },
            { "Fixed place", "Фиксированное" },
            { "Near host", "Рядом с хостом" },
            { "Players", "Игроки" },
            { "Steam P2P", "Steam P2P" },
            { "Public server", "Публичный сервер" },
            { "Port", "Порт" },
            { "Default is 26950", "По умолчанию 26950" },
            { "Fire Sync", "Огонь" },
            { "Disabled", "Выключено" },
            { "Visual", "Только вид" },
            { "Share", "Общий" },
            { "Cheats", "Читы" },
            { "Host only", "Только хост" },
            { "Allow to everyone", "Для всех" },
            { "Availability", "Доступ" },
            { "Invites only", "По приглашению" },
            { "Friends only", "Только друзья" },
            { "Public", "Публичный" },
        };

        static readonly Dictionary<string, string> RuHints = new Dictionary<string, string>
        {
            { "Dupes", "Дублирование предметов.\n\nВыключено — когда кто-то подбирает предмет, найденный в мире, он исчезает у всех остальных. Неважно, подобран он до или после захода других игроков.\n\nВключено — предмет не исчезает у других, его может подобрать каждый игрок, даже зашедший позже." },
            { "SafeZones", "Круги безопасных зон.\n\nВключено — внутри безопасных кругов на карте (поселения, лагеря) игроки не могут наносить урон друг другу. Безопасные помещения остаются безопасными всегда.\n\nВыключено — PvP работает везде, кроме безопасных помещений." },
            { "DupesBoxes","Дублирование контейнеров.\n\nВыключено — когда кто-то открывает контейнер, предметы внутри исчезают у всех остальных. Неважно, открыт он до или после захода других игроков.\n\nВключено — предметы в контейнерах не исчезают у других, их может забрать каждый игрок, даже зашедший позже." },
            { "Spawn", "Где появятся подключившиеся игроки.\n\nКак у хоста — в регионе, где сейчас хост, в случайной точке региона.\n\nВыбор клиента — клиент сам выбирает регион появления.\n\nСлучайно — случайный регион и случайная точка.\n\nФиксированное — всегда одно и то же место (регион и точка) для всех. Оно выбирается один раз при первом запуске сервера." },
            { "Steam", "Steam Peer To Peer.\n\nПозволяет создать сервер без эмуляции LAN и проброса портов, но у вас и у всех игроков должна быть Steam-версия игры.\n\nПосле создания пригласите друзей из Steam или скопируйте ссылку-приглашение в лобби. Сервер также можно сделать публичным, и он появится в списке серверов.\n\nЕсли нужно играть Steam вместе с Epic Games Store (или другими лаунчерами), снимите галочку и используйте эмуляцию LAN (Hamachi, Radmin) или проброс порта." },
            { "SteamWarn", "Не просите помощи с эмуляцией LAN и пробросом портов! Мы не ваш интернет-провайдер, мы только делаем мод." },
            { "SteamMaster", "Отправляет ваш Steam ID на мастер-сервер, чтобы к вам могли зайти случайные игроки.\n\nЕсли хотите играть ТОЛЬКО С ДРУЗЬЯМИ — отключите эту опцию. Если включить, сервер увидят все владельцы игры в Steam в списке серверов." },
            { "Port", "Порт для вашего сервера.\n\nЕсли оставить 26950, игрокам не придётся вводить порт при подключении по IP.\n\nЕсли указать другой порт, клиентам нужно вводить его так: ip:порт (например 127.0.0.1:7777).\n\n(Если не знаете, что это, — не трогайте!)" },
            { "Fire", "Синхронизация огня.\n\nВыключено — у каждого свой огонь.\n\nТолько вид — вы видите костры и печи других игроков, но не получаете от них тепло, пока не добавите свое топливо.\n\nОбщий — как «Только вид», но если кто-то добавил топливо, у вас тоже добавится столько же. (Общее топливо у всех может отличаться, потому что вы тратите его на пропуск времени и сон.) Когда у вас кончится топливо, огонь станет только визуальным." },
            { "Cheats", "Читы (консольные команды).\n\nВыключено — никто не может использовать читы.\nТолько хост — читы доступны только владельцу сервера.\nДля всех — читы доступны любому игроку." },
            { "LobbyType", "Доступ к серверу.\n\nПо приглашению — только вы можете приглашать игроков.\nТолько друзья — любой ваш друг из Steam может зайти без приглашения.\nПубличный — любой игрок может зайти из списка серверов." },
        };

        static string Pick(string en, string ru)
        {
            return Loc.IsRussian ? ru : en;
        }

        static string Tr(string en)
        {
            string key = en.Trim();
            return Loc.IsRussian && Ru.TryGetValue(key, out string ru) ? ru : en;
        }

        static string ControlName(Transform t)
        {
            // Label -> HintPanel -> Hint -> control
            Transform p = t.parent;
            for (int i = 0; i < 2 && p != null; i++)
            {
                p = p.parent;
            }
            return p != null ? p.name : "";
        }

        static void SetImage(Component c, Color col)
        {
            Image img = c.GetComponent<Image>();
            if (img != null)
            {
                img.color = col;
            }
        }

        static void AddBorder(GameObject go)
        {
            if (go.GetComponent<Outline>() == null)
            {
                Outline o = go.AddComponent<Outline>();
                o.effectColor = new Color(1f, 1f, 1f, 0.22f);
                o.effectDistance = new Vector2(1f, -1f);
            }
        }

        public static void Apply(GameObject menu, Font gameFont)
        {
            if (menu == null)
            {
                return;
            }
            try
            {
                bool ru = Loc.IsRussian;

                SetImage(menu.transform, PanelBg);
                AddBorder(menu);

                foreach (Image img in menu.GetComponentsInChildren<Image>(true))
                {
                    string n = img.gameObject.name;
                    Transform parent = img.transform.parent;
                    string pn = parent != null ? parent.name : "";
                    if (n == "HintPanel")
                    {
                        img.color = HintBg;
                        AddBorder(img.gameObject);
                    }
                    else if (n == "Hint" || n == "Close" || n == "Host")
                    {
                        img.color = ControlBg;
                        AddBorder(img.gameObject);
                    }
                    else if (n == "Background" && img.GetComponentInParent<Toggle>() != null)
                    {
                        img.color = ControlBg;
                        AddBorder(img.gameObject);
                    }
                    else if (n == "Checkmark")
                    {
                        img.color = Accent;
                    }
                    else if (n == "Template")
                    {
                        img.color = HintBg;
                        AddBorder(img.gameObject);
                    }
                    else if (n.StartsWith("Item Background"))
                    {
                        img.color = ItemBg;
                    }
                    else if (img.GetComponent<Dropdown>() != null || img.GetComponent<InputField>() != null)
                    {
                        img.color = ControlBg;
                        AddBorder(img.gameObject);
                    }
                }

                foreach (Button b in menu.GetComponentsInChildren<Button>(true))
                {
                    ColorBlock cb = b.colors;
                    cb.normalColor = Color.white;
                    cb.highlightedColor = new Color(1.4f, 1.4f, 1.4f, 1f);
                    cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    b.colors = cb;
                }

                foreach (Text t in menu.GetComponentsInChildren<Text>(true))
                {
                    string n = t.gameObject.name;
                    string parentName = t.transform.parent != null ? t.transform.parent.name : "";
                    if (gameFont != null)
                    {
                        t.font = gameFont;
                    }
                    t.color = TextMain;

                    if (n == "Title")
                    {
                        t.text = Pick("CREATE SERVER", "СОЗДАТЬ СЕРВЕР");
                        t.color = TextTitle;
                        t.fontSize = 34;
                    }
                    else if (parentName == "Close" || parentName == "Host")
                    {
                        t.text = parentName == "Close" ? Pick("BACK", "НАЗАД") : Pick("CREATE", "СОЗДАТЬ");
                        t.color = TextTitle;
                        t.fontSize = 20;
                    }
                    else if (parentName == "HintPanel")
                    {
                        t.color = TextHint;
                        if (ru)
                        {
                            string key = n == "Label" ? ControlName(t.transform) : "SteamWarn";
                            if (RuHints.TryGetValue(key, out string hint))
                            {
                                t.text = hint;
                            }
                        }
                        t.resizeTextForBestFit = true;
                        t.resizeTextMinSize = 9;
                        t.resizeTextMaxSize = 14;
                    }
                    else if (n == "Text" && parentName == "Hint")
                    {
                        t.color = Accent;
                    }
                    else if (n.StartsWith("Label") || n == "Placeholder" || n.StartsWith("Item Label"))
                    {
                        t.text = Tr(t.text);
                        t.resizeTextForBestFit = true;
                        t.resizeTextMinSize = 9;
                        t.resizeTextMaxSize = 14;
                        t.horizontalOverflow = HorizontalWrapMode.Overflow;
                    }
                }

                // Option lists: the shown caption and the opened list are built from these.
                foreach (Dropdown d in menu.GetComponentsInChildren<Dropdown>(true))
                {
                    // Spawn style list: extra "Near host" option (index 4).
                    if (d.options.Count == 4 && d.options[3].text.Trim() == "Fixed place")
                    {
                        d.options.Add(new Dropdown.OptionData("Near host"));
                    }
                    if (ru)
                    {
                        for (int i = 0; i < d.options.Count; i++)
                        {
                            d.options[i].text = Tr(d.options[i].text);
                        }
                    }
                    d.RefreshShownValue();
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[UI] Can't skin the host menu: " + e.Message);
            }
        }
    }
}
