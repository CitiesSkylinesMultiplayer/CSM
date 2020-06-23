﻿using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;
using System.Collections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineRemoveStopHandler : CommandHandler<TransportLineRemoveStopCommand>
    {
        protected override void Handle(TransportLineRemoveStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_building", command.Building);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "NewLine");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);
            ReflectionHelper.SetAttr(tool, "m_errors", ToolBase.ToolErrors.None);

            IEnumerator removeStop = (IEnumerator)ReflectionHelper.Call(tool, "RemoveStop");
            removeStop.MoveNext();

            IgnoreHelper.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}
