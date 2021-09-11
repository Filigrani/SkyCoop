# SkyCoop
Multiplayer for The Long Dark game

We don't own The Long Dark! The Long Dark belong to Hinterland Studio Inc. 
This is free modification that loads with using MelonLoader.

# How to install mod?

First of all this is mod that loads with using MelonLoader, so you need to install [MelonLoader](https://melonwiki.xyz/#/)
Next download last release version of the mod, and unzip files from archive in "Mods" folder in your The Long Dark directory.


# How to play online?

How to play the mod?

If you wanna play with using Epic Games Store version or GoG. You need to use lan network emulator.
If all players use Steam version, host just can invite players by steam, if so, start with step 5.

1. Install any LAN network emulators, I recommend [LogMeIn Hamachi](https://www.vpn.net/)
2. Register account in the hamachi.
3. Create room, and tell other player name of the room to they join in.
4. When you join the room in the hamachi, copy your friend's IPv4 adress in the hamachi and run the game.

5. Now host player need to select game mode you want to play, Survival sandbox or Challenges. Just select game mode like a you do when play single player, create new save file (Do not use your old single player saves, this will case to desync of items and houses).
6. Just host player loaded press Escape button, there will be new button "Host", press it (If you on steam version  you will have choice, use steam if all your friends use steam version, if not use local), and server will be up (if you not sure that server is started, check out melon loader console window there will be message about it).
7. Other player just need to press "Connect" button in main menu, input IP from hamachi, and press connect, If the connection was successful, you will see panel with selecting survivor(or it will be load save file itself instantly if you has played before).
(On steam version, if you want to join by invite, your game shouln't be run when you click on join button at steam chat).

# Mechanics:
- Save Compatibility Please do not use your old saves/single player saves, please create new save slot for multiplayer, if you will use your old saves, this will case to a lot of desynces.
- Giving item to other player You can give item to other player if they are near! Open your inventory, select item you want to give, and press G on your keyboard.
- Cycle time and sleeping Time itself is going in runtime, 1 in game minute is 5 seconds of real time. You can still sleep, pass time or harvest animals, it still wastes your calories, but there are no time speedups, so anyway if both players are sleeping, time will be skipped based on the highest number of hours that players have selected. (Like if player1 selected 3 hours but player2 has selected 5 hours, 5 hours will be skipped).
- Spawned items All spawned items are same for every player, if any player pickup item, item will be removed for all other players.
- Animals Animals can attack or react only on nearest player.
- Locations All players can be in different locations in same time.
- Friendly Fire Watch your fire! You can hurt other player with firearms, be careful when hunting animals!
- Reviving Across whole world you can find medkits, or craft them yourself. If one of players are dead, other players can revive them with medkit, you need have medkit in your inventory and hover on body of player until you see text "Revive".
-  Saving To save your progress, you need to sleep one hour. Saves works for host and client.


# Used stuff:

BuildAssetBundles script for Unity

ModComponent by Original dll by WulfMarius, rework by ds5678 
AssetLoader by Original dll by WulfMarius, rework by ds5678 

Some models used in the mod is free models from site https://sketchfab.com/
Human model https://assetstore.unity.com/packages/3d/characters/survival-stylized-characters-5-weapons-115559
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

Used for debuging:

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
