using Mono.Nat;

namespace SkyCoopServer
{
    public class NetworkHelper : IDisposable
    {
        public int m_Port;
        private Mapping m_MappingPort;
        private INatDevice? m_Device;

        public NetworkHelper(int port, string description)
        {
            m_Port = port;
            m_MappingPort = new Mapping(Protocol.Udp, port, port, 0, description);

            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();
        }

        readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private async void DeviceFound(object? sender, DeviceEventArgs args)
        {
            await locker.WaitAsync();
            try
            {
                if(args.Device.NatProtocol == NatProtocol.Upnp)
                {
                    m_Device = args.Device;
                    Logger.Log(ConsoleColor.DarkGreen, $"[NetworkHelper] External IP: {await m_Device.GetExternalIPAsync()}");

                    await m_Device.CreatePortMapAsync(m_MappingPort);
                    Logger.Log(ConsoleColor.DarkGreen, $"[NetworkHelper] Create upnp port: {m_MappingPort.Protocol}:{m_MappingPort.PublicPort}");
                }
                else
                {
                    Logger.Log(ConsoleColor.DarkRed, "[NetworkHelper] Upnp unsupported on this device");
                }
            }
            finally
            {
                locker.Release();
            }
        }

        public void Dispose()
        {
            if (m_Device != null)
            {
                NatUtility.DeviceFound -= DeviceFound;
                NatUtility.StopDiscovery();
                try
                {
                    m_Device.DeletePortMap(m_MappingPort);
                    Logger.Log(ConsoleColor.DarkGreen, $"[NetworkHelper] Deleting upnp port: {m_MappingPort.Protocol}:{m_MappingPort.PublicPort}");
                }
                catch
                {
                    Logger.Log(ConsoleColor.DarkGreen, $"[NetworkHelper] Couldn't delete specific port");
                }
            }
        }
    }
}
