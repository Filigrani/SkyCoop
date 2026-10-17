using System;
using System.Collections.Generic;
using GameServer;
#if (!DEDICATED)
using MelonLoader;
#endif

namespace SkyCoop
{
    /// <summary>
    /// Support for the "Working Vehicles" mod by stykko (drivable trucks).
    /// Working Vehicles exposes WorkingVehicles.Multiplayer: we fill its delegates and pass on the vehicle states
    /// it produces. There is no hard dependency: without that mod (or with an old version without the hooks) nothing happens.
    /// The host's Working Vehicles keeps every vehicle's state in the host's save; clients ask for it when a vehicle appears.
    /// </summary>
    public static class WorkingVehiclesSync
    {
        // Last state of each vehicle ("scene|orig" -> state), so a host without Working Vehicles can still answer requests.
        static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();

        static string KeyOf(string state)
        {
            // state: "WV1|scene|orig|..."
            string[] p = state.Split(new[] { '|' }, 4);
            return p.Length >= 3 ? p[1] + "|" + p[2] : null;
        }

        /// <summary>Server: a client sent a vehicle state.</summary>
        public static void ServerGotState(int from, string state)
        {
            string key = KeyOf(state);
            if (key == null) return;
            Cache[key] = state;
#if (!DEDICATED)
            Receive(state, from);
#endif
            ServerSend.WVSTATE(from, state, from, -1);
        }

        /// <summary>Server: a client asks for the state of a vehicle ("scene|orig").</summary>
        public static void ServerGotRequest(int from, string request)
        {
#if (!DEDICATED)
            if (Bound)
            {
                Request(request, from);
                return;
            }
#endif
            string state;
            if (Cache.TryGetValue(request, out state)) ServerSend.WVSTATE(0, state, -1, from);
        }

        public static void ResetCache() { Cache.Clear(); }

#if (!DEDICATED)
        static bool tried;
        static Type api;
        static System.Reflection.FieldInfo fActive, fIsHost, fLocalId;
        static Action<string, int> receive, request;
        static bool lastActive, lastHost;
        static int lastId = int.MinValue;

        public static bool Bound { get { return api != null; } }

        static void Bind()
        {
            tried = true;
            try
            {
                foreach (MelonMod m in MelonMod.RegisteredMelons)
                {
                    Type t = m.MelonAssembly.Assembly.GetType("WorkingVehicles.Multiplayer", false);
                    if (t == null) continue;
                    const System.Reflection.BindingFlags S = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
                    fActive = t.GetField("Active", S);
                    fIsHost = t.GetField("IsHost", S);
                    fLocalId = t.GetField("LocalId", S);
                    receive = (Action<string, int>)Delegate.CreateDelegate(typeof(Action<string, int>), t.GetMethod("Receive", S));
                    request = (Action<string, int>)Delegate.CreateDelegate(typeof(Action<string, int>), t.GetMethod("Request", S));
                    t.GetField("SendState", S).SetValue(null, new Action<string>(SendState));
                    t.GetField("SendRequest", S).SetValue(null, new Action<string>(SendRequest));
                    t.GetField("SendAnswer", S).SetValue(null, new Action<string, int>(SendAnswer));
                    api = t;
                    MelonLogger.Msg(ConsoleColor.Green, "[SkyCoop] Working Vehicles found: vehicles are synchronized.");
                    return;
                }
                MelonLogger.Msg("[SkyCoop] Working Vehicles with multiplayer hooks not found: vehicles are not synchronized.");
            }
            catch (Exception e)
            {
                api = null;
                MelonLogger.Warning("[SkyCoop] Could not hook Working Vehicles: " + e.Message);
            }
        }

        /// <summary>Every frame: tells Working Vehicles whether we are online, as host or client, and with which id.</summary>
        public static void Update()
        {
            if (!tried) Bind();
            if (api == null) return;
            bool host = MyMod.iAmHost;
            bool active = host || MyMod.sendMyPosition;
            int id = host ? 0 : ClientUser.myId;
            if (active == lastActive && host == lastHost && id == lastId) return;
            if (!active) ResetCache();
            lastActive = active; lastHost = host; lastId = id;
            try
            {
                fIsHost.SetValue(null, host);
                fLocalId.SetValue(null, id);
                fActive.SetValue(null, active);
            }
            catch (Exception) { }
        }

        static void Receive(string state, int from)
        {
            if (receive != null) receive(state, from);
        }

        static void Request(string req, int from)
        {
            if (request != null) request(req, from);
        }

        // ---- called by Working Vehicles

        static void SendState(string state)
        {
            if (MyMod.iAmHost)
            {
                string key = KeyOf(state);
                if (key != null) Cache[key] = state;
                ServerSend.WVSTATE(0, state, -1, -1);
            }
            else if (MyMod.sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.WVSTATE))
                {
                    _packet.Write(state);
                    MyMod.SendUDPData(_packet);
                }
            }
        }

        static void SendRequest(string req)
        {
            if (MyMod.iAmHost || !MyMod.sendMyPosition) return;
            using (Packet _packet = new Packet((int)ClientPackets.WVREQUEST))
            {
                _packet.Write(req);
                MyMod.SendUDPData(_packet);
            }
        }

        static void SendAnswer(string state, int to)
        {
            if (MyMod.iAmHost && to > 0) ServerSend.WVSTATE(0, state, -1, to);
        }

        /// <summary>Client: a vehicle state arrived from the host.</summary>
        public static void ClientGotState(string state, int from)
        {
            if (!tried) Bind();
            Receive(state, from);
        }
#endif
    }
}
