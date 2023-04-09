using ColossalFramework;
using CSM.API.Helpers;
using CSM.BaseGame.Helpers;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndRenderingImpl")]
    public class ToolManagerGeometryRenderer {
        
        public static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            foreach (ToolBase simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                if(simulatedTool is TransportTool) {
                    // TODO: fix transport tool rendering so the tool doesn't break when another client enters the tool
                    continue;
                }
                simulatedTool.RenderGeometry(cameraInfo);
            }
        }

    }

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndOverlayImpl")]
    public class ToolManagerOverlayRenderer {
        
        public static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            // the NetTool uses static fields to share guidelines between render and simulation loops
            // unsetting the static field here will prevent any NetTools from clients also rendering the
            // guidelines again

            FastList<NetTool.HelperLine> currentHelperLines = NetTool.m_helperLines;
            NetTool.m_helperLines = new FastList<NetTool.HelperLine>();

            // var currentDirectionArrows = ReflectionHelper.GetAttr((NetTool)null, "m_directionArrowsBuffer");
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", new FastList<NetTool.HelperDirection2>());

            foreach (ToolBase simulatedTool in Singleton<ToolSimulator>.instance.GetTools())
            {
                switch (simulatedTool)
                {
                    case TransportTool _:
                        // TODO: fix transport tool rendering so the tool doesn't break when another client enters the tool
                        continue;
                    case NetTool _:
                        // Reset angle number
                        ReflectionHelper.Call(simulatedTool, "ShowExtraInfo", new []{typeof(bool), typeof(string), typeof(Vector3)}, false, null, Vector3.zero);
                        break;
                }

                simulatedTool.RenderOverlay(cameraInfo);
            }

            NetTool.m_helperLines = currentHelperLines;
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", currentDirectionArrows);
        }

    }
}
