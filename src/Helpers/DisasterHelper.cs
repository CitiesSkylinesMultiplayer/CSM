namespace CSM.Helpers
{
    public static class DisasterHelper
    {

        private static Dictionary<int, DisasterClient> disasterClientMap = new Dictionary<int, DisasterClient>();

        public static void receiveCreate(int client, ushort server, ushort client)
        {
            if (!disasterClientMap.ContainsKey(client))
            {
                disasterClientMap.Add(client, new DisasterClient());
            }

            DisasterClient client = disasterClientMap[client];
            client.add(server, client);
        }

        public static ushort getLocal(int client, ushort server)
        {
            DisasterClient client = disasterClientMap[client];
            return client.convertServerToLocal(server);
        }

        public static class DisasterClient
        {
            private IDictionary<ushort, ushort> localToServerMap = new IDictionary<ushort, ushort>();
            private IDictionary<ushort, ushort> serverToLocalMap = new IDictionary<ushort, ushort>();

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
