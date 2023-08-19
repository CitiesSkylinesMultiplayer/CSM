using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using CSM.BaseGame.Helpers;
using ColossalFramework;
using CSM.API;

namespace CSM.BaseGame.Commands.Handler.TransportLines
{
    public class TransportLineSyncHandler : CommandHandler<TransportLineSyncCommand>
    {
        protected override void Handle(TransportLineSyncCommand command)
        {
            Log.Info(command.ToString());
            Log.Info(command.HitPosition.ToString());
            Log.Info(command.FixedPlatform.ToString());
            Log.Info(command.HoverStopIndex.ToString());
            Log.Info(command.HoverSegmentIndex.ToString());
            Log.Info(command.Mode.ToString());
            Log.Info(command.Errors.ToString());
            Log.Info(command.UpdateLines.ToString());
            Log.Info(command.UpdatePaths.ToString());

            TransportTool tool = Singleton<ToolSimulator>.instance.GetTool<TransportTool>(command.SenderId);

            IgnoreHelper.Instance.StartIgnore();

            if (command.UpdateLines)
            {
                TransportManager.instance.UpdateLinesNow();
            }

            if (command.UpdatePaths)
            {
                ushort tempLine = ReflectionHelper.GetAttr<ushort>(tool, "m_tempLine");
                TransportManager.instance.m_lines.m_buffer[tempLine].UpdatePaths(tempLine);
            }

            IgnoreHelper.Instance.EndIgnore();

            ReflectionHelper.SetAttr(tool, "m_hitPosition", command.HitPosition);
            ReflectionHelper.SetAttr(tool, "m_fixedPlatform", command.FixedPlatform);
            ReflectionHelper.SetAttr(tool, "m_hoverStopIndex", command.HoverStopIndex);
            ReflectionHelper.SetAttr(tool, "m_hoverSegmentIndex", command.HoverSegmentIndex);
            ReflectionHelper.SetAttr(tool, "m_mode", command.Mode);
            ReflectionHelper.SetAttr(tool, "m_errors", command.Errors);
        }
    }
}
