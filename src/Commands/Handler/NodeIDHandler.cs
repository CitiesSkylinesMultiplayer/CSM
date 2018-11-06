using ColossalFramework;
using CSM.Networking;
using UnityEngine;


namespace CSM.Commands.Handler
{
	class NodeIDHandler : CommandHandler<NodeIDCommand>
	{
		public override byte ID => 113;


		public override void HandleOnServer(NodeIDCommand command, Player player) => HandleNodeID(command);

		public override void HandleOnClient(NodeIDCommand command) => HandleNodeID(command);

		private void HandleNodeID(NodeIDCommand command)
		{
			Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeIDSender, (ushort)command.NodeIDReciever);
		}
	}
}
