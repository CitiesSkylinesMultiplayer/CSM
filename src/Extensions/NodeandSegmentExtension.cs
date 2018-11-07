using ColossalFramework;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using ICities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace CSM.Extensions
{
	public class NodeAndSegmentExtension : ThreadingExtensionBase
	{
		/// <summary>
		/// This Extensions Syncs Node and SegmentNodes, which is responsible for Roads, Powerlines, Waterpipes ect.
		/// The Nodes are the 'fundamental' building blocks, the NodeSegments contains a start node and an end node and makes a connection between them
		/// It requres that the Clients and Server are using the same loaded game when connecting
		/// 
		/// This also sync the different Zones
		///
		/// TODO: Change the release function to release nodesegments instead of nodes
		///
		/// TODO: Split this code into a detector that detects changes, and other extensions that are called when a change is detected
		///
		/// </summary>

		private List<Vector3> VectorList = new List<Vector3>();

		private bool _nodeReleased = false;
		private bool _treadRunning = false;
		private bool _updateNetNode = false;
		private bool _updateNetSegment = false;
		private bool _nodeChange = false;
		private bool _initialised = false;
		private bool ZoneChange = false;
		private Vector3 _nonVector = new Vector3(0.0f, 0.0f, 0.0f); //a non vector used to distinguish between initialized and non initialized Nodes
		private NetSegment[] _NetSegments = Singleton<NetManager>.instance.m_segments.m_buffer;
		private NetSegment[] _lastNetSegment = new NetSegment[Singleton<NetManager>.instance.m_segments.m_buffer.Length];
		private NetNode[] _netNode = Singleton<NetManager>.instance.m_nodes.m_buffer;
		private NetNode[] _lastNetNode = new NetNode[Singleton<NetManager>.instance.m_nodes.m_buffer.Length];
		private ZoneBlock[] _ZoneBlock = Singleton<ZoneManager>.instance.m_blocks.m_buffer;
		private ZoneBlock[] _LastZoneBlock = new ZoneBlock[Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length];


		public static Dictionary<Vector3, ushort> ZoneVectorDictionary = new Dictionary<Vector3, ushort>(); // This dictionary contains ZoneVectors and ZoneId and are used to identify the zoneId of a recived zone

		public static Dictionary<Vector3, ushort> NodeVectorDictionary = new Dictionary<Vector3, ushort>(); //This dictionary contains a combination of a nodes vector and ID, used to ensure against Nodes ocilliation between server and client
		public static Dictionary<ushort, ushort> NodeIDDictionary = new Dictionary<ushort, ushort>(); // This dictionary contains a combination of The server's and the Clients NodeID, This is used so the receiver can set the StartNode and Endnode of the Segment
		public static Dictionary<StartEndNode, ushort> StartEndNodeDictionary = new Dictionary<StartEndNode, ushort>(); //This dictionary contains a combination of start and end nodes, is used to ensure against NodesSegment ocilliation between server and client




		public override void OnAfterSimulationTick()
		{
			base.OnAfterSimulationTick();

			if (_initialised == false)  //This is run when initialized, it ensures that the dictionaries are filled and that we can change for later changes
			{
				switch (MultiplayerManager.Instance.CurrentRole)
				{
					case MultiplayerRole.Server:

						for (ushort i = 0; i < _netNode.Length; i++)
						{
							if (_netNode[i].m_position != _nonVector)
							{
								NodeVectorDictionary.Add(_netNode[i].m_position, i);
								NodeIDDictionary.Add(i, i);
							}
						}

						for (ushort i = 0; i < _NetSegments.Length; i++)
						{
							if (_NetSegments[i].m_startNode != 0)
							{
								StartEndNode startEndNode = new StartEndNode(_netNode[_NetSegments[i].m_startNode].m_position, _netNode[_NetSegments[i].m_endNode].m_position);
								StartEndNodeDictionary.Add(startEndNode, i);
							}
						}
						for (ushort i = 0; i < _ZoneBlock.Length; i++)
						{
							if (_ZoneBlock[i].m_position != _nonVector)
							{
								ZoneVectorDictionary.Add(_ZoneBlock[i].m_position, i);
							}
						}


						_NetSegments.CopyTo(_lastNetSegment, 0);
						_netNode.CopyTo(_lastNetNode, 0);
						_ZoneBlock.CopyTo(_LastZoneBlock, 0);


						_initialised = true;
						break;

					case MultiplayerRole.Client:

						for (ushort i = 0; i < _netNode.Length; i++)
						{
							if (_netNode[i].m_position != _nonVector)
							{
								NodeVectorDictionary.Add(_netNode[i].m_position, i);
								NodeIDDictionary.Add(i, i);
							}
						}

						for (ushort i = 0; i < _NetSegments.Length; i++)
						{
							if (_NetSegments[i].m_startNode != 0)
							{
								StartEndNode startEndNode = new StartEndNode(_netNode[_NetSegments[i].m_startNode].m_position, _netNode[_NetSegments[i].m_endNode].m_position);
								StartEndNodeDictionary.Add(startEndNode, i);
							}
						}

						for (ushort i = 0; i < _ZoneBlock.Length; i++)
						{
							if (_ZoneBlock[i].m_position != _nonVector)
							{
								ZoneVectorDictionary.Add(_ZoneBlock[i].m_position, i);
							}
						}

						_NetSegments.CopyTo(_lastNetSegment, 0);
						_netNode.CopyTo(_lastNetNode, 0);
						_ZoneBlock.CopyTo(_LastZoneBlock, 0);

						_initialised = true;
						break;
				}
			}

			if (_treadRunning == false && _initialised == true)
			{
				_treadRunning = true;
				new Thread(() =>
				{
					for (int i = 0; i < _netNode.Length; i++) //this checks if any new nodes has been created by cheching if any nodes positions has been changed
					{
						if (_netNode[i].m_position != _lastNetNode[i].m_position)
						{
							_nodeChange = true;
							break;
						}
					}
					foreach (var Id in NodeVectorDictionary) // this checks if any nodes has been removed, by controlling if any of the nodes that we have created, has been deleted
					{
						if (_nodeChange == true)
							break;

						ushort value = Id.Value;
						if (_netNode[value].m_flags == 0)
						{
							_nodeReleased = true;
							break;
						}
					}

					for (ushort i = 0; i < Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length; i++)  
					{
						if (_nodeChange == true | _nodeReleased == true)
							break;

						if (_ZoneBlock[i].m_position != _nonVector && !ZoneVectorDictionary.ContainsKey(_ZoneBlock[i].m_position)) // The zones are created when road is created, this detect all zoon created and adds it to dictionary
							ZoneVectorDictionary.Add(_ZoneBlock[i].m_position, i); 

						if (_ZoneBlock[i].m_zone1 != _LastZoneBlock[i].m_zone1 | _ZoneBlock[i].m_zone2 != _LastZoneBlock[i].m_zone2) //this checks if anything have changed
						{
							ZoneChange = true;
							break;
						}
					}

					if (_nodeChange == true)
					{
						for (uint i = 0; i < _netNode.Length; i++)
						{
							if (_netNode[i].m_position != _lastNetNode[i].m_position)
							{
								var position = _netNode[i].m_position;
								var infoIndex = _netNode[i].m_infoIndex;
								var nodeId = i;
								_updateNetNode = true;

								if (!NodeVectorDictionary.ContainsKey(_netNode[i].m_position))
								{
									NodeVectorDictionary.Add(_netNode[i].m_position, (ushort)i);
									switch (MultiplayerManager.Instance.CurrentRole)
									{
										case MultiplayerRole.Server:
											Command.SendToClients(new NodeCreateCommand
											{
												Position = position,
												InfoIndex = infoIndex,
												NodeId = nodeId
											});
											break;

										case MultiplayerRole.Client:
											Command.SendToServer(new NodeCreateCommand
											{
												Position = position,
												InfoIndex = infoIndex,
												NodeId = nodeId
											});
											break;
									}
								}
							}
						}
						for (int i = 0; i < _NetSegments.Length; i++)
						{
							if (_NetSegments[i].m_startNode != _lastNetSegment[i].m_startNode | _NetSegments[i].m_endNode != _lastNetSegment[i].m_endNode)
							{
								var startnode = _NetSegments[i].m_startNode;
								var endnode = _NetSegments[i].m_endNode;
								var startDirection = _NetSegments[i].m_startDirection;
								var enddirection = _NetSegments[i].m_endDirection;
								var modifiedIndex = _NetSegments[i].m_modifiedIndex;
								var infoIndex = _NetSegments[i].m_infoIndex;
								_updateNetSegment = true;
								StartEndNode startEndNode = new StartEndNode(_netNode[startnode].m_position, _netNode[endnode].m_position);
								if (!StartEndNodeDictionary.ContainsKey(startEndNode))
								{
									StartEndNodeDictionary.Add(startEndNode, (ushort)i);

									switch (MultiplayerManager.Instance.CurrentRole)
									{
										case MultiplayerRole.Server:
											Command.SendToClients(new NodeSegmentCreateCommand
											{
												StartNode = startnode,
												EndNode = endnode,
												StartDirection = startDirection,
												EndDirection = enddirection,
												ModifiedIndex = modifiedIndex,
												InfoIndex = infoIndex
											});
											break;

										case MultiplayerRole.Client:
											Command.SendToServer(new NodeSegmentCreateCommand
											{
												StartNode = startnode,
												EndNode = endnode,
												StartDirection = startDirection,
												EndDirection = enddirection,
												ModifiedIndex = modifiedIndex,
												InfoIndex = infoIndex
											});
											break;
									}
								}
							}
						}
						if (_updateNetSegment)
						{
							_NetSegments.CopyTo(_lastNetSegment, 0);
							_updateNetSegment = false;
						}
						if (_updateNetNode)
						{
							_netNode.CopyTo(_lastNetNode, 0);
							_updateNetNode = false;
						}
						UnityEngine.Debug.Log("Done Updating");

						_nodeChange = false;
					}

					if (_nodeReleased == true)
					{
						foreach (var Id in NodeVectorDictionary)
						{
							if (_netNode[Id.Value].m_flags == 0)
							{
								switch (MultiplayerManager.Instance.CurrentRole)
								{
									case MultiplayerRole.Server:
										Command.SendToClients(new NodeReleaseCommand
										{
											NodeId = Id.Value
										});
										break;

									case MultiplayerRole.Client:
										Command.SendToServer(new NodeReleaseCommand
										{
											NodeId = Id.Value
										});
										break;
								}
								VectorList.Add(Id.Key);
								foreach (var ID in NodeIDDictionary.Where(kvp => kvp.Value == Id.Value).ToList())
								{
									NodeIDDictionary.Remove(Id.Value);
								}
							}
						};
						foreach (var vector in VectorList)
						{
							NodeVectorDictionary.Remove(vector);
						}
						VectorList.Clear();
						_nodeReleased = false;
					}

					if (ZoneChange == true)
					{
						for (ushort i = 0; i < Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length; i++)
						{
							if (_ZoneBlock[i].m_zone1 != _LastZoneBlock[i].m_zone1 | _ZoneBlock[i].m_zone2 != _LastZoneBlock[i].m_zone2)   //this runs through all Zoneblocks and detect if the zonetype has changed
							{

								Command.SendToAll(new ZoneCreateCommand
								{
									Position = _ZoneBlock[i].m_position,
									Zone1 = _ZoneBlock[i].m_zone1,
									Zone2 = _ZoneBlock[i].m_zone2
								});
							}
						}
						_ZoneBlock.CopyTo(_LastZoneBlock, 0);
					ZoneChange = false;
					}

					_treadRunning = false;
				}).Start();
			}
		}
	}
}