using ColossalFramework;
using CSM.Commands;
using ICities;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CSM.Extensions
{
    /// <summary>
    ///     This Extensions syncs zones.
    /// </summary>
    public class ZoneExtension : ThreadingExtensionBase
    {
        private bool _treadRunning = false;
        private bool _initialised = false;
        private bool ZoneChange = false;
        private bool ZoneReleased = false;
        private Vector3 _nonVector = new Vector3(0.0f, 0.0f, 0.0f); //a non vector used to distinguish between initialized and non initialized Nodes
        private ZoneBlock[] _ZoneBlock = Singleton<ZoneManager>.instance.m_blocks.m_buffer;
        private ZoneBlock[] _LastZoneBlock = new ZoneBlock[Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length];

        public static Dictionary<Vector3, ushort> ZoneVectorDictionary = new Dictionary<Vector3, ushort>(); // This dictionary contains ZoneVectors and ZoneId and are used to identify the zoneId of a recived zone
        private List<Vector3> tempZoneRemovalList = new List<Vector3>();

        public override void OnAfterSimulationTick()
        {
            base.OnAfterSimulationTick();

            if (_initialised == false)  //This is run when initialized, it ensures that the dictionaries are filled and that we can change for later changes
            {
                for (ushort i = 0; i < _ZoneBlock.Length; i++)
                {
                    if (_ZoneBlock[i].m_position != _nonVector)
                    {
                        ZoneVectorDictionary.Add(_ZoneBlock[i].m_position, i);
                    }
                }
                _ZoneBlock.CopyTo(_LastZoneBlock, 0);

                _initialised = true;
            }

            if (_treadRunning == false && _initialised == true)
            {
                _treadRunning = true;
                new Thread(() =>
                {
                    foreach (var id in ZoneVectorDictionary) //removes uninitialized Zones from the ZoneVectorDictionary
                    {
                        if (_ZoneBlock[id.Value].m_flags == 0)
                        {
                            ZoneReleased = true;
                            break;
                        }
                    }

                    for (ushort i = 0; i < Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length; i++) // this checks if any zones has been added
                    {
                        if (ZoneReleased == true)
                            break;

                        if (_ZoneBlock[i].m_flags != 0 && !ZoneVectorDictionary.ContainsKey(_ZoneBlock[i].m_position)) // The zones are created when road is created, this detect all zoon created and adds it to dictionary
                            ZoneVectorDictionary.Add(_ZoneBlock[i].m_position, i);

                        if (_ZoneBlock[i].m_zone1 != _LastZoneBlock[i].m_zone1 | _ZoneBlock[i].m_zone2 != _LastZoneBlock[i].m_zone2) //this checks if anything have changed
                        {
                            ZoneChange = true;
                            break;
                        }
                    }

                    if (ZoneReleased == true)
                    {
                        foreach (var id in ZoneVectorDictionary)
                        {
                            if (_ZoneBlock[id.Value].m_flags == 0)
                            {
                                tempZoneRemovalList.Add(id.Key);
                            }
                        }
                        foreach (var id in tempZoneRemovalList)
                        {
                            UnityEngine.Debug.Log("removes zone from dictionary");
                            ZoneVectorDictionary.Remove(id);
                        }
                        tempZoneRemovalList.Clear();
                        ZoneReleased = false;
                    }

                    if (ZoneChange == true)
                    {
                        for (ushort i = 0; i < Singleton<ZoneManager>.instance.m_blocks.m_buffer.Length; i++)
                        {
                            if (_ZoneBlock[i].m_zone1 != _LastZoneBlock[i].m_zone1 | _ZoneBlock[i].m_zone2 != _LastZoneBlock[i].m_zone2)   //this runs through all Zoneblocks and detect if the zonetype has changed
                            {
                                UnityEngine.Debug.Log("zone changed");
                                Command.SendToAll(new ZoneCommand
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