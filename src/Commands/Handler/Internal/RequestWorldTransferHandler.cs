using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSM.Commands.Handler.Internal
{
   
    class RequestWorldTransferHandler : CommandHandler<RequestWorldTransferCommand>
    {
        protected override void Handle(RequestWorldTransferCommand command)
        {
            //pause game
            MultiplayerManager.Instance.GameBlocked = true;
            SimulationManager.instance.SimulationPaused = true;
            //saving world
            SaveHelpers.SaveServerLevel();

            new Thread(() =>
            {
                while (SaveHelpers.IsSaving())
                {
                    Thread.Sleep(100);
                }
                //send the world to the client
                Command.SendToClient(MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[command.SenderId], new WorldTransferCommand
                {
                    World = SaveHelpers.GetWorldFile()
                });

                newPlayer.Status = ClientStatus.Loading;
            }).Start();
            
        }
    }
}
