using ColossalFramework;
using CSM.API.Commands;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Injections.Tools
{
    public abstract class BaseToolCommandHandler<Cmd, Tool> : CommandHandler<Cmd> where Cmd: ToolCommandBase where Tool: ToolBase 
    {
        protected override void Handle(Cmd command)
        {
            Singleton<ToolSimulator>.instance.GetToolAndController(command.SenderId, out Tool tool,
                out ToolController controller);

            Configure(tool, controller, command);

            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                PlayerCursorManager cursorView =
                    Singleton<ToolSimulatorCursorManager>.instance.GetCursorView(command.SenderId);
                if (cursorView)
                {
                    cursorView.SetLabelContent(command);
                    cursorView.SetCursor(this.GetCursorInfo(tool));
                }
            });
        }

        protected abstract void Configure(Tool tool, ToolController toolController, Cmd command);

        protected abstract CursorInfo GetCursorInfo(Tool tool);
    }
}
