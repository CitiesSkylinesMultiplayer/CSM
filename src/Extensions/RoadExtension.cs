namespace CSM.Extensions
{
    /*
	public class RoadExtension: ThreadingExtensionBase
	{
		NetSegment[] _lastSegment = Singleton<NetManager>.instance.m_segments.m_buffer;

		public override void OnBeforeSimulationTick()
		{
			base.OnBeforeSimulationTick();
		}
		public override void OnAfterSimulationTick()
		{
			base.OnAfterSimulationTick();

			if (!_lastSegment.SequenceEqual(Singleton<NetManager>.instance.m_segments.m_buffer))
			{
				NetManager netManager = Singleton<NetManager>.instance;
				NetSegment[] netSegments = netManager.m_segments.m_buffer; ;
				for (int segmentID = 0; segmentID < netSegments.Length; segmentID++)
				{
					NetSegment netSegment = netSegments[segmentID];
					var startnode = netSegment.m_startNode;
					var endnode = netSegment.m_endNode;
					var startDirection = netSegment.m_startDirection;
					var enddirection = netSegment.m_endDirection;
					var modifiedIndex = netSegment.m_modifiedIndex;
					var infoIndex = netSegment.m_infoIndex;

					switch (MultiplayerManager.Instance.CurrentRole)
					{
						case MultiplayerRole.Server:
							MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.RoadCommandID, new RoadCommand
							{
								Startnode = startnode,
								Endnode = endnode,
								StartDirection = startDirection,
								Enddirection = enddirection,
								ModifiedIndex = modifiedIndex,
								InfoIndex = infoIndex
							});
							break;
					}
				}

				_lastSegment = Singleton<NetManager>.instance.m_segments.m_buffer;
			}
		}
	}
		*/
}