﻿using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using CSM.Injections;
using System.Collections;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineCancelMoveStopHandler : CommandHandler<TransportLineCancelMoveStopCommand>
    {
        protected override void Handle(TransportLineCancelMoveStopCommand command)
        {
            TransportTool tool = ToolSimulator.GetTool<TransportTool>(command.SenderId);

            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.Prefab);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            IgnoreHelper.Instance.StartIgnore();

            int mode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");
            ReflectionHelper.SetAttr(tool, "m_mode", mode);

            IEnumerator cancelMoveStop = (IEnumerator)ReflectionHelper.Call(tool, "CancelMoveStop");
            cancelMoveStop.MoveNext();

            IgnoreHelper.Instance.EndIgnore();

            ArrayHandler.StopApplying();
        }
    }
}
