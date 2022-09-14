using ColossalFramework;
using CSM.BaseGame.Helpers;
using HarmonyLib;

namespace CSM.BaseGame.Injections.Tools
{

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndRenderingImpl")]
    public class ToolManagerGeometryRenderer {
        
        public static void Prefix(RenderManager.CameraInfo cameraInfo)
        {
            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
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

            var currentHelperLines = NetTool.m_helperLines;
            NetTool.m_helperLines = new FastList<NetTool.HelperLine>();

            // var currentDirectionArrows = ReflectionHelper.GetAttr((NetTool)null, "m_directionArrowsBuffer");
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", new FastList<NetTool.HelperDirection2>());

            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                if(simulatedTool is TransportTool) {
                    // TODO: fix transport tool rendering so the tool doesn't break when another client enters the tool
                    continue;
                }
                simulatedTool.RenderOverlay(cameraInfo);
            }

            NetTool.m_helperLines = currentHelperLines;
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", currentDirectionArrows);
        }

    }
}
