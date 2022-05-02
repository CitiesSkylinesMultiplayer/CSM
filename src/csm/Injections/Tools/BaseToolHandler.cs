using CSM.API.Commands;
using CSM.Networking;
using CSM.BaseGame.Helpers;
using ColossalFramework;

namespace CSM.Injections.Tools
{
    public abstract class BaseToolCommandHandler<Command, Tool> : CommandHandler<Command> where Command: ToolCommandBase where Tool: ToolBase 
    {

        protected override void Handle(Command command)
        {
            if (!MultiplayerManager.Instance.IsConnected())
            {
                // Ignore packets while not connected
                return;
            }

            Tool tool;
            ToolController controller;
            Singleton<ToolSimulator>.instance.GetToolAndController<Tool>(command.SenderId, out tool, out controller);

            Configure(tool, controller, command);

            var cursorView = Singleton<ToolSimulatorCursorManager>.instance.GetCursorView(command.SenderId);
            cursorView.SetLabelContent(command);
            cursorView.SetCursor(this.GetCursorInfo(tool));
            
        }

        protected abstract void Configure(Tool tool, ToolController toolController, Command command);

        protected abstract CursorInfo GetCursorInfo(Tool tool);
    }

    
}