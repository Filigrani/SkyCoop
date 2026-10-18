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

## Comparison with the original (LTS 0.13.2, TLD 2.01–2.02)

Compared against upstream commit `c3e5d76`. The network protocol and game logic are kept; the port is mostly an API adaptation (~3 300 lines added, ~1 700 removed, plus `Compat.cs` / `Loc.cs`).

### Changed
| Area | Original | This port |
|------|----------|-----------|
| Game / loader | TLD 2.01–2.02, old MelonLoader (Unhollower) | TLD 2.55, MelonLoader 0.7 (.NET 6, Il2Cpp interop); `Compat.cs` maps the old API onto 2.55 |
| Interactions | `ProcessInteraction`, `Container.Open/Close`, etc. | New 2.55 `*Interaction.PerformInteraction`, `Container.BeginContainerOpen/Close`, `SimpleInteraction` on mod objects |
| Prefabs | `Resources.Load` | Addressables (gear, animals, death backpack `CONTAINER_BackPack`) |
| Hashes / seeds | `string.GetHashCode` | Stable FNV-1a hash (random per process on .NET 6; loot, drops and scene keys differed between players) |
| Mod data in saves | Keys inside the game save slot | Sidecar files `<PersistentDataPath>/SkyCoop/<slot>/<key>` |
| Scene loading | Custom start scene | Vanilla `LoadScene`, interiors load with floors; respawn scene is loaded like a door |
| Containers | Marked looted when opening starts | Marked on close; unopened ones show their own seeded loot |
| Dropped / picked-up items | Key by exact position | Child colliders, fallback match by name + distance, cookware keeps its condition |
| Campfires | Sent as is | Fuel amount synced (`m_Fuel` added to the packet), burn time reconciled every 30 s, no duplicate fake campfires |
| Revive | Medkit (`GEAR_MedicalSupplies_hangar`) | Emergency stim (`GEAR_EmergencyStim`) |
| Mod validation | `MelonHandler` hashes | `MelonMod.RegisteredMelons` / `MelonAssembly.Hash` |
| Launch args | Read on the `Boot` scene | Read on the first scene load (2.55 has no `Boot`), so Steam *Join Game* (`+connect_lobby`) works |
| Platform detection | Version string contains `S` / `E` | `SteamConnect.CanUseSteam` |
| JSON | `MelonLoader.TinyJSON` | Bundled `TinyJSON` |

### Added
- English / Russian language choice (Auto / English / Russian) in Multiplayer > Settings (`Loc.cs`); the original had Russian hard-coded in places.
- **Downed state**: at 0 HP the player lies down; a teammate revives them with an emergency stim, or they give up and respawn at the last indoor sleep place. Respawn button on the death screen; the vanilla *Life After Death* screen is blocked online.
- Death backpack works again in 2.55; the stim no longer sticks in the hands.
- Cooperative respawn: both players end up in the same scene.
- Animal corpse sync fixed (animals loaded from Addressables, null checks in spawn regions).
- Fixed customization camera (the old fixed positions are off the map in 2.55).
- Placed bedrolls sleep via the bed's own interaction.
- `ResumePreviewer` compatibility: its save-name suffix is hidden on the HOST SERVER / NEW GAME buttons (kept on the vanilla Resume button).
- Runs alongside [Working Vehicles](https://github.com/stykko/Working-Vehicles) (vehicles are not synced).
- Guards against 2.55 save-load exceptions and log spam; `Tools/BundleRepack` for rebuilding the asset bundle.

### Removed / no longer working
- **Epic Games Store support**: `EpicOnlineServicesManager` is gone in 2.55; Epic / non-Steam players connect by IP only.
- Startup through the `Boot` scene, Steam init through `SteamManager`, mod hashing through `MelonHandler`.
- The old weather call `m_TemperatureCountForTimeOfDay` (removed from the game).
- Marking containers as looted on open, and the custom start-scene override.
- Working Vehicles multiplayer sync was tried and reverted.
- **Not compatible with the original mod or older ports**: the campfire packet has an extra field, so all players need this exact build. Old container data may be empty.

### Known limitations of this port
- Cooking is not synchronized (only the player who placed the pot sees it).
- Wood stove: hover text is patched and fire fuel is synced, but the stove state may still differ between players.
- **Clients get less warmth from a fire than the host** (about 10 °C in diagnostics); the cause is not fixed. Weather is synced by seed, but freezing and body temperature are computed locally on each client.
- The short-wave radio is not shown in first person (its model is not created in 2.55).
- Vehicles (Working Vehicles) are not synchronized.
- Epic / non-Steam: IP connection only, UDP 26950 must be open.
- Natural-item pickup sync falls back to name + distance matching, so rare mismatches are possible.
- Early, lightly tested build. The latest fixes (fire/stove sync, coop respawn, animal corpses, Steam lobby join, ResumePreviewer patch) are not committed yet and have had little in-game testing.

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

## Сравнение с оригиналом (LTS 0.13.2, TLD 2.01–2.02)

Сравнение с upstream-коммитом `c3e5d76`. Сетевой протокол и игровая логика сохранены; порт — в основном адаптация под новое API (~3 300 строк добавлено, ~1 700 удалено, плюс `Compat.cs` / `Loc.cs`).

### Что изменилось
| Область | Оригинал | Этот порт |
|---------|----------|-----------|
| Игра / загрузчик | TLD 2.01–2.02, старый MelonLoader (Unhollower) | TLD 2.55, MelonLoader 0.7 (.NET 6, Il2Cpp interop); `Compat.cs` переводит старое API на 2.55 |
| Взаимодействия | `ProcessInteraction`, `Container.Open/Close` и т.д. | Новые `*Interaction.PerformInteraction`, `Container.BeginContainerOpen/Close`, `SimpleInteraction` на объектах мода |
| Префабы | `Resources.Load` | Addressables (предметы, животные, рюкзак смерти `CONTAINER_BackPack`) |
| Хэши / сиды | `string.GetHashCode` | Стабильный FNV-1a (на .NET 6 хэш случайный; лут, дропы и ключи сцен расходились между игроками) |
| Данные мода в сейвах | Ключи в слоте сохранения игры | Файлы рядом: `<PersistentDataPath>/SkyCoop/<слот>/<ключ>` |
| Загрузка сцен | Свой стартовый сценарий | Ванильный `LoadScene`, интерьеры с полами; сцена респавна грузится как дверь |
| Контейнеры | Помечались залутанными при начале открытия | Помечаются при закрытии; нетронутые показывают свой лут по сиду |
| Выброшенные / подобранные предметы | Ключ по точной позиции | Дочерние коллайдеры, запасной поиск по имени и расстоянию, посуда хранит состояние |
| Костры | Отправлялись как есть | Синхронизируется топливо (в пакет добавлено поле `m_Fuel`), время горения сверяется раз в 30 с, нет дублей фейковых костров |
| Реанимация | Аптечка (`GEAR_MedicalSupplies_hangar`) | Экстренный стимулятор (`GEAR_EmergencyStim`) |
| Проверка модов | Хэши через `MelonHandler` | `MelonMod.RegisteredMelons` / `MelonAssembly.Hash` |
| Аргументы запуска | Читались на сцене `Boot` | Читаются при первой загрузке сцены (в 2.55 `Boot` нет), поэтому Steam *Join Game* (`+connect_lobby`) работает |
| Определение платформы | Строка версии содержит `S` / `E` | `SteamConnect.CanUseSteam` |
| JSON | `MelonLoader.TinyJSON` | Встроенный `TinyJSON` |

### Что появилось
- Выбор языка English / Русский (Авто / English / Русский) в Multiplayer > Settings (`Loc.cs`); в оригинале русский местами был зашит в код.
- **Состояние «повержен»**: при 0 HP игрок лежит; напарник поднимает его экстренным стимом, либо игрок сдаётся и возрождается у последнего места сна в помещении. Кнопка Respawn на экране смерти; ванильный экран *Life After Death* в сети заблокирован.
- Рюкзак смерти снова работает в 2.55; стим больше не «залипает» в руках.
- Кооперативный респавн: оба игрока оказываются в одной сцене.
- Исправлена синхронизация туш животных (животные из Addressables, проверки null в регионах спавна).
- Исправлена камера кастомизации (старые фиксированные точки вне карты в 2.55).
- Установленные спальники используют собственное взаимодействие кровати.
- Совместимость с `ResumePreviewer`: суффикс с именем сейва скрыт на кнопках HOST SERVER / NEW GAME (на ванильном Resume остаётся).
- Работает вместе с [Working Vehicles](https://github.com/stykko/Working-Vehicles) (машины не синхронизируются).
- Защита от исключений при загрузке сейвов 2.55 и спама в логе; `Tools/BundleRepack` для пересборки бандла.

### Что исчезло / больше не работает
- **Поддержка Epic Games Store**: в 2.55 нет `EpicOnlineServicesManager`; игроки Epic / без Steam подключаются только по IP.
- Старт через сцену `Boot`, инициализация Steam через `SteamManager`, хэширование модов через `MelonHandler`.
- Старый вызов погоды `m_TemperatureCountForTimeOfDay` (удалён из игры).
- Пометка контейнеров залутанными при открытии и подмена стартовой сцены.
- Синхронизация машин Working Vehicles была попыткой и откатена.
- **Несовместимо с оригинальным модом и старыми портами**: в пакете костра лишнее поле, у всех игроков должна быть эта сборка. Старые данные контейнеров могут быть пустыми.

### Известные ограничения этого порта
- Готовка не синхронизируется (котелок видит только тот, кто его поставил).
- Печка: подсказка исправлена и топливо огня синхронизируется, но состояние печки у игроков может отличаться.
- **Клиенты получают меньше тепла от костра, чем хост** (по диагностике около 10 °C); причина не устранена. Погода синхронизируется по сиду, а замерзание и температура тела считаются локально на каждом клиенте.
- Коротковолновая рация не отображается от первого лица (модель в 2.55 не создаётся).
- Машины (Working Vehicles) не синхронизируются.
- Epic / без Steam: только подключение по IP, должен быть открыт UDP 26950.
- Синхронизация подбора природных предметов использует запасной поиск по имени и расстоянию — редкие рассинхроны возможны.
- Ранняя, слабо протестированная сборка. Последние исправления (огонь/печка, кооп-респавн, туши животных, вход в Steam-лобби, патч ResumePreviewer) ещё не закоммичены и мало проверены в игре.

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
