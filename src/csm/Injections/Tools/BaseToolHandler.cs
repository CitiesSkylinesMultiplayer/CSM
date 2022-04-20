using CSM.API;
using CSM.API.Commands;
using CSM.Networking;
using CSM.BaseGame.Helpers;
using CSM.API.Helpers;
using ColossalFramework;
using ICities;
using CSM.BaseGame;
using CSM.Helpers;
using UnityEngine;

namespace CSM.Injections.Tools
{
    public abstract class BaseToolCommandHandler<Command, Tool> : CommandHandler<Command> where Command: CommandBase where Tool: ToolBase 
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
            
        }

        protected abstract void Configure(Tool tool, ToolController toolController, Command command);
    }
}