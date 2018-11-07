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
        /// TODO make the way it handles sending things between server and client in the right order.
        /// It has to be ensured that all Nodes are send and added before any NodeSegments are send and added.
        ///
        /// </summary>

        private List<Vector3> VectorList = new List<Vector3>();

        private bool _nodeReleased = false;
        private bool _treadRunning = false;
        private bool _updateNetNode = false;
        private bool _updateNetSegment = false;
        private bool _nodeChange = false;
        private bool _initialised = false;
        private Vector3 _nonVector = new Vector3(0.0f, 0.0f, 0.0f); //a non vector used to distinguish between initialized and non initialized Nodes

        private NetSegment[] _lastNetSegment = new NetSegment[Singleton<NetManager>.instance.m_segments.m_buffer.Length];

        private NetNode[] _netNode = Singleton<NetManager>.instance.m_nodes.m_buffer;
        private NetNode[] _lastNetNode = new NetNode[Singleton<NetManager>.instance.m_nodes.m_buffer.Length];

        public static bool NetSegmentLocked = false;

        public static Dictionary<Vector3, ushort> VectorDictionary = new Dictionary<Vector3, ushort>(); //This dictionary contains a combination of a nodes vector and ID, used to ensure against Nodes ocilliation between server and client
        public static Dictionary<ushort, ushort> NodeIDDictionary = new Dictionary<ushort, ushort>(); // This dictionary contains a combination of The server's and the Clients NodeID, This is used so the receiver can set the StartNode and Endnode of the Segment
        public static Dictionary<StartEndNode, ushort> StartEndNodeDictionary = new Dictionary<StartEndNode, ushort>(); //This dictionary contains a combination of start and end nodes, is used to ensure against NodesSegment ocilliation between server and client

        public static NetSegment[] NetSegments = Singleton<NetManager>.instance.m_segments.m_buffer;

        public override void OnBeforeSimulationTick()
        {
            base.OnBeforeSimulationTick();
        }

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

                        _initialised = true;
                        break;
                }
            }

            if (_treadRunning == false && _initialised == true)
            {
                _treadRunning = true;
                new Thread(() =>
                {
                    Thread.Sleep(100);
                    for (int i = 0; i < _netNode.Length; i++) //this checks for changes by cheching if a node has been changed
                    {
                        if (_netNode[i].m_position != _lastNetNode[i].m_position)
                        {
                            _nodeChange = true;
                            break;
                        }
                    }
                    foreach (var Id in VectorDictionary)
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
                        Thread.Sleep(300);
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
                        //}
                        //	}
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
                                VectorList.Add(Id.Key);
                                foreach (var ID in NodeIDDictionary.Where(kvp => kvp.Value == Id.Value).ToList())
                                {
                                    NodeIDDictionary.Remove(Id.Value);
                                }
                            }
                        };
                        foreach (var vector in VectorList)
                        {
                            VectorDictionary.Remove(vector);
                        }
                        VectorList.Clear();
                        _nodeReleased = false;
                    }
                    _treadRunning = false;
                }).Start();
            }
        }
    }
}