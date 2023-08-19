using System.Collections;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineCreateHandler : CommandHandler<TransportLineCreateCommand>
    {
        protected override void Handle(TransportLineCreateCommand command)
        {
            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_building", command.Building);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "NewLine");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator newLine = (IEnumerator)ReflectionHelper.Call(tool, "NewLine");
            newLine.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}
