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
            var buildingTool = Singleton<ToolSimulator>.instance.GetTool<Tool>(command.SenderId);

            Configure(buildingTool, command);
            
        }

        protected abstract void Configure(Tool tool, Command command);
    }
}