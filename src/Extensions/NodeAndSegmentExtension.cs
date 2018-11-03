using ColossalFramework;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using ICities;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CSM.Extensions
{
    public class NodeandSegmentExtension : ThreadingExtensionBase
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

        public static bool _NetSegmentLocked = false;
        private bool TreadRunning = false;
        private bool UpdateNetNode = false;
        private bool UpdateNetSegment = false;
        private bool NodeChange = false;
        private bool Initialised = false;
        public static Dictionary<Vector3, ushort> VectorDictionary = new Dictionary<Vector3, ushort>(); //This dictionary contains a combination of a nodes vektor and ID, used to ensure against Nodes ocilliation between server and client
        public static Dictionary<ushort, ushort> NodeIDDictionary = new Dictionary<ushort, ushort>(); // This dictionary contains a combination of The server's and the Clients NodeID, This is used so the reciever can set the StartNode and Endnode of the Segment
        public static Dictionary<StartEndNode, ushort> StartEndNodeDictionary = new Dictionary<StartEndNode, ushort>(); //This dictionary contains a combination of start and end nodes, is used to ensure against NodesSegment ocilliation between server and client
        private Vector3 NonVector = new Vector3(0.0f, 0.0f, 0.0f); //a non vector used to distingish between initialised and non initialised Nodes
        public static NetSegment[] _netSegment = Singleton<NetManager>.instance.m_segments.m_buffer;
        private NetSegment[] _lastNetSegment = new NetSegment[Singleton<NetManager>.instance.m_segments.m_buffer.Length];
        private NetNode[] _netNode = Singleton<NetManager>.instance.m_nodes.m_buffer;
        private NetNode[] _lastNetNode = new NetNode[Singleton<NetManager>.instance.m_nodes.m_buffer.Length];

        public override void OnBeforeSimulationTick()
        {
            base.OnBeforeSimulationTick();
        }

        public override void OnAfterSimulationTick()
        {
            base.OnAfterSimulationTick();

            if (Initialised == false)  //This is run when initiallised, it ensures that the dictionaries are filled and that we can change for later changes
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Server:

                        for (ushort i = 0; i < _netNode.Length; i++)
                        {
                            if (_netNode[i].m_position != NonVector)
                            {
                                VectorDictionary.Add(_netNode[i].m_position, i);
                                NodeIDDictionary.Add(i, i);
                            }
                        }

                        for (ushort i = 0; i < _netSegment.Length; i++)
                        {
                            if (_netSegment[i].m_startNode != 0)
                            {
                                StartEndNode startEndNode = new StartEndNode(_netNode[_netSegment[i].m_startNode].m_position, _netNode[_netSegment[i].m_endNode].m_position);
                                StartEndNodeDictionary.Add(startEndNode, i);
                            }
                        }
                        _netSegment.CopyTo(_lastNetSegment, 0);
                        _netNode.CopyTo(_lastNetNode, 0);

                        Initialised = true;
                        break;

                    case MultiplayerRole.Client:

                        for (ushort i = 0; i < _netNode.Length; i++)
                        {
                            if (_netNode[i].m_position != NonVector)
                            {
                                VectorDictionary.Add(_netNode[i].m_position, i);
                                NodeIDDictionary.Add(i, i);
                            }
                        }

                        for (ushort i = 0; i < _netSegment.Length; i++)
                        {
                            if (_netSegment[i].m_startNode != 0)
                            {
                                StartEndNode startEndNode = new StartEndNode(_netNode[_netSegment[i].m_startNode].m_position, _netNode[_netSegment[i].m_endNode].m_position);
                                StartEndNodeDictionary.Add(startEndNode, i);
                            }
                        }
                        _netSegment.CopyTo(_lastNetSegment, 0);
                        _netNode.CopyTo(_lastNetNode, 0);

                        Initialised = true;
                        break;
                }
            }

            if (TreadRunning == false && Initialised == true)
            {
                TreadRunning = true;
                new Thread(() =>
                {
                    for (int i = 0; i < _netNode.Length; i++) //this checks for changes by cheching if a node has been changed
                    {
                        if (_netNode[i].m_position != _lastNetNode[i].m_position)
                        {
                            NodeChange = true;
                            break;
                        }
                    }

                    if (NodeChange == true)
                        lock (_netNode)
                        {
                            if (_NetSegmentLocked)
                                Thread.Sleep(1000);
                            lock (_netSegment)
                            {
                                for (int i = 0; i < _netNode.Length; i++)
                                {
                                    if (_netNode[i].m_position != _lastNetNode[i].m_position)
                                    {
                                        var position = _netNode[i].m_position;
                                        var infoIndex = _netNode[i].m_infoIndex;
                                        var nodeId = i;
                                        UpdateNetNode = true;

                                        if (!VectorDictionary.ContainsKey(_netNode[i].m_position))
                                        {
                                            VectorDictionary.Add(_netNode[i].m_position, (ushort)i);
                                            switch (MultiplayerManager.Instance.CurrentRole)
                                            {
                                                case MultiplayerRole.Server:
                                                    Command.SendToClients(new NodeCommand
                                                    {
                                                        Position = position,
                                                        InfoIndex = infoIndex,
                                                        NodeID = nodeId
                                                    });
                                                    //UnityEngine.Debug.Log("Command send");
                                                    break;

                                                case MultiplayerRole.Client:
                                                    Command.SendToServer(new NodeCommand
                                                    {
                                                        Position = position,
                                                        InfoIndex = infoIndex,
                                                        NodeID = nodeId
                                                    });
                                                    //UnityEngine.Debug.Log("Command send");
                                                    break;
                                            }
                                            Thread.Sleep(50); //The biggest problem with this implementation is that we need to ensure that the Reciver first recieves and add all the Nodes before the Nodes segments are recieved
                                        }
                                    }
                                }
                                for (int i = 0; i < _netSegment.Length; i++)
                                {
                                    if (_netSegment[i].m_startNode != _lastNetSegment[i].m_startNode)
                                    {
                                        var startnode = _netSegment[i].m_startNode;
                                        var endnode = _netSegment[i].m_endNode;
                                        var startDirection = _netSegment[i].m_startDirection;
                                        var enddirection = _netSegment[i].m_endDirection;
                                        var modifiedIndex = _netSegment[i].m_modifiedIndex;
                                        var infoIndex = _netSegment[i].m_infoIndex;
                                        UpdateNetSegment = true;
                                        StartEndNode startEndNode = new StartEndNode(_netNode[startnode].m_position, _netNode[endnode].m_position);
                                        if (!StartEndNodeDictionary.ContainsKey(startEndNode))
                                        {
                                            StartEndNodeDictionary.Add(startEndNode, (ushort)i);
                                            Thread.Sleep(50); //The biggest problem with this implementation is that we need to ensure that the Reciver first recieves and add all the Nodes before the Nodes segments are recieved
                                            switch (MultiplayerManager.Instance.CurrentRole)
                                            {
                                                case MultiplayerRole.Server:
                                                    Command.SendToClients(new NodeSegmentCommand
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
                                                    Command.SendToServer(new NodeSegmentCommand
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
                                        //UnityEngine.Debug.Log("Command send");
                                        //UnityEngine.Debug.Log($"NetSegment created: {i}");
                                    }
                                }
                                if (UpdateNetSegment)
                                {
                                    _netSegment.CopyTo(_lastNetSegment, 0);
                                    UnityEngine.Debug.Log("NetSegment Updated");
                                    UpdateNetSegment = false;
                                }
                                if (UpdateNetNode)
                                {
                                    _netNode.CopyTo(_lastNetNode, 0);
                                    UnityEngine.Debug.Log("NetNode Updated");
                                    UpdateNetNode = false;
                                }
                            }
                        }
                    TreadRunning = false;
                }).Start();
            }
        }
    }
}