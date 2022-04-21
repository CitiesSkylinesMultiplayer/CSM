using ColossalFramework;
using System.Collections.Generic;
using HarmonyLib;
using CSM.BaseGame.Helpers;
using CSM.API.Helpers;

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
            // the NetTool uses static fields to share guidelines between render and simulation loops
            // unsetting the static field here will prevent any NetTools from clients also rendering the
            // guidelines again

            var currentHelperLines = NetTool.m_helperLines;
            NetTool.m_helperLines = new FastList<NetTool.HelperLine>();

            // var currentDirectionArrows = ReflectionHelper.GetAttr((NetTool)null, "m_directionArrowsBuffer");
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", new FastList<NetTool.HelperDirection2>());

            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                simulatedTool.RenderOverlay(cameraInfo);
            }

            NetTool.m_helperLines = currentHelperLines;
            // ReflectionHelper.SetAttr((NetTool)null, "m_directionArrowsBuffer", currentDirectionArrows);
        }

    }
}