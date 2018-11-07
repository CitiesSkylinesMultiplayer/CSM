﻿using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler
{
	public class ZoneCreateHandler : CommandHandler<ZoneCreateCommand>
	{
		public override byte ID => 114;

		public override void HandleOnServer(ZoneCreateCommand command, Player player) => Handle(command);

		public override void HandleOnClient(ZoneCreateCommand command) => Handle(command);

		private void Handle(ZoneCreateCommand command)
		{
			var id = Extensions.NodeAndSegmentExtension.ZoneVectorDictionary[command.Position];
			Singleton<ZoneManager>.instance.m_blocks.m_buffer[id].m_zone1 = command.Zone1;
			Singleton<ZoneManager>.instance.m_blocks.m_buffer[id].m_zone2 = command.Zone2;
			Singleton<ZoneManager>.instance.m_blocks.m_buffer[id].RefreshZoning(id);  //this Command is neccesery to get the Zone to render, else it will first show when a building is build on it
		}
	}
}