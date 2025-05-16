using Il2Cpp;
using LiteNetLib.Utils;
using SkyCoopClient;
using SkyCoopServer;
using UnityEngine;
using static Il2CppParadoxNotion.Services.Logger;

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

        public static void ServerConfig(NetDataReader Reader)
        {
            int PlayersMax = Reader.GetInt();
            int Seed = Reader.GetInt();
            string StartingRegion = Reader.ReadString();
            string GameMode = Reader.ReadString();
            int VoicePort = Reader.GetInt();
            string SpawnScene = Reader.GetString();

            PlayersManager.InitilizePlayers(PlayersMax);

            Logger.Log(ConsoleColor.Cyan, "Server config");
            Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + PlayersMax);
            Logger.Log(ConsoleColor.Cyan, "Seed: " + Seed);
            Logger.Log(ConsoleColor.Cyan, "StartingRegion: "+ StartingRegion);
            Logger.Log(ConsoleColor.Cyan, "GameMode: " + GameMode);
            Logger.Log(ConsoleColor.Cyan, "VoicePort: " + VoicePort);
            Logger.Log(ConsoleColor.Cyan, "SceneToSpawn: " + SpawnScene);

            ModMain.Client.m_IsReady = true;
            ModMain.Client.ProcessAllDelayedPackages();
            MenuHook.RemovePleaseWait();
            ModMain.SetupSurvivalSettings(GameMode, Seed, StartingRegion, SpawnScene);

            if(VoicePort != 0)
            {
                Task.Run(() => { ModMain.Client.ConnectToServerVoice(VoicePort); });
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

        public static void ClientPosition(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Vector3 Position = Reader.ReadVector3Unity();

            Comps.NetworkPlayer Player = PlayersManager.GetPlayer(PlayerID);
            if(Player)
            {
                Player.SetPosition(Position);
            }
        }

        public static void ClientRotation(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            Quaternion Rotation = Reader.ReadQuaternionUnity();

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
            string GearName = Reader.ReadString();
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
            Vector3 Pos = Reader.ReadVector3Unity();
            Quaternion Rot = Reader.ReadQuaternionUnity();
            string ProjectileName = Reader.GetString();
            float ExtraFloat = Reader.GetFloat();
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, ExtraFloat);
        }
        public static void ClientProjectileThrow(NetDataReader Reader)
        {
            int ShooterID = Reader.GetInt();
            Vector3 Pos = Reader.ReadVector3Unity();
            Quaternion Rot = Reader.ReadQuaternionUnity();
            string ProjectileName = Reader.GetString();
            Vector3 Velocity = Reader.ReadVector3Unity();
            Vector3 AngularVelocity = Reader.ReadVector3Unity();
            float Fuse = Reader.GetFloat();
            WeaponsManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName, Velocity, AngularVelocity, Fuse);
        }
        public static void KillFeedMessage(NetDataReader Reader)
        {
            DataStr.KillFeedMessage Message = Reader.ReadKillFeedMessage();
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
            Vector3 Position = Reader.ReadVector3Unity();
            Quaternion Quaternion = Reader.ReadQuaternionUnity();
            bool RespawnAnim = Reader.GetBool();

            PlayersManager.RespawnOnPoint(Position, Quaternion, RespawnAnim);
        }

        public static void ClientInjectedItem(NetDataReader Reader)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int ObjectID = Reader.GetInt();
            Vector3 Position = Reader.ReadVector3Unity();
            Quaternion Rotation = Reader.ReadQuaternionUnity();

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
            DataStr.GearDataVisual Visual = Reader.ReadGearVisual();

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
    }
}
