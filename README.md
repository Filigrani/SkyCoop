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
- **Epic / no Steam:** IP connection only. The host opens UDP port **26950** (or use Radmin VPN / Hamachi).
- Quitting the game during a network session closes the whole game (by design).

## What is new in this port
- Updated for The Long Dark 2.55 (TFTFT) game APIs
- English / Russian language choice
- Fixed the customization camera
- Compatible with [Working Vehicles](https://github.com/stykko/Working-Vehicles) (vehicles are **not** synchronized between players)

## Known limitations
- Cooking is not synchronized (only the player who placed the pot sees it)
- The short-wave radio is not shown in first person (its model is not created in 2.55)
- Containers opened on older port versions may be empty — start a new save
- Early build: if something breaks, attach `MelonLoader\Latest.log`

## Build
`dotnet build -c Release SkyCoop.csproj` (game assemblies must be available; see `SkyCoop.csproj`). Branch: `port-2.55`.

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

Старые файлы SkyCoop удали заранее. У всех игроков должна быть одна и та же сборка.

## Как играть
- **Steam:** хост — "Host Server" (работает Steam-лобби), остальные заходят через лобби друга.
- **Epic / без Steam:** только подключение по IP. Хосту нужно открыть UDP-порт **26950** (или использовать Radmin VPN / Hamachi).
- Выход из игры в сетевой сессии закрывает игру целиком — так задумано.

## Что нового в порте
- Обновление под API игры 2.55 (TFTFT)
- Выбор языка English / Русский
- Исправлена камера кастомизации
- Совместим с [Working Vehicles](https://github.com/stykko/Working-Vehicles) (машины между игроками **не** синхронизируются)

## Известные ограничения
- Готовка не синхронизируется (котелок видит только тот, кто его поставил)
- Коротковолновая рация не отображается от первого лица (модель в 2.55 не создаётся)
- Ящики, открытые на старых версиях порта, могут быть пустыми — лучше новое сохранение
- Ранняя сборка: если что-то сломалось, пришли `MelonLoader\Latest.log`

## Сборка
`dotnet build -c Release SkyCoop.csproj` (нужны сборки игры, см. `SkyCoop.csproj`). Ветка: `port-2.55`.

---

# Original project / Оригинальный проект

Multiplayer for The Long Dark game. We don't own The Long Dark! The Long Dark belongs to Hinterland Studio Inc. This is a free modification that loads using MelonLoader.
Discord: https://discord.gg/ydmufU2HHj · Patreon: https://www.patreon.com/Filigrani

# Used stuff:

BuildAssetBundles script for Unity

ModComponent by Original dll by WulfMarius, rework by ds5678 
AssetLoader by Original dll by WulfMarius, rework by ds5678 

Some models used in the mod is free models from site https://sketchfab.com/
Human RIG https://assetstore.unity.com/packages/3d/characters/survival-stylized-characters-5-weapons-115559
Glowing outlines https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488

----------------------------------------------------------------------

Co-Creating

Players characters models and all clothing for them by NativeCodeMake https://www.patreon.com/NativeCodeMaker

----------------------------------------------------------------------

Some code references used from:

JumpMod by DigitalzombieTLD https://github.com/DigitalzombieTLD/JumpMod
FoxCompanion by DigitalzombieTLD https://github.com/DigitalzombieTLD/FoxCompanion

Tutorial how to write multiplayer https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5

----------------------------------------------------------------------

Used for debugging:

UnityExplorer https://github.com/sinai-dev/UnityExplorer
DeveloperConsole https://github.com/FINDarkside/TLD-Developer-Console

(For custom gears spawns)

Coordinates-Grabber https://github.com/ds5678/Coordinates-Grabber
PlacingAnywhere https://github.com/Xpazeman/tld-placing-anywhere
KeyboardUtilities https://github.com/ds5678/KeyboardUtilities


----------------------------------------------------------------------

Big thanks for The long Dark Modding Discord Community!

Special thanks:
Digitalzombie for explanation how to load custom models, animation and resources works.
ds5678 for help with some problems, and help with ModComponent.
