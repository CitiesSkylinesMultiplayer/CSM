using ColossalFramework;
using System.Collections.Generic;
using HarmonyLib;
using CSM.BaseGame.Helpers;

namespace CSM.Injections.Tools
{

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndRenderingImpl")]
    public class ToolManagerGeometryRenderer {
        
        public static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                simulatedTool.RenderGeometry(cameraInfo);
            }        
        }

    }

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndOverlayImpl")]
    public class ToolManagerOverlayRenderer {
        
        public static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                simulatedTool.RenderOverlay(cameraInfo);
            }
        }

    }
}