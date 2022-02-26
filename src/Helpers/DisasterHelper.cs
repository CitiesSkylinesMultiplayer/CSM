using System.Collections.Generic;

namespace CSM.Helpers
{
    public static class DisasterHelper
    {

        private static Dictionary<int, DisasterClient> disasterClientMap = new Dictionary<int, DisasterClient>();

        public static void receiveCreate(int id, ushort server, ushort client)
        {
            if (!disasterClientMap.ContainsKey(id))
            {
                disasterClientMap.Add(id, new DisasterClient());
            }

            DisasterClient disaster = disasterClientMap[id];
            disaster.add(server, client);
        }

        public static ushort getLocal(int client, ushort server)
        {
            DisasterClient disaster = disasterClientMap[client];
            return disaster.convertServerToLocal(server);
        }

        public class DisasterClient
        {
            private IDictionary<ushort, ushort> localToServerMap = new Dictionary<ushort, ushort>();
            private IDictionary<ushort, ushort> serverToLocalMap = new Dictionary<ushort, ushort>();

            public ushort convertServerToLocal(ushort data)
            {
                return serverToLocalMap[data];
            }

            public ushort convertLocalToServer(ushort data)
            {
                return localToServerMap[data];
            }

            public void add(ushort server, ushort local)
            {
                localToServerMap.Add(local, server);
                serverToLocalMap.Add(server, local);
            }

            public void removeLocal(ushort local)
            {
                ushort server = convertLocalToServer(local);
                localToServerMap.Remove(local);
                serverToLocalMap.Remove(server);
            }

            public void removeServer(ushort server)
            {
                ushort local = convertServerToLocal(server);
                localToServerMap.Remove(local);
                serverToLocalMap.Remove(server);
            }
        }
    }
}
