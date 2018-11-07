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
    /// <summary>
    /// This Extensions Syncs Node and SegmentNodes, which is responsible for Roads, Powerlines, Waterpipes ect.
    /// The Nodes are the 'fundamental' building blocks, the NodeSegments contains a start node and an end node and makes a connection between them
    /// It requres that the Clients and Server are using the same loaded game when connecting
    ///
    /// TODO: Change the release function to release nodesegments instead of nodes
    /// </summary>
    public class NodeAndSegmentExtension : ThreadingExtensionBase
    {
        private List<Vector3> _vectorList = new List<Vector3>();

        private bool _nodeReleased = false;
        private bool _treadRunning = false;
        private bool _updateNetNode = false;
        private bool _updateNetSegment = false;
        private bool _nodeChange = false;
        private bool _initialised = false;
        private bool ZoneChange = false;
        private Vector3 _nonVector = new Vector3(0.0f, 0.0f, 0.0f); //a non vector used to distinguish between initialized and non initialized Nodes
        private NetSegment[] NetSegments = Singleton<NetManager>.instance.m_segments.m_buffer;
        private NetSegment[] _lastNetSegment = new NetSegment[Singleton<NetManager>.instance.m_segments.m_buffer.Length];
        private NetNode[] _netNode = Singleton<NetManager>.instance.m_nodes.m_buffer;
        private NetNode[] _lastNetNode = new NetNode[Singleton<NetManager>.instance.m_nodes.m_buffer.Length];
        private ZoneBlock[] _zoneBlock = Singleton<ZoneManager>.instance.m_blocks.m_buffer;
        private ZoneBlock[] _lastZoneBlock = new ZoneBlock[Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length];

        public static Dictionary<Vector3, ushort> VectorDictionary = new Dictionary<Vector3, ushort>(); //This dictionary contains a combination of a nodes vector and ID, used to ensure against Nodes ocilliation between server and client
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
                                VectorDictionary.Add(_netNode[i].m_position, i);
                                NodeIDDictionary.Add(i, i);
                            }
                        }

                        for (ushort i = 0; i < NetSegments.Length; i++)
                        {
                            if (NetSegments[i].m_startNode != 0)
                            {
                                StartEndNode startEndNode = new StartEndNode(_netNode[NetSegments[i].m_startNode].m_position, _netNode[NetSegments[i].m_endNode].m_position);
                                StartEndNodeDictionary.Add(startEndNode, i);
                            }
                        }
                        NetSegments.CopyTo(_lastNetSegment, 0);
                        _netNode.CopyTo(_lastNetNode, 0);

                        _initialised = true;
                        break;

                    case MultiplayerRole.Client:

                        for (ushort i = 0; i < _netNode.Length; i++)
                        {
                            if (_netNode[i].m_position != _nonVector)
                            {
                                VectorDictionary.Add(_netNode[i].m_position, i);
                                NodeIDDictionary.Add(i, i);
                            }
                        }

                        for (ushort i = 0; i < NetSegments.Length; i++)
                        {
                            if (NetSegments[i].m_startNode != 0)
                            {
                                StartEndNode startEndNode = new StartEndNode(_netNode[NetSegments[i].m_startNode].m_position, _netNode[NetSegments[i].m_endNode].m_position);
                                StartEndNodeDictionary.Add(startEndNode, i);
                            }
                        }
                        NetSegments.CopyTo(_lastNetSegment, 0);
                        _netNode.CopyTo(_lastNetNode, 0);
                        _zoneBlock.CopyTo(_lastZoneBlock, 0);

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
                    foreach (var Id in VectorDictionary) // this checks if any nodes has been removed, by controlling if any of the nodes that we have created, has been deleted
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
                    /*
					for (int i = 0; i < Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length; i++)
					{
						if (_zoneBlock[i].m_position != _lastZoneBlock[i].m_position)
						{
							UnityEngine.Debug.Log("Zone Changed");
							Singleton<ZoneManager>.instance.m_blocks;
							Singleton<ZoneManager>.instance.m_blocks.m_buffer[1].m_valid
							ZoneChange = true;
							break;
						}
					}
					*/

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

                                if (!VectorDictionary.ContainsKey(_netNode[i].m_position))
                                {
                                    VectorDictionary.Add(_netNode[i].m_position, (ushort)i);
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
                        for (int i = 0; i < NetSegments.Length; i++)
                        {
                            if (NetSegments[i].m_startNode != _lastNetSegment[i].m_startNode | NetSegments[i].m_endNode != _lastNetSegment[i].m_endNode)
                            {
                                var startnode = NetSegments[i].m_startNode;
                                var endnode = NetSegments[i].m_endNode;
                                var startDirection = NetSegments[i].m_startDirection;
                                var enddirection = NetSegments[i].m_endDirection;
                                var modifiedIndex = NetSegments[i].m_modifiedIndex;
                                var infoIndex = NetSegments[i].m_infoIndex;
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
                            NetSegments.CopyTo(_lastNetSegment, 0);
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
                        foreach (var Id in VectorDictionary)
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
                                _vectorList.Add(Id.Key);
                                foreach (var ID in NodeIDDictionary.Where(kvp => kvp.Value == Id.Value).ToList())
                                {
                                    NodeIDDictionary.Remove(Id.Value);
                                }
                            }
                        };
                        foreach (var vector in _vectorList)
                        {
                            VectorDictionary.Remove(vector);
                        }
                        _vectorList.Clear();
                        _nodeReleased = false;
                    }
                    _treadRunning = false;
                }).Start();
            }
        }
    }
}