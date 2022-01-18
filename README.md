# SkyCoop
Multiplayer for The Long Dark game

We don't own The Long Dark! The Long Dark belong to Hinterland Studio Inc. 
This is a free modification that loads using MelonLoader.

# How to install mod

First of all this mod loads using MelonLoader, so you need to install [MelonLoader](https://melonwiki.xyz/#/)
Next download the latest release of the mod, and unzip the files from the archive into the "Mods" folder in the install directory for The Long Dark.


# How to play online

How to play the mod

If you wanna play on the Epic Games Store version or GoG, you need to use a LAN emulator.
If all players are using the Steam version, then the host can just invite players through Steam. If so, start with step 5.

1. Install any LAN network emulator, I recommend [LogMeIn Hamachi](https://www.vpn.net/)
2. Register an account in the hamachi.
3. Create a room, and tell other players the name of the room for them to join.
4. When you join the room in the hamachi, copy your friend's IPv4 adress in the hamachi and run the game.

5. Now the host player need to select the game mode you want to play: Survival, Sandbox or Challenges. Just select game mode like you do when playing single player, and create a new save file (Do not use your old single player saves, this will cause desync of items and houses).
6. Host player: once the game has loaded,  press the Escape button. There will be new button "Host", press it (If you're on the Steam version you will have a choice. Use Steam if all your friends are using the Steam version, if not use local), and then the server will be up! If you not sure that the server has started, check out the MelonLoader console window, and there will be a message about it.
7. Other players just need to press the "Connect" button in the main menu, input the IP from hamachi, and press connect. If the connection was successful, you will see a menu to select a survivor (or it will load a save file itself automatically if you have played before).
On the Steam version, if you want to join by invite, your game shouldn't be running when you click on join button in Steam chat.

# Mechanics:
- **Save Compatibility** Please do not use your old saves/single player saves! Create a new save slot for multiplayer. If you use your old saves, this will cause a lot of desyncs.
- **Giving item to other player** You can give an item to another player if they are nearby! Open your inventory, select the item you want to give, and press G on your keyboard.
- **Cycle time and sleeping** Time itself is going in runtime, 1 in game minute is 5 seconds of real time. You can still sleep, pass time or harvest animals. It still wastes your calories, but there are no time speedups. If both players are sleeping, time will be skipped based on the highest number of hours that all players have selected. (For example, if player1 selected 3 hours, but player2 has selected 5 hours, 5 hours will be skipped).
- **Spawned items** All spawned items are the same for every player. If any player picks up an item, that item will be removed for all other players.
- **Animals** Animals can only attack or react to the nearest player.
- **Locations** All players can be in different locations at the same time.
- **Friendly Fire** Watch your fire! You can hurt other player with firearms, be careful when hunting animals!
- **Reviving** Across the whole world you can find medkits, or craft them yourself. If a player is dead, another player can revive them with a medkit. You need to have a medkit in your inventory and hover over the body of the dead player until you see the text "Revive".
-  **Saving** To save your progress, you need to sleep one hour. Saves works for host and client.


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
