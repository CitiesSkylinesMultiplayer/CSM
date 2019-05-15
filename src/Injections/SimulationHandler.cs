using ColossalFramework;
using CSM.Commands;
using CSM.Extensions;
using Harmony;
using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static NetTool;
using static ToolBase;
using System.Collections.Generic;
using System.Linq;
using CSM.Networking;

namespace CSM.Injections
{
    class SimulationHandler
    {
        public static bool IgnoreAll { get; set; } = false;
    }

    [HarmonyPatch(typeof(SimulationManager))]
    [HarmonyPatch("SimulationStep")]
    public class SimulationStep //once something is created (building, road, etc) sometimes its properties or edited afterwards, for examples road pillars are given extra flags
    {

        public static void Postfix() //now if there are any commands (because this is called a lot without it actually creating things), include extra things (just the building flags for now but this can be applied to more cases)
        {
            if (SimulationHandler.IgnoreAll)
                return;


            BuildingHandler.HoldAll = false;
            foreach (CommandBase cmd in BuildingHandler.queuedCommands)
            {
                if (cmd is BuildingCreateCommand)
                {
                    BuildingCreateCommand bcmd = (BuildingCreateCommand)cmd;
                    Building building = BuildingManager.instance.m_buildings.m_buffer[bcmd.BuildingID];
                    Command.SendToAll(new BuildingCreateExtendedCommand //can we add optional parameters to the normal version of the command? i'll just use this a separate command for now. this should be cleaned up later.
                    {
                        BuildingID = bcmd.BuildingID,
                        Position = bcmd.Position,
                        Infoindex = bcmd.Infoindex,
                        Angle = bcmd.Angle,
                        Length = bcmd.Length,
                        Flags = building.m_flags //this is the important additional thing in the case of the road pillar fuckery (the fixed height flags and untouchable aren't being synced so the pillar's height is being moved to the terrain instead of road height)
                    });
                } else
                {
                    Command.SendToAll(cmd);
                }
            }
            BuildingHandler.queuedCommands.Clear();
        }
    } // yes I realize we could just check if they are pillars and just set the flags since we now know this is the cause, but this is far more extensible to countless other situations and should work with mods that add similar things

    [HarmonyPatch(typeof(NetManager))]
    [HarmonyPatch("SimulationStep")]
    public class NetStep
    {
        public static void Prefix()
        {
            BuildingHandler.HoldAll = true;
        }

        public static void Postfix()
        {
            BuildingHandler.HoldAll = false;
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("SimulationStep")]
    public class BuildingStep
    {
        public static void Prefix()
        {
            BuildingHandler.HoldAll = true;
        }

        public static void Postfix()
        {
            BuildingHandler.HoldAll = false;
        }
    }
}
