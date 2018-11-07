using ColossalFramework;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        public override byte ID => 109;

        public override void HandleOnServer(NodeCreateCommand command, Player player) => HandleCreateNode(command);

        public override void HandleOnClient(NodeCreateCommand command) => HandleCreateNode(command);

        private void HandleCreateNode(NodeCreateCommand command)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            Extensions.NodeAndSegmentExtension.VectorDictionary.Add(command.Position, 100); //adds a dummynode to hinder ocilliation
            Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
            Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeID, node); //Adds the NodeID recived and the NodeID generated on NodeCreation.
            Extensions.NodeAndSegmentExtension.VectorDictionary[command.Position] = node;

            switch (MultiplayerManager.Instance.CurrentRole) //returns the newly created Nodes NodeID so that it can be added to the original builders Dictionary
            {
                case MultiplayerRole.Client:
                    {
                        Command.SendToServer(new NodeIDCommand
                        {
                            NodeIDSender = node,
                            NodeIDReciever = command.NodeID
                        });
                        break;
                    }
                case MultiplayerRole.Server:
                    {
                        Command.SendToClients(new NodeIDCommand
                        {
                            NodeIDSender = node,
                            NodeIDReciever = command.NodeID
                        });
                        break;
                    }
            }
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = false;
            //}
        }
    }
}