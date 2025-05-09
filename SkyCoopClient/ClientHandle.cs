using LiteNetLib.Utils;
using SkyCoopServer;
using UnityEngine;

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

            PlayersManager.InitilizePlayers(PlayersMax);

            Logger.Log(ConsoleColor.Cyan, "Server config");
            Logger.Log(ConsoleColor.Cyan, "PlayersMax: " + PlayersMax);
            Logger.Log(ConsoleColor.Cyan, "Seed: " + Seed);
            Logger.Log(ConsoleColor.Cyan, "StartingRegion: "+ StartingRegion);
            Logger.Log(ConsoleColor.Cyan, "GameMode: " + GameMode);
            Logger.Log(ConsoleColor.Cyan, "VoicePort: " + VoicePort);

            ModMain.Client.m_IsReady = true;
            MenuHook.RemovePleaseWait();
            ModMain.SetupSurvivalSettings(GameMode, Seed, StartingRegion);

            if(VoicePort != 0)
            {
                Task.Run(() => { ModMain.Client.ConnectToServerVoice(VoicePort); });
            }
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
            bool Melee = Reader.GetBool();
            string MeleeName = Reader.GetString();
            PlayersManager.OtherPlayerDamageMe(Damage, PlayerID, BodyPart, Melee, MeleeName);
        }
        public static void ClientProjectile(NetDataReader Reader)
        {
            int ShooterID = Reader.GetInt();
            Vector3 Pos = Reader.ReadVector3Unity();
            Quaternion Rot = Reader.ReadQuaternionUnity();
            string ProjectileName = Reader.GetString();
            PlayersManager.HandleProjectileSync(ShooterID, Pos, Rot, ProjectileName);
        }
    }
}
