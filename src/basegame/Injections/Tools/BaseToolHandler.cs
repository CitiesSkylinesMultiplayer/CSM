using ColossalFramework;
using CSM.API.Commands;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Injections.Tools
{
    public abstract class BaseToolCommandHandler<Cmd, Tool> : CommandHandler<Cmd> where Cmd: ToolCommandBase where Tool: ToolBase
    {
        protected BaseToolCommandHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(Cmd command)
        {
            // Skip tool sync commands while the game is still loading/downloading.
            // The host sends cursor position updates while the client is connecting,
            // but the client's Unity managers aren't ready to create tool instances yet.
            if (!Singleton<LoadingManager>.exists || !Singleton<LoadingManager>.instance.m_loadingComplete)
                return;

            Singleton<ToolSimulator>.instance.GetToolAndController(command.SenderId, out Tool tool,
                out ToolController controller);

            if (object.ReferenceEquals(tool, null))
                return;

            Configure(tool, controller, command);

            if (SimulationManager.instance != null && SimulationManager.instance.m_ThreadingWrapper != null)
            {
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
        }

        protected abstract void Configure(Tool tool, ToolController toolController, Cmd command);

        protected abstract CursorInfo GetCursorInfo(Tool tool);
    }
}
