using CSM.Commands.Data.Terrain;
using CSM.Helpers;

namespace CSM.Commands.Handler.Terrain
{
    public class TerrainModificationHandler : CommandHandler<TerrainModificationCommand>
    {
        protected override void Handle(TerrainModificationCommand command)
        {
            TerrainTool tool = ToolSimulator.GetTool<TerrainTool>(command.SenderId);

            // Apply data from command
            command.BrushData.CopyTo(ReflectionHelper.GetAttr<ToolController>(tool, "m_toolController").BrushData, 0);
            tool.m_brushSize = command.BrushSize;
            tool.m_strength = command.Strength;
            ReflectionHelper.SetAttr(tool, "m_mousePosition", command.MousePosition);
            ReflectionHelper.SetAttr(tool, "m_startPosition", command.StartPosition);
            ReflectionHelper.SetAttr(tool, "m_endPosition", command.EndPosition);
            ReflectionHelper.SetAttr(tool, "m_currentCost", 0);
            tool.m_mode = command.Mode;
            ReflectionHelper.SetAttr(tool, "m_mouseRightDown", command.MouseRightDown);

            IgnoreHelper.StartIgnore();
            // Call original method
            ReflectionHelper.Call(tool, "ApplyBrush");

            IgnoreHelper.EndIgnore();
        }
    }
}
