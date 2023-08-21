using System;
using System.Collections.Generic;
using CSM.API.Helpers;
using UnityEngine;
using ColossalFramework;
using CSM.BaseGame.Injections.Tools;

namespace CSM.BaseGame.Helpers
{
    public class ToolSimulator: Singleton<ToolSimulator>
    {
        private readonly Dictionary<int, ToolBase> _currentTools = new Dictionary<int, ToolBase>();

        private static readonly Color[] _playerColors = new Color[5];

        static ToolSimulator() {
            for (int i = 0; i < _playerColors.Length; i++) {
                _playerColors[i] = Color.HSVToRGB((i+1) / (float) 6, 0.8f, 1.0f);
                _playerColors[i].a = 0.5f;
            }
        }

        public IEnumerable<ToolBase> GetTools() {
            return _currentTools.Values;
        }

        public void GetToolAndController<Tool>(int sender, out Tool tool, out ToolController toolController) where Tool: ToolBase {
            tool = this.GetTool<Tool>(sender);
            toolController = ReflectionHelper.GetAttr<ToolController>(tool, "m_toolController");
        }

        public T GetTool<T>(int sender) where T : ToolBase
        {
            if (_currentTools.TryGetValue(sender, out ToolBase tool))
            {
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
            ReflectionHelper.SetAttr(controller, "ID_BrushTex", Shader.PropertyToID("_BrushTex"));
            ReflectionHelper.SetAttr(controller, "ID_BrushWS", Shader.PropertyToID("_BrushWS"));
            ReflectionHelper.SetAttr(controller, "m_collidingSegments1", new ulong[576]);
            ReflectionHelper.SetAttr(controller, "m_collidingSegments2", new ulong[576]);
            ReflectionHelper.SetAttr(controller, "m_collidingBuildings1", new ulong[768]);
            ReflectionHelper.SetAttr(controller, "m_collidingBuildings2", new ulong[768]);
            ReflectionHelper.SetAttr(controller, "m_collidingDepth", 0);
            controller.m_brushMaterial = ToolsModifierControl.toolController.m_brushMaterial;
            ReflectionHelper.SetAttr(controller, "m_brushMaterial2", new Material(controller.m_brushMaterial));

            switch (tool)
            {
                case BulldozeTool bulldozeTool:
                    bulldozeTool.m_cursor = ToolsModifierControl.toolController.GetComponent<BulldozeTool>().m_cursor;
                    bulldozeTool.m_undergroundCursor = ToolsModifierControl.toolController.GetComponent<BulldozeTool>().m_undergroundCursor;
                    break;

                case DefaultTool defaultTool:
                    defaultTool.m_cursor = ToolsModifierControl.toolController.GetComponent<DefaultTool>().m_cursor;
                    defaultTool.m_undergroundCursor = ToolsModifierControl.toolController.GetComponent<DefaultTool>().m_undergroundCursor;
                    break;
                case BuildingTool buildingTool:
                    buildingTool.m_buildCursor = ToolsModifierControl.toolController.GetComponent<BuildingTool>().m_buildCursor;
                    break;
                case NetTool netTool:
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
                    netTool.m_upgradeCursor = ToolsModifierControl.toolController.GetComponent<NetTool>().m_upgradeCursor;
                    netTool.m_placementCursor = ToolsModifierControl.toolController.GetComponent<NetTool>().m_placementCursor;
                    break;
                case ZoneTool zoneTool:
                {
                    // See ZoneTool::Awake
                    ReflectionHelper.SetAttr(zoneTool, "m_closeSegments", new ushort[16]);
                    ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer1", new ulong[64]);
                    ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer2", new ulong[64]);
                    ReflectionHelper.SetAttr(zoneTool, "m_fillBuffer3", new ulong[64]);
                    Type fillPos = typeof(ZoneTool).GetNestedType("FillPos", ReflectionHelper.AllAccessFlags);
                    ReflectionHelper.SetAttr(zoneTool, "m_fillPositions",  Activator.CreateInstance(typeof(FastList<>).MakeGenericType(fillPos)));
                    ReflectionHelper.SetAttr(zoneTool, "m_dataLock",  new object());
                    zoneTool.m_zoneCursors = ToolsModifierControl.toolController.GetComponent<ZoneTool>().m_zoneCursors;
                    break;
                }
                case TerrainTool terrainTool:
                {
                    // copy terrain tools across
                    TerrainTool realTerrainTool = ToolsModifierControl.toolController.GetComponent<TerrainTool>();
                    terrainTool.m_brush = realTerrainTool.m_brush;
                    terrainTool.m_shiftCursor = realTerrainTool.m_shiftCursor;
                    terrainTool.m_levelCursor = realTerrainTool.m_levelCursor;
                    terrainTool.m_slopeCursor = realTerrainTool.m_slopeCursor;
                    terrainTool.m_softenCursor = realTerrainTool.m_softenCursor;
                    ReflectionHelper.SetAttr(terrainTool, "m_undoList", new List<TerrainTool.UndoStroke>());
                    break;
                }
                case TransportTool transportTool:
                    // See TransportTool::Awake
                    ReflectionHelper.SetAttr(transportTool, "m_lastMoveIndex", -2);
                    ReflectionHelper.SetAttr(transportTool, "m_lastAddIndex", -2);
                    break;
                case PropTool propTool:
                    // copy prop tool cursor across
                    propTool.m_buildCursor = ToolsModifierControl.toolController.GetComponent<PropTool>().m_buildCursor;
                    propTool.m_brush = ToolsModifierControl.toolController.GetComponent<PropTool>().m_brush;
                    break;
                case TreeTool treeTool:
                    // see TreeTool::Awake()

                    // ReflectionHelper.SetAttr(treeTool, "m_randomizer", new Randomizer((int)DateTime.Now.Ticks));
                    ReflectionHelper.SetAttr(treeTool, "m_upgradedSegments", new HashSet<ushort>());
                    ReflectionHelper.SetAttr(treeTool, "m_tempUpgraded", new FastList<ushort>());

                    treeTool.m_buildCursor = ToolsModifierControl.toolController.GetComponent<TreeTool>().m_buildCursor;
                    treeTool.m_upgradeCursor = ToolsModifierControl.toolController.GetComponent<TreeTool>().m_upgradeCursor;
                    treeTool.m_brush = ToolsModifierControl.toolController.GetComponent<TreeTool>().m_brush;
                    break;

                case DistrictTool districtTool:
                    districtTool.m_brush = ToolsModifierControl.toolController.GetComponent<DistrictTool>().m_brush;
                    break;
            }

            // pick a color out of the pre-generated array to be this players color from here on
            controller.m_validColor = _playerColors[(sender + _playerColors.Length) % _playerColors.Length];

            _currentTools[sender] = tool;
            return (T)tool;
        }

        public void RemoveSender(int sender)
        {
            if (_currentTools.TryGetValue(sender, out ToolBase tool))
            {
                _currentTools.Remove(sender);

                // Tool based cleanup
                switch (tool)
                {
                    case TransportTool transportTool:
                        IgnoreHelper.Instance.StartIgnore();
                        ushort tempLine = ReflectionHelper.GetAttr<ushort>(transportTool, "m_tempLine");
                        if (tempLine != 0)
                        {
                            Singleton<TransportManager>.instance.ReleaseLine(tempLine);
                        }

                        IgnoreHelper.Instance.EndIgnore();
                        break;
                }
            }
        }

        public void Clear()
        {
            _currentTools.Clear();
            Singleton<ToolSimulatorCursorManager>.instance.Clear();
        }
    }
}
