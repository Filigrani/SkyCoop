using Il2Cpp;
using Il2CppRewired;
using Il2CppSystem.Linq;
using Il2CppTLD.PDID;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public class PropsManager
    {
        public static void HandlePropSpawn(DataStr.PropData PropData)
        {
            GameObject Reference;

            if (PropData.frombundle)
            {
                Reference = AssetManager.GetAssetFromBundle<GameObject>(PropData.prefabname);
            }
            else
            {
                Reference = AssetManager.GetAssetFromGame<GameObject>(PropData.prefabname);
            }

            if (Reference)
            {
                GameObject PropObj = UnityEngine.Object.Instantiate<GameObject>(Reference, PropData.GetVector3Unity(), PropData.GetQuaternionUnity());
                if (PropObj)
                {
                    ObjectGuid GUIDObj = PropObj.GetComponent<ObjectGuid>();
                    if (GUIDObj == null)
                    {
                        GUIDObj = PropObj.AddComponent<ObjectGuid>();
                    }
                    PdidTable.RuntimeRegister(GUIDObj, PropData.guid);

                    if(PropData.prefabname == "CardGameTablePrefab")
                    {
                        PropObj.AddComponent<Comps.CardGameProp>().SetInteraction("Start Game", PropData.guid);
                    }else if(PropData.prefabname == "TexasHoldEmGamePrefab")
                    {
                        Comps.TexasHoldEmProp Game = PropObj.AddComponent<Comps.TexasHoldEmProp>();
                        Game.SetGUID(PropData.guid);
                        Game.ManualStart();
                    }
                }
            }
        }

        public static void HandlePropRemove(string GUID)
        {
            GameObject GearObject = PdidTable.GetGameObject(GUID);
            if (GearObject)
            {
                //SkyCoop.Logger.Log(ConsoleColor.Green, $"ClientRemoveProp {GUID} found and deleted");
                UnityEngine.Object.Destroy(GearObject);
            }
            else
            {
                //SkyCoop.Logger.Log(ConsoleColor.Red, $"ClientRemoveProp {GUID} not found!");
            }
        }

        public static void HandleCardGameJoin(string GUID, int PlayerID, int PokerID)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                PropObj.GetComponent<Comps.TexasHoldEmProp>().RegisterPlayer(PlayerID, PokerID);
            }
        }

        public static void HandleCardGameTurn(string GUID, int Turn)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                PropObj.GetComponent<Comps.TexasHoldEmProp>().SetCurrentPlayerTurn(Turn);
            }
        }
        public static void HandleCardGameDealer(string GUID, int Dealer)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                PropObj.GetComponent<Comps.TexasHoldEmProp>().SetDealer(Dealer);
            }
        }

        public static void HandleCardGameChips(string GUID, int GamePlayerID, int Chips)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                PropObj.GetComponent<Comps.TexasHoldEmProp>().SetPlayerChips(GamePlayerID, Chips);
            }
        }
        public static void HandleCardGameBet(string GUID, int GamePlayerID, int Bet)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                PropObj.GetComponent<Comps.TexasHoldEmProp>().SetPlayerBet(GamePlayerID, Bet);
            }
        }
        public static void HandleCardGameCard(string GUID, int GamePlayerID, int CardID, int CardType, int CardSuit)
        {
            GameObject PropObj = PdidTable.GetGameObject(GUID);
            if (PropObj)
            {
                if(GamePlayerID == 4)
                {
                    PropObj.GetComponent<Comps.TexasHoldEmProp>().SetCard(CardID, CardType, CardSuit);
                }
                else
                {
                    PropObj.GetComponent<Comps.TexasHoldEmProp>().SetCard(GamePlayerID, CardID, CardType, CardSuit);
                }
            }
        }
    }
}
