using Il2Cpp;
using Il2CppTMPro;
using LiteNetLib.Utils;
using SkyCoopClient;
using SkyCoopServer;
using UnityEngine;
using static SkyCoop.PlayersManager;
using static SkyCoopServer.DataStr;

namespace SkyCoop
{
    public class ClientHandle
    {
        public static void Welcome(NetDataReader Reader)
        {
            int ID = Reader.GetInt();
            Logger.Log(ConsoleColor.Cyan, $"Server welcomes me with my ID: {ID}");
            ClientSend.Welcome();
            MenuHook.RemovePleaseWait();
            MenuHook.DoPleaseWait("Please wait...", "Getting data about server...");
            //MenuHook.DoOKMessage("Connected!", Message);
        }

        public static void ApplyConfig(DataStr.ServerConfig CFG)
        {
            ModMain.Client.m_Config = CFG;
            Logger.Log(ConsoleColor.Cyan, "Server config");
            Logger.Log(ConsoleColor.Cyan, "ServerName: " + CFG.m_ServerName);
            Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + CFG.m_MaxPlayers);
            Logger.Log(ConsoleColor.Cyan, "Seed: " + CFG.m_Seed);
            Logger.Log(ConsoleColor.Cyan, "ExperienceMode: " + CFG.m_ExperienceMode);
            Logger.Log(ConsoleColor.Cyan, "VoicePort: " + CFG.m_VoicePort);
            Logger.Log(ConsoleColor.Cyan, "SceneToSpawn: " + CFG.m_SceneToSpawn);
            Logger.Log(ConsoleColor.Cyan, "GameMode: " + CFG.m_GameMode);
            Logger.Log(ConsoleColor.Cyan, "CheatsAllowed: " + CFG.m_CheatsAllowed);

            if (CFG.m_CheatsAllowed)
            {
                if(uConsole.m_Instance == null)
                {
                    DebugConsole.ReimplementConsole();
                }
            }
        }

        public static void ApplyRules(DataStr.GameRules Rules)
        {
            ModMain.Client.m_Rules = Rules;
        }

        public static void ServerConfig(NetDataReader Reader)
        {
            DataStr.ServerConfig CFG = Reader.GetConfig();
            DataStr.GameRules Rules = Reader.GetRules();

            ApplyConfig(CFG);
            ApplyRules(Rules);

            PlayersManager.DestoryPlayers();
            PlayersManager.InitilizePlayers(CFG.m_MaxPlayers);

            ModMain.Client.m_IsReady = true;
            ModMain.Client.ProcessAllDelayedPackages();
            MenuHook.RemovePleaseWait();
            ModMain.SetupSurvivalSettings(CFG.m_ExperienceMode, CFG.m_Seed, CFG.m_SceneToSpawn);
            PlayersManager.DeactivateAllSpectatingTargets();

            if (CFG.m_VoicePort != 0)
            {
                Task.Run(() => { ModMain.Client.ConnectToServerVoice(CFG.m_VoicePort); });
            }

            //GameObject SoundPlayerPrefab = AssetManager.GetAssetFromBundle<GameObject>("JoinServer");
            //if (SoundPlayerPrefab)
            //{
            //    GameObject SoundPlayer = GameObject.Instantiate(SoundPlayerPrefab);
            //    SoundPlayer.GetComponent<AudioSource>().Play();
            //    SceneManager.DontDestroyOnLoad(SoundPlayer);
            //    UnityEngine.Object.Destroy(SoundPlayer, 15);
            //}
            //else
            //{
            //    Logger.Log(ConsoleColor.Red, "Can't load cringe audio. JoinServer prefab not exist!");
            //}
        }

        public static void ServerConfigUpdated(NetDataReader Reader)
        {
            DataStr.ServerConfig CFG = Reader.GetConfig();
            DataStr.GameRules Rules = Reader.GetRules();

            ApplyConfig(CFG);
            ApplyRules(Rules);
        }

        public static void ServerChangesMap(NetDataReader Reader)
        {
            DataStr.ServerConfig CFG = ModMain.Client.m_Config;
            PlayersManager.DeactivateAllSpectatingTargets();
            ModMain.ChangeMap();
        }

        public static void ClientPosition(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Vector3 Position = Reader.GetVector3Unity();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if(Player)
            {
                Player.SetPosition(Position);
            }
        }

        public static void ClientRotation(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Quaternion Rotation = Reader.GetQuaternionUnity();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetRotation(Rotation);
            }
        }

        public static void ClientSceneNotification(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            bool Present = Reader.GetBool();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                bool PreviousState = Player.gameObject.activeSelf;
                
                if (Present)
                {
                    Player.SetVisibile(true);
                } else
                {
                    Player.SetVisibile(false);
                }

                if(Present != PreviousState)
                {
                    Logger.Log("(ClientSceneNotification) Player ID " + PlayerID + " Visible " + Present);
                }
            }
        }
        public static void ClientHoldingGear(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int GearVariant = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetGear(GearName, GearVariant);
            }
        }

        public static void ClientCrouch(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            bool IsCrouching = Reader.GetBool();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetCrouching(IsCrouching);
            }
        }
        public static void ClientAction(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            int Action = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetAcation(Action);
            }
        }
        public static void ClientFire(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.DoFire();
            }
        }
        public static void ClientDamagesMe(NetDataReader Reader)
        {
            float Damage = Reader.GetFloat();
            int PlayerID = Reader.GetInt();
            Comps.PlayerDamageColider.DamageZone BodyPart = (Comps.PlayerDamageColider.DamageZone)Reader.GetInt();
            string MeleeName = Reader.GetString();
            PlayersManager.OtherPlayerDamageMe(Damage, PlayerID, BodyPart, MeleeName);
        }
        public static void ClientProjectile(NetDataReader Reader)
        {
            int ShooterID = Reader.GetInt();
            Vector3 Pos = Reader.GetVector3Unity();
            Quaternion Rot = Reader.GetQuaternionUnity();
            string ProjectileName = Reader.GetString();
            float ExtraFloat = Reader.GetFloat();
            bool PlayEffect = Reader.GetBool();
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, PlayEffect, ExtraFloat);
        }
        public static void ClientProjectileThrow(NetDataReader Reader)
        {
            int ShooterID = Reader.GetInt();
            Vector3 Pos = Reader.GetVector3Unity();
            Quaternion Rot = Reader.GetQuaternionUnity();
            string ProjectileName = Reader.GetString();
            Vector3 Velocity = Reader.GetVector3Unity();
            Vector3 AngularVelocity = Reader.GetVector3Unity();
            float Fuse = Reader.GetFloat();
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, true, Velocity, AngularVelocity, Fuse);
        }
        public static void KillFeedMessage(NetDataReader Reader)
        {
            DataStr.KillFeedMessage Message = Reader.GetKillFeedMessage();
            SkyCoop.Logger.Log("KillFeedMessage");
            SkyCoop.Logger.Log("- m_Killer" + Message.m_Killer);
            SkyCoop.Logger.Log("- m_Victim" + Message.m_Victim);
            SkyCoop.Logger.Log("- m_Assist" + Message.m_Assist);
            SkyCoop.Logger.Log("- m_DeathReason" + Message.m_DeathReason.ToString());
            SkyCoop.Logger.Log("- m_Flags:");
            foreach (DataStr.KillFeedFlag Flag in Message.m_Flags)
            {
                SkyCoop.Logger.Log("-- Flag: " + Flag.ToString());
            }
            CanvasUI.AddKillFeedMessage(Message);
        }
        public static void ServerSquadEliminated(NetDataReader Reader)
        {
            string SquadName = Reader.GetString();
            CanvasUI.AddTextMessage($"Squad {SquadName} Eliminated!");
        }
        public static void ClientName(NetDataReader Reader)
        {
            string ClientName = Reader.GetString();
            int ClientID = Reader.GetInt();
            Logger.Log(ConsoleColor.Cyan, "Player: " + ClientName+" with ID "+ClientID);
            PlayersManager.SetPlayerName(ClientID, ClientName);
        }
        public static void ClientRequestRespawn(NetDataReader Reader)
        {
            Vector3 Position = Reader.GetVector3Unity();
            Quaternion Rotation = Reader.GetQuaternionUnity();
            bool RespawnAnim = Reader.GetBool();

            PlayersManager.DeactivateAllSpectatingTargets();
            PlayersManager.RespawnOnPoint(Position, Rotation, RespawnAnim);
        }

        public static void ClientInjectedItem(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int ObjectID = Reader.GetInt();
            Vector3 Position = Reader.GetVector3Unity();
            Quaternion Rotation = Reader.GetQuaternionUnity();

            PlayersManager.GetPlayer(PlayerID).AddInjectedItem(GearName, ObjectID, Position, Rotation);
        }
        public static void ClientRemoveInjectedItem(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int DamageZone = Reader.GetInt();

            PlayersManager.GetPlayer(PlayerID).RemoveInjectedItem(GearName, (Comps.PlayerDamageColider.DamageZone)DamageZone);
        }

        public static void ClientGettingDamage(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();

            PlayersManager.GetPlayer(PlayerID).DoGetDamage();
        }

        public static void ClientSendGear(NetDataReader Reader)
        {
            DataStr.GearDataVisual Visual = Reader.GetGearVisual();

            GearsSync.HandleGearDropped(Visual);
        }

        public static void ClientPickUpGear(NetDataReader Reader)
        {
            bool GotGear = Reader.GetBool();
            MenuHook.RemovePleaseWait(); // Мы возможно в ожидании действия, и получили рефанд гира.
            FireHook.FinishCookingAction();

            if (GotGear)
            {
                string GearName = Reader.GetString();
                string JSON = Reader.GetString();
                bool DropAround = Reader.GetBool();
                float TimeBeingCooked = Reader.GetFloat();
                string CookingResult = Reader.GetString();
                float Volume = Reader.GetFloat();
                GearsSync.HandleGearPickUp(new GearsSync.GearPickedElement(GearName, JSON, DropAround, false, TimeBeingCooked, CookingResult, Volume));
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "ClientPickUpGear, gear no longer exist.");
                GearsSync.PickUpFailed();
            }
        }

        public static void ClientRemoveGear(NetDataReader Reader)
        {
            string GUID = Reader.GetString();

            GearsSync.HandleGearRemove(GUID);
        }

        public static void ClientOpenableInteraction(NetDataReader Reader)
        {
            string GUID = Reader.GetString();
            bool OpenState = Reader.GetBool();
            bool AllowAudio = Reader.GetBool();

            OpenablesSync.HandleOpenableSync(GUID, OpenState, AllowAudio);
        }

        public static void ClientClothing(NetDataReader Reader)
        {
            DataStr.ClothingData Data = Reader.GetClothingData();
            int FromID = Reader.GetInt();

            PlayersManager.GetPlayer(FromID).SetClothing(Data);
        }

        public static void ClientZoneUpdated(NetDataReader Reader)
        {
            bool Active = Reader.GetBool();

            if (Active)
            {
                DataStr.DangerCircleShrinkStateData Stage = Reader.GetZoneStage();

                Vector3 NextCetner = Reader.GetVector3Unity();
                float NextRadius = Reader.GetFloat();
                Vector2 MapRefScale = Reader.GetVector2Unity();

                DangerCircleManager.HandleDangerCircleSync(Stage, NextCetner, NextRadius, MapRefScale);
            }
            else
            {
                DangerCircleManager.RemoveDangerCircle();
            }
        }

        public static void ClientGameModeTimer(NetDataReader Reader)
        {
            int Seconds = Reader.GetInt();

            GameModeHUD.UpdateGameModeTimer(Seconds);
        }

        public static void ClientHUDSideBar(NetDataReader Reader)
        {
            int SideBarIndex = Reader.GetInt();
            string Icon = Reader.GetString();
            string Prefix = Reader.GetString();
            string Afix = Reader.GetString();

            GameModeHUD.SetSideIcon(SideBarIndex, Icon);
            GameModeHUD.SetSideLablePrefix(SideBarIndex, Prefix);
            GameModeHUD.SetSideLable(SideBarIndex, $" {Afix}");

            Logger.Log($"ClientHUDSideBar() SideBarIndex {SideBarIndex} Icon {Icon} Prefix {Prefix} Afix {Afix}");
        }

        public static void ClientHUDSideBarUpdate(NetDataReader Reader)
        {
            int SideBarIndex = Reader.GetInt();
            string Afix = Reader.GetString();
            GameModeHUD.SetSideLable(SideBarIndex, $" {Afix}");

            Logger.Log($"ClientHUDSideBarUpdate() SideBarIndex {SideBarIndex} Afix {Afix}");
        }

        public static void ClientFreeze(NetDataReader Reader)
        {
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
            PlayerManager playerManagerComponent = GameManager.GetPlayerManagerComponent();
            if (playerManagerComponent.m_ItemInHands)
            {
                if (playerManagerComponent.m_ItemInHands.m_CantDropItem)
                {
                    GameManager.GetPlayerAnimationComponent().DropCurrentItemInHand();
                    if (playerManagerComponent.m_ItemInHands)
                    {
                        playerManagerComponent.m_ItemInHands.StickToGroundAtPlayerFeet(GameManager.GetPlayerTransform().position);
                    }
                }
                else
                {
                    GameManager.GetPlayerManagerComponent().UnequipImmediate(false);
                }
            }
            if (playerManagerComponent.m_ItemInHands && (playerManagerComponent.m_ItemInHands.IsLitLamp() || playerManagerComponent.m_ItemInHands.IsLitFlashlight()))
            {
                playerManagerComponent.m_ItemInHands.Drop(1, false, true);
            }
            AnimatedInteraction.InterruptAnyInProgressAnimations();
            playerManagerComponent.ResetPickup();
            GameManager.GetVpFPSPlayer().CancelZoom();
            InterfaceManager.TrySetPanelEnabled<Panel_Inventory>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Container>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Inventory_Examine>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_LifeAfterDeath>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Container>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Map>(false);
            PlayersManager.FullyCure();
            PlayersManager.ExitVehicleForced();
        }

        public static void ServerLeaders(NetDataReader Reader)
        {
            PlayersManager.DeactivateAllSpectatingTargets();

            int Count = Reader.GetInt();
            List<DataStr.LeaderData> LeadersList = new List<DataStr.LeaderData>();
            for (int i = 0; i < Count; i++)
            {
                LeadersList.Add(Reader.GetLeaderData());
            }

            Vector3 Position = Reader.GetVector3Unity();
            Quaternion Rotation = Reader.GetQuaternionUnity();

            string SquadName = Reader.GetString();
            int RandomSeed = Reader.GetInt();

            string PrefabName = ModMain.Client.m_Config.m_GameMode != "Shrink" ? "Victory" : "Victory_Squad";

            if(LeadersList.Count == 1)
            {
                PrefabName = "Victory_Star";
            }

            GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>(PrefabName);
            if (Reference)
            {
                GameObject Obj = UnityEngine.Object.Instantiate(Reference, Position, Rotation);
                if (Obj)
                {
                    if(PrefabName == "Victory_Star")
                    {
                        GameObject VictoryDoll = Obj.transform.GetChild(0).gameObject;
                        if (VictoryDoll)
                        {
                            VictoryDoll.gameObject.SetActive(true);

                            UnityEngine.Random.InitState(RandomSeed);

                            int AnimStyle = UnityEngine.Random.Range(4, 6);


                            VictoryDoll.GetComponent<Animator>().SetInteger("VictoryPlace", AnimStyle);
                            Comps.NetworkPlayer PlayerComp = PlayersManager.ApplyPlayer(VictoryDoll, -1);

                            if (PlayerComp)
                            {
                                PlayerComp.SetClothing(LeadersList[0].m_ClothingData);
                            }
                        }
                        Obj.transform.GetChild(1).gameObject.SetActive(true);
                        Obj.transform.GetChild(1).GetComponent<TextMeshPro>().SetText(CanvasUI.GetPlayerName(LeadersList[0].m_ID));
                    }
                    else
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            DataStr.LeaderData Data = LeadersList[i];
                            GameObject VictoryDoll = Obj.transform.GetChild(i).gameObject;
                            if (VictoryDoll)
                            {
                                VictoryDoll.gameObject.SetActive(true);
                                VictoryDoll.GetComponent<Animator>().SetInteger("VictoryPlace", i + 1);
                                Comps.NetworkPlayer PlayerComp = PlayersManager.ApplyPlayer(VictoryDoll, -1);
                                if (PlayerComp)
                                {
                                    PlayerComp.SetClothing(Data.m_ClothingData);
                                }
                            }
                            Obj.transform.GetChild(i + 3).gameObject.SetActive(true);
                            Obj.transform.GetChild(i + 3).GetComponent<TextMeshPro>().SetText(CanvasUI.GetPlayerName(Data.m_ID));
                        }
                        if (PrefabName == "Victory_Squad")
                        {
                            if (!string.IsNullOrEmpty(SquadName))
                            {
                                Obj.transform.GetChild(6).gameObject.SetActive(true);
                                Obj.transform.GetChild(6).GetComponent<TextMeshPro>().SetText(SquadName);
                            }
                            else
                            {
                                Obj.transform.GetChild(6).gameObject.SetActive(false);
                            }
                        }
                    }

                    Transform Cam = Obj.transform.FindChild("Camera");
                    Cam.GetComponent<Camera>().enabled = false;
                    Cam.GetComponent<Animator>().enabled = true;
                    Cam.gameObject.AddComponent<Comps.CameraAttention>();
                }
            }
        }

        public static void ClientTryInteract(NetDataReader Reader)
        {
            bool Result = Reader.GetBool();
            PlayersManager.HandleTryInteract(Result);
        }

        public static void ClientInVehicle(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            bool InVechicle = Reader.GetBool();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetInVehicle(InVechicle);
            }
        }

        public static void ClientStatusMessage(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            int Status = Reader.GetInt();

            if(Status == 0)
            {
                CanvasUI.AddLeaveMessage(PlayerID);
            }else if(Status == 1)
            {
                CanvasUI.AddJoinMessage(PlayerID);
            }
        }
        public static void ClientDeathPackAdded(NetDataReader Reader)
        {
            DataStr.DeathPack Pack = Reader.GetDeathPack();
            DeathPacksManager.HandleDeathPack(Pack.m_Prefab, Pack.m_Position.ConvertToUnity(), Pack.m_Rotation.ConvertToUnity(), Pack.m_GUID, Pack.m_Owner);
        }

        public static void ClientDeathPackRemoved(NetDataReader Reader)
        {
            string GUID = Reader.GetString();
            DeathPacksManager.HandleDeathPackRemoved(GUID);
        }
        public static void ClientContainerOpen(NetDataReader Reader)
        {
            string CompressedJSON = Reader.GetString();
            MenuHook.RemovePleaseWait();
            ContainersSync.HandleContainerOpen(CompressedJSON);
        }
        public static void ServerContainerDataArrived(NetDataReader Reader)
        {
            bool Result = Reader.GetBool();
            MenuHook.RemovePleaseWait();
            ContainersSync.HandleClosePanel();
        }

        public static void ClientContainerStateUpdated(NetDataReader Reader)
        {
            string GUID = Reader.GetString();
            int State = Reader.GetInt();

            ContainersSync.HandleStateUpdated(GUID, State);
        }

        public static void ClientHUDTimerPrefix(NetDataReader Reader)
        {
            string Prefix = Reader.GetString();
            GameModeHUD.SetTimerPrefix(Prefix);
        }

        public static void ClientRespawnAsSpectator(NetDataReader Reader)
        {
            PlayersManager.RespawnAsSpecator();
        }

        public static void ClientSpawnProp(NetDataReader Reader)
        {
            DataStr.PropData PropData = Reader.GetPropData();
            PropsManager.HandlePropSpawn(PropData);
        }

        public static void ClientMoveProp(NetDataReader Reader)
        {
            string GUID = Reader.GetString();
            Vector3 Position = Reader.GetVector3Unity();

            if (!Reader.GetBool())
            {
                PropsManager.HandlePropMoved(GUID, Position);
            }
            else // If predictable
            {
                PropsManager.HandlePropMoved(GUID, Position, Reader.GetVector3Unity(), Reader.GetVector3Unity());
            }
        }

        public static void ClientRemoveProp(NetDataReader Reader)
        {
            string PropGUID = Reader.GetString();
            PropsManager.HandlePropRemove(PropGUID);
        }

        public static void ClientJoinGame(NetDataReader Reader)
        {
            string GameGUID = Reader.GetString();
            int PlayerID = Reader.GetInt();
            int PokerID = Reader.GetInt();

            PropsManager.HandleCardGameJoin(GameGUID, PlayerID, PokerID);
        }

        public static void ClientCardGameTurn(NetDataReader Reader)
        {
            string GameGUID = Reader.GetString();
            int Turn = Reader.GetInt();

            SkyCoop.Logger.Log($"ClientCardGameTurn GameGUID {GameGUID} Turn {Turn}");

            PropsManager.HandleCardGameTurn(GameGUID, Turn);
        }

        public static void ClientCardGamePokerUpdate(NetDataReader Reader)
        {
            string GameGUID = Reader.GetString();
            int UpdateType = Reader.GetInt();

            if(UpdateType == 0)
            {
                int GamePlayerID = Reader.GetInt();
                int Chips = Reader.GetInt();

                PropsManager.HandleCardGameChips(GameGUID, GamePlayerID, Chips);
            }else if(UpdateType == 1)
            {
                int GamePlayerID = Reader.GetInt();
                int Bets = Reader.GetInt();

                PropsManager.HandleCardGameBet(GameGUID, GamePlayerID, Bets);
            }
            else if (UpdateType == 2)
            {
                int GamePlayerID = Reader.GetInt();
                int CardID = Reader.GetInt();
                int CardType = Reader.GetInt();
                int CardSuit = Reader.GetInt();

                PropsManager.HandleCardGameCard(GameGUID, GamePlayerID, CardID, CardType, CardSuit);
            }
            else if (UpdateType == 3)
            {
                int Dealer = Reader.GetInt();

                PropsManager.HandleCardGameDealer(GameGUID, Dealer);
            }
        }


        public static void ClientFishTalk(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.DoFishTalk();
            }
        }

        public static void ClientGetTier(NetDataReader Reader)
        {
            int MyNewTier = Reader.GetInt();

            int OldTier = PlayersManager.m_LocalPlayerData.m_Tier;

            PlayersManager.m_LocalPlayerData.m_Tier = MyNewTier;

            if (OldTier != -1 && MyNewTier != OldTier && !PlayersManager.s_Spectator && !GameManager.GetConditionComponent().IsConsideredDead())
            {
                PlayersManager.GiveoutStartingGear(MyNewTier);
                PlayersManager.DoWeaponSwitch(true);
            }
        }

        public static void ClientAssignSquad(NetDataReader Reader)
        {
            bool HasSquad = Reader.GetBool();

            if (HasSquad)
            {
                HUDMessage.AddMessage("You joined squad!", true, true);
                GameAudioManager.PlayGUIButtonClick();
            }
            else
            {
                SquadHUD.s_SquadMembers.Clear();
            }
            PlayersManager.s_InSquad = HasSquad;
        }

        public static void ClientSquadHealth(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            float Health = Reader.GetFloat();
            bool Debuffs = Reader.GetBool();
            bool KnockedDown = Reader.GetBool();

            SquadHUD.UpdateMember(PlayerID, Health, Debuffs, KnockedDown);
        }

        public static void ServerRequestSquadHealth(NetDataReader Reader)
        {
            ClientSend.SendSquadHealth(m_LocalPlayerData.m_Health, m_LocalPlayerData.m_HasDebuffs, m_LocalPlayerData.m_KnockedDown);
        }

        public static void ClientTilt(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            float Tilt = Reader.GetFloat();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if (Player)
            {
                Player.SetTilt(Tilt);
            }
        }

        public static void ServerGearSpawnerMarker(NetDataReader Reader)
        {
            GearsSync.s_SpawnersMarkers.Clear();

            for (int i = GearsSync.s_SpawnersMarkersObjects.Count-1; i >= 0; i--)
            {
                GameObject Obj = GearsSync.s_SpawnersMarkersObjects[i];
                if (Obj)
                {
                    UnityEngine.Object.Destroy(Obj);
                }
            }
            GearsSync.s_SpawnersMarkersObjects.Clear();

            int Count = Reader.GetInt();

            for (int i = 0; i < Count; i++)
            {
                GearsSync.s_SpawnersMarkers.Add(Reader.GetVector3Unity());
            }
        }

        public static void ServerSquadMemberLeft(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();

            SquadHUD.RemoveMember(PlayerID);
        }

        public static void ServerSquadResponce(NetDataReader Reader)
        {
            Packet.SquadResponce Reason = (Packet.SquadResponce)Reader.GetInt();

            switch (Reason)
            {
                case Packet.SquadResponce.CantCreateSquad:
                    HUDMessage.AddMessage("Wasn't able to create squad!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.SquadCreated:
                    HUDMessage.AddMessage("Squad created!", true, true);
                    GameAudioManager.PlayGUIButtonClick();
                    break;
                case Packet.SquadResponce.YouNotInSquad:
                    HUDMessage.AddMessage("You not in squad!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.YouLeftSquad:
                    HUDMessage.AddMessage("You left squad!", true, true);
                    GameAudioManager.PlayGUIButtonClick();
                    break;
                case Packet.SquadResponce.YouAlreadyInSquad:
                    HUDMessage.AddMessage("You already in squad!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.YouAreNotInvited:
                    HUDMessage.AddMessage("You are not invited!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.InviteSent:
                    HUDMessage.AddMessage("Invite sent", true, true);
                    GameAudioManager.PlayGUIButtonClick();
                    break;
                case Packet.SquadResponce.SquadNotExist:
                    HUDMessage.AddMessage("Squad not exist!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.SquadIsFull:
                    HUDMessage.AddMessage("Squad is full!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.TheyAlreadyInSquad:
                    HUDMessage.AddMessage("Player already in squad!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.YouCantInviteThemATM:
                    HUDMessage.AddMessage("You can't invite this player right now!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                case Packet.SquadResponce.YouInvitedTooMuch:
                    HUDMessage.AddMessage("You sent too much invites, please wait!", true, true);
                    GameAudioManager.PlayGUIError();
                    break;
                default:
                    break;
            }
        }
        public static void ServerSquadCreated(NetDataReader Reader)
        {
            string SquadName = Reader.GetString();
            CanvasUI.AddTextMessage($"Squad {SquadName} Created!");
        }

        public static void ClientInviteToSquad(NetDataReader Reader)
        {
            string SquadName = Reader.GetString();
            MenuHook.DoInviteSquadMessage(SquadName);
        }

        public static void ClientBloodLosses(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            int BloodLosses = Reader.GetInt();

            Comps.NetworkPlayer Player = s_Players[PlayerID];

            if (Player)
            {
                Player.m_BloodLosses = BloodLosses;
            }
        }

        public static void ClientReviveRequest(NetDataReader Reader)
        {
            int ReviverID = Reader.GetInt();

            if (GameManager.GetBrokenBody().HasAffliction)
            {
                PlayersManager.RevivedViaEmergencyStim(ReviverID);
            }
        }

        public static void ClientChatMessage(NetDataReader Reader)
        {
            int From = Reader.GetInt();
            string Message = Reader.GetString();

            CanvasUI.HandleChatMessage(Message, From);
        }

        public static void ServerUpdateInGameTime(NetDataReader Reader)
        {
            float ElapsedInGameHours = Reader.GetFloat();
            float NormalizedTOD = Reader.GetFloat();
            bool EveryoneIsSleeping = Reader.GetBool();
            int PlayersReady = Reader.GetInt();
            int TotalPlayers = Reader.GetInt();

            UniStormWeatherSystem Uni = GameManager.GetUniStorm();

            if (Uni)
            {
                Uni.m_ElapsedHours = ElapsedInGameHours;
                Uni.SetNormalizedTime(NormalizedTOD);
            }

            if(ModMain.Client != null && ModMain.Client.m_IsReady)
            {
                ModMain.Client.m_LastServerTime = ElapsedInGameHours;
            }

            SleepHook.SetEveryoneIsSleeping(EveryoneIsSleeping);

            if (SleepHook.s_LastPlayersReadyForAccelerate != PlayersReady)
            {
                SleepHook.s_LastPlayersReadyForAccelerate = PlayersReady;

                Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();
                if (Panel)
                {
                    HUDMessage.HUDMessageInfo msg = new HUDMessage.HUDMessageInfo();
                    msg.m_Text = $"{PlayersReady} / {TotalPlayers} {Localization.Get("GAMEPLAY_PlayersReady_Accelerate")}";
                    HUDMessage.ShowMessage(Panel, msg);
                }
            }
        }

        public static void ClientStartFire(NetDataReader Reader)
        {
            string GUID = Reader.GetString();
            float MaxBurnTime = Reader.GetFloat();
            float ElapsedBurnTime = Reader.GetFloat();
            float FuelHeatIncress = Reader.GetFloat();
            float Heat = Reader.GetFloat();
            float InnerRadius = Reader.GetFloat();
            float OutterRadius = Reader.GetFloat();
            int State = Reader.GetInt();
            bool IsDynamic = Reader.GetBool();

            if (IsDynamic)
            {
                Vector3 Position = Reader.GetVector3Unity();
                Quaternion Rotation = Reader.GetQuaternionUnity();

                FireHook.CreateCampfire(GUID, Position, Rotation);
            }
            FireHook.HandleFireSync(GUID, MaxBurnTime, ElapsedBurnTime, FuelHeatIncress, Heat, InnerRadius, OutterRadius, (FireState)State);
        }
        public static void ClientAddFuel(NetDataReader Reader)
        {
            string GUID = Reader.GetString();

            FireHook.HandleAddFuel(GUID);
        }

        public static void ClientTakeTorch(NetDataReader Reader)
        {
            bool Allowed = Reader.GetBool();

            if (Allowed)
            {
                FireHook.TakeTorch();
            }
            else
            {
                FireHook.TakeTorchFailed();
            }
        }

        public static void ClientDismantleCampfire(NetDataReader Reader)
        {
            string GUID = Reader.GetString();

            FireHook.HandleRemoveFire(GUID);
        }

        public static void ClientCharcoalCollect(NetDataReader Reader)
        {
            int Charcoal = Reader.GetInt();
            MenuHook.RemovePleaseWait();

            if(Charcoal > 0)
            {
                FireHook.HandleCharcoal(Charcoal);
            }
            else
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage("No charcoal to collect", true, true);
            }
        }
        public static void ClientRequestFreeCookingSlot(NetDataReader Reader)
        {
            int SlotIndex = Reader.GetInt();
            MenuHook.RemovePleaseWait();

            if (SlotIndex != -1)
            {
                FireHook.HandleFreeCookingSlot(SlotIndex);
            }
            else
            {
                GameAudioManager.PlayGUIError();

                if (FireHook.s_AnySlotsMode)
                {
                    HUDMessage.AddMessage("There no free cooking slots left!", true, true);
                }
                else
                {
                    HUDMessage.AddMessage("This cooking slot is already in use!", true, true);
                }
                FireHook.FinishCookingAction();
            }
        }

        public static void ClientGearCookingInteration(NetDataReader Reader)
        {
            string GearGUID = Reader.GetString();
            string FireGUID = Reader.GetString();

            MenuHook.RemovePleaseWait();
            FireHook.HandleCookingInteraction(GearGUID, FireGUID);
        }

        public static void ServerGearBeingCookedProgress(NetDataReader Reader)
        {
            string GearGUID = Reader.GetString();
            float Progress = Reader.GetFloat();

            GearsSync.HandleGearCooking(GearGUID, Progress);
        }

        public static void ServerWaterRefund(NetDataReader Reader)
        {
            float Volume = Reader.GetFloat();
            bool GoodWater = Reader.GetBool();

            if (GameManager.GetInventoryComponent())
            {
                if (GoodWater)
                {
                    GameManager.GetInventoryComponent().AddToWaterSupply(new Il2CppTLD.IntBackedUnit.ItemLiquidVolume(FireHook.ConvertVolumeToUnits(Volume)), LiquidQuality.Potable);
                }
                else
                {
                    GameManager.GetInventoryComponent().AddToWaterSupply(new Il2CppTLD.IntBackedUnit.ItemLiquidVolume(FireHook.ConvertVolumeToUnits(Volume)), LiquidQuality.NonPotable);
                }
            }
        }

        public static void ServerWeather(NetDataReader Reader)
        {
            DataStr.WeatherSyncData Data = Reader.GetWeather();

            WeatherHook.HandleWeatherSync(Data);
        }

        public static void ClientHarvest(NetDataReader Reader)
        {
            string GUID = Reader.GetString();

            HarvestHook.HandleRemove(GUID);
        }

        public static void ClientBreakDown(NetDataReader Reader)
        {
            string GUID = Reader.GetString();

            BreakDownHook.HandleRemove(GUID);
        }

        public static void ClientWaterSourceInteraction(NetDataReader Reader)
        {
            bool Result = Reader.GetBool();
            string GUID = Reader.GetString();
            float Current = Reader.GetFloat();
            bool IsGood = Reader.GetBool();

            if (Result)
            {
                WaterSourceSync.HandleWaterSource(GUID, Current, IsGood);
            }
            else
            {
                HUDMessage.AddMessage("Interaction blocked by other player!", true, true);
                GameAudioManager.PlayGUIError();
            }
        }
    }
}
