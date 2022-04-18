using System;
using System.Collections.Generic;
using CSM.API.Helpers;
using UnityEngine;
using ColossalFramework;

namespace CSM.BaseGame.Helpers
{
    public class ToolSimulator: Singleton<ToolSimulator>
    {
        private Dictionary<int, ToolBase> _currentTools = new Dictionary<int, ToolBase>();

        private static Color[] PLAYER_COLORS = new Color[6];

        static ToolSimulator() {
            for(int i = 0; i < PLAYER_COLORS.Length; i++) {
                PLAYER_COLORS[i] = Color.HSVToRGB(i / (float) PLAYER_COLORS.Length, 0.8f, 1.0f);
                PLAYER_COLORS[i].a = 0.5f;
            }
        }

        public ICollection<ToolBase> GetTools() {
            return _currentTools.Values;
        }

        public T GetTool<T>(int sender) where T : ToolBase
        {
            ToolBase tool;
            if (_currentTools.ContainsKey(sender))
            {
                tool = _currentTools[sender];
                if (tool.GetType() == typeof(T))
                {
                    return (T)tool;
                }
            }

            tool = (ToolBase)Activator.CreateInstance(typeof(T));
            

            ToolController controller = new ToolController();
            ReflectionHelper.SetAttr(tool, "m_toolController", controller);
            // See ToolController::Awake
            ReflectionHelper.SetAttr(controller, "m_brushData", new float[4096]);
            ReflectionHelper.SetAttr(controller, "m_collidingSegments", new float[4096]);
            ReflectionHelper.SetAttr(controller, "ID_BrushTex", Shader.PropertyToID("_BrushTex"));
            ReflectionHelper.SetAttr(controller, "ID_BrushWS", Shader.PropertyToID("_BrushWS"));
            ReflectionHelper.SetAttr(controller, "m_collidingSegments1", new ulong[576]);
            ReflectionHelper.SetAttr(controller, "m_collidingSegments2", new ulong[576]);
            ReflectionHelper.SetAttr(controller, "m_collidingBuildings1", new ulong[768]);
            ReflectionHelper.SetAttr(controller, "m_collidingBuildings2", new ulong[768]);
            ReflectionHelper.SetAttr(controller, "m_collidingDepth", 0);

            if(tool is NetTool netTool) {
                // See NetTool::Awake
                ReflectionHelper.SetAttr(netTool, "m_bulldozerTool", new BulldozeTool());
                ReflectionHelper.SetAttr(netTool, "m_controlPoints", new NetTool.ControlPoint[3]);
                ReflectionHelper.SetAttr(netTool, "m_cachedControlPoints", new NetTool.ControlPoint[3]);
                ReflectionHelper.SetAttr(netTool, "m_closeSegments", new ushort[16]);
                ReflectionHelper.SetAttr(netTool, "m_cacheLock", new object());
                ReflectionHelper.SetAttr(netTool, "m_upgradedSegments", new HashSet<ushort>());
                ReflectionHelper.SetAttr(netTool, "m_tempUpgraded", new FastList<ushort>());
                ReflectionHelper.SetAttr(netTool, "m_helperLineTimer", new Dictionary<int, NetTool.HelperLineTimer>());
                ReflectionHelper.SetAttr(netTool, "m_overlayBuildings", new HashSet<ushort>());
            }
            if (tool is ZoneTool zoneTool) {  
                // See ZoneTool::Awake              
                ReflectionHelper.SetAttr(zoneTool, "m_closeSegments",  new ushort[16]);
                ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer1",  new ulong[64]);
                ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer2",  new ulong[64]);
                ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer3",  new ulong[64]);
                // ReflectionHelper.SetAttr(zoneTool, "m_fillPositions",  new FastList<ZoneTool.FillPos>());
                ReflectionHelper.SetAttr(zoneTool, "m_dataLock",  new object());
            }

            controller.m_validColor = PLAYER_COLORS[(sender + PLAYER_COLORS.Length) % PLAYER_COLORS.Length];

            _currentTools[sender] = tool;
            return (T)tool;
        }

        public void RemoveSender(int sender)
        {
            _currentTools.Remove(sender);
        }

        public void Clear()
        {
            _currentTools.Clear();
        }
    }
}
