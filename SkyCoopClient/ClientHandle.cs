using Il2Cpp;
using Il2CppRewired;
using Il2CppTMPro;
using LiteNetLib.Utils;
using SkyCoopClient;
using SkyCoopServer;
using UnityEngine;
using static Il2CppParadoxNotion.Services.Logger;
using static SkyCoop.Comps.PlayerDamageColider;

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
            Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + CFG.m_MaxPlayers);
            Logger.Log(ConsoleColor.Cyan, "Seed: " + CFG.m_Seed);
            Logger.Log(ConsoleColor.Cyan, "StartingRegion: " + CFG.m_StartingRegion);
            Logger.Log(ConsoleColor.Cyan, "ExperienceMode: " + CFG.m_ExperienceMode);
            Logger.Log(ConsoleColor.Cyan, "VoicePort: " + CFG.m_VoicePort);
            Logger.Log(ConsoleColor.Cyan, "SceneToSpawn: " + CFG.m_SceneToSpawn);
            Logger.Log(ConsoleColor.Cyan, "GameMode: " + CFG.m_GameMode);
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

            PlayersManager.InitilizePlayers(CFG.m_MaxPlayers);

            ModMain.Client.m_IsReady = true;
            ModMain.Client.ProcessAllDelayedPackages();
            MenuHook.RemovePleaseWait();
            ModMain.SetupSurvivalSettings(CFG.m_ExperienceMode, CFG.m_Seed, CFG.m_StartingRegion, CFG.m_SceneToSpawn);

            if(CFG.m_VoicePort != 0)
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
                    Player.KeepVisible();
                } else
                {
                    Player.gameObject.SetActive(false);
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
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, ExtraFloat);
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
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, Velocity, AngularVelocity, Fuse);
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
            Quaternion Quaternion = Reader.GetQuaternionUnity();
            bool RespawnAnim = Reader.GetBool();

            PlayersManager.RespawnOnPoint(Position, Quaternion, RespawnAnim);
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

            if (GotGear)
            {
                string GearName = Reader.GetString();
                string JSON = Reader.GetString();
                GearsSync.HandleGearPickUp(GearName, JSON);
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
            Vector3 Center = Reader.GetVector3Unity();
            float Radius = Reader.GetFloat();

            DangerCircleManager.HandleDangerCircleSync(Center, Radius);
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
        }

        public static void ClientHUDSideBarUpdate(NetDataReader Reader)
        {
            int SideBarIndex = Reader.GetInt();
            string Afix = Reader.GetString();
            GameModeHUD.SetSideLable(SideBarIndex, $" {Afix}");
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
            PlayersManager.FullyCure();
            PlayersManager.ExitVehicleForced();
        }

        public static void ServerLeaders(NetDataReader Reader)
        {
            int Count = Reader.GetInt();
            string Str = "";
            List<string> WinnersNames = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                WinnersNames.Add(PlayersManager.GetPlayerName(Reader.GetInt()));
            }
            Vector3 Position = Reader.GetVector3Unity();

            GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("Victory");
            if (Reference)
            {
                GameObject Obj = UnityEngine.Object.Instantiate(Reference, Position, Quaternion.identity);
                if (Obj)
                {
                    for (int i = 0; i < Count;i++)
                    {
                        Comps.NetworkPlayer PlayerObj = PlayersManager.GetPlayer(i);
                        GameObject VictoryDoll = Obj.transform.GetChild(i).gameObject;
                        if (PlayerObj && VictoryDoll)
                        {
                            VictoryDoll.gameObject.SetActive(true);
                            PlayerObj.m_VisualData.m_ClothingData = PlayersManager.m_LocalPlayerData.m_ClothingData;
                            PlayerObj.UpdateClothing();
                            PlayerObj.CloneMeshSettingsToDummy(VictoryDoll);
                            VictoryDoll.GetComponent<Animator>().SetInteger("VictoryPlace", i+1);
                        }
                        Obj.transform.GetChild(i+3).gameObject.SetActive(true);
                        Obj.transform.GetChild(i+3).GetComponent<TextMeshPro>().SetText(WinnersNames[i]);
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
    }
}
