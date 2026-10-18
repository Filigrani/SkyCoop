# SkyCoop — The Long Dark 2.55 port

**English** | [Русский](#русский)

Unofficial port of [Filigrani's SkyCoop](https://github.com/Filigrani/SkyCoop) (LTS 0.13.2) to **The Long Dark 2.55**. The original author did not release this build and does not support it.

## Requirements
- The Long Dark **2.55**
- MelonLoader **0.7.x** (tested on 0.7.2)
- ModComponent **7.x** (`ModComponent.dll` in `Mods`) — without it the mod's items will not appear

## Installation
Download the zip from [Releases](../../releases) and copy its contents into the game folder, replacing files:

```
Mods\SkyCoop.dll
Mods\SkyCoop.modcomponent
Mods\multiplayerstuff.unity3d
UserLibs\Steamworks.NET.dll
```

Remove any older SkyCoop files first. All players must use the same build.

## Playing
- **Steam:** host with "Host Server" (Steam lobby works), friends join via your Steam lobby.
- **Epic / no Steam:** IP connection only. The host opens UDP port **26950**.
- Quitting the game during a network session closes the whole game (by design).

## What is new in this port
- Updated for The Long Dark 2.55 (TFTFT) game APIs
- English / Russian language choice

## Known limitations
- Cooking is not synchronized (only the player who placed the pot sees it)
- Travois is not synchronized (only the player who place sees it)
- Wood stove: hover text is patched and fire fuel is synced, but the stove state may still differ between players.
- Epic / non-Steam: IP connection only, UDP 26950 must be open.
- Early build: if something breaks, attach `MelonLoader\Latest.log`

## Build
`dotnet build -c Release SkyCoop.csproj` (game assemblies must be available; see `SkyCoop.csproj`).

---

<a name="русский"></a>
# Русский

Неофициальный порт [SkyCoop от Filigrani](https://github.com/Filigrani/SkyCoop) (LTS 0.13.2) на **The Long Dark 2.55**. Оригинальный автор эту сборку не выпускал и не поддерживает.

## Что нужно
- The Long Dark **2.55**
- MelonLoader **0.7.x** (проверено на 0.7.2)
- ModComponent **7.x** (`ModComponent.dll` в папке `Mods`) — без него не появятся предметы мода

## Установка
Скачай zip из [Releases](../../releases) и скопируй содержимое в папку игры с заменой:

```
Mods\SkyCoop.dll
Mods\SkyCoop.modcomponent
Mods\multiplayerstuff.unity3d
UserLibs\Steamworks.NET.dll
```

Старые файлы SkyCoop удалите заранее. У всех игроков должна быть одна и та же сборка.

## Как играть
- **Steam:** хост — "Host Server" (включите Steam-лобби), остальные заходят через друзья Steam.
- **Epic / без Steam:** только подключение по IP. Хосту нужно открыть UDP-порт **26950**.
- Выход из игры в сетевой сессии закрывает игру (так задумано).

## Что нового в этой версии
- Обновлено для работы с API игры The Long Dark версии 2.55 (TFTFT)
- Добавлена ​​возможность выбора языка (английский / русский)

## Известные ограничения
- Процесс приготовления пищи не синхронизирован (котелок видит только тот игрок, который его установил)
- Волокуши (travois) не синхронизированы (их видит только тот игрок, который их установил)
- Дровяная печь: текст при наведении исправлен, а топливо синхронизировано, однако состояние самой печи у разных игроков может различаться.
- Версии для Epic Games или не Steam: поддерживается только прямое подключение по IP; необходимо открыть UDP-порт 26950.
- Ранняя сборка: в случае возникновения неполадок приложите файл `MelonLoader\Latest.log`

## Сборка
`dotnet build -c Release SkyCoop.csproj` (нужны сборки игры, см. `SkyCoop.csproj`).

---

# Original project 

Multiplayer for The Long Dark game. We don't own The Long Dark! The Long Dark belongs to Hinterland Studio Inc. This is a free modification that loads using MelonLoader.
Mod creator's Discord: https://discord.gg/ydmufU2HHj · Mod creator's Patreon: https://www.patreon.com/Filigrani

# Оригинальный проект

Мультиплеер для игры The Long Dark. Мы не владеем The Long Dark! The Long Dark принадлежит Hinterland Studio Inc. Это бесплатная модификация, загружающаяся с помощью MelonLoader.
Дискорд создателя мода: https://discord.gg/ydmufU2HHj · Патреон создателя мода: https://www.patreon.com/Filigrani

---

Большое спасибо что создал мод [Filigrani](https://github.com/Filigrani)
