using ColossalFramework;
using System.Collections.Generic;
using HarmonyLib;
using CSM.BaseGame.Helpers;


namespace CSM.Injections.Tools
{
    
    public interface IPlayerToolRenderer
    {
        void RenderGeometry(RenderManager.CameraInfo cameraInfo);
        void RenderOverlay(RenderManager.CameraInfo cameraInfo);
    }

    public class PlayerToolManager: Singleton<PlayerToolManager>, IPlayerToolRenderer {

        public void RenderGeometry(RenderManager.CameraInfo cameraInfo) {
            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                simulatedTool.RenderGeometry(cameraInfo);
            }
        }

        public void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            foreach (var simulatedTool in Singleton<ToolSimulator>.instance.GetTools()) {
                simulatedTool.RenderOverlay(cameraInfo);
            }
        }
    }

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndRenderingImpl")]
    public class ToolManagerGeometryRenderer {
        
        public static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            Singleton<PlayerToolManager>.instance.RenderGeometry(cameraInfo);
        }

    }

    [HarmonyPatch(typeof(ToolManager))]
    [HarmonyPatch("EndOverlayImpl")]
    public class ToolManagerOverlayRenderer {
        
        public static void Postfix(RenderManager.CameraInfo cameraInfo)
        {
            Singleton<PlayerToolManager>.instance.RenderOverlay(cameraInfo);
        }

    }
}