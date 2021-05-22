# SkyCoop
Multiplayer for The Long Dark game

# How to install mod?

First of all this is mod that loads with using MelonLoader, so you need to install [MelonLoader](https://melonwiki.xyz/#/)
Next download last release version of the mod, and unzip files from archive in "Mods" folder in your The Long Dark directory.

And you will need to install [ModComponent](https://github.com/ds5678/ModComponent/releases/latest/download/ModComponent.dll) by ds5678, same way put that dll file near with SkyCoop.dll.


# Troubleshooting

1. -If you has been used PvP release of the mod ModComponentAPI, ModComponentMapper, and AssetLoader all need to be deleted for correct work of the mod.
2. -Make sure you did not leave zip archive of mod in "Mods" folder but unziped it.


# How to play online?

How to play the mod?
1. Install any LAN network emulators, I recommend [LogMeIn Hamachi](https://www.vpn.net/)
2. Register account in the hamachi.
3. Create room, and tell other player name of the room to they join in.
4. After you join the same room in the hamachi, copy your friend's IPv4 adress in the hamachi and run the game.
5. Now host player need to select game mode you want to play, Survival sandbox or Challenges. Just select game mode like a you do when play single player, load your save file or create a  new one.
6. Just host player loaded press Escape button, there will be new button "Host", press it, and server will be up (if you not sure that server is started, check out melon loader console window there will be message about it).
7. Other player just need to press "Connect" button in main menu, input IP from hamachi, and press connect, If the connection was successful, you will see panel with selecting survivor(or it will be load save file itself instantly if you has played before).

# Mechanics:
- Giving item to other player You can give item to other player if they are near! Open your inventory, select item you want to give, and press G.
- Cycle time and sleeping Time itself is going in runtime, 1 in game minute is 5 seconds of real time. You can still sleep, pass time or harvest animals, it still wastes your calories, but there are no time speedups, so anyway if both players are sleeping, time will be skipped based on the highest number of hours that players have selected. (Like if player1 selected 3 hours but player2 has selected 5 hours, 5 hours will be skipped).
- Spawned items When you connect to each other, game creates save file with same seed, this makes all item locations will the same for both players, however each player has his own item instance, meaning 1 item can be picked up by both players, the same way the forest multiplayer portrays it. But some specific things can be still random, like additional spawned loot or items from the other mods.
- Animals Animals can attack or react only on nearest player.
- Locations Both players can be in different locations in same time, but if you on one location and very far from each others, animals may not spawn around one of players.
- Friendly Fire Watch your fire! You can hurt other player with firearms, be careful when hunting animals!
- Reviving Across whole world you can find medkits, or craft them yourself. If one of players are dead, other player revive them with medkit, you need to get close to them and press N when you will see text about possible reviving
