using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;
using CSM.API;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineCreateHandler : CommandHandler<TransportLineCreateCommand>
    {
        protected override void Handle(TransportLineCreateCommand command)
        {
            Log.Info(command.ToString());
            Log.Info(command.Array16Ids?.ToString());
            Log.Info(command.Prefab.ToString());
            Log.Info(command.Building.ToString());
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);
            Log.Info(ReflectionHelper.GetAttr(tool, "m_lastEditLine").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_hoverStopIndex").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_hoverSegmentIndex").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_lastAddIndex").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_hitPosition").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_fixedPlatform").ToString());

            ArrayHandler.StartApplying(command.Array16Ids, null);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_building", command.Building);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "NewLine");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator newLine = (IEnumerator)ReflectionHelper.Call(tool, "NewLine");
            newLine.MoveNext();
            
            Log.Info(ReflectionHelper.GetAttr(tool, "m_errors").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_line").ToString());
            Log.Info(ReflectionHelper.GetAttr(tool, "m_mode").ToString());

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}
