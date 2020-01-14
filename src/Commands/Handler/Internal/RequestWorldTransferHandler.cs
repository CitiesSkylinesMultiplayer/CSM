using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSM.Commands.Handler.Internal
{
   
    class RequestWorldTransferHandler : CommandHandler<RequestWorldTransferCommand>
    {
        protected override void Handle(RequestWorldTransferCommand command)
        {
            ///send the world to the client
            Command.SendToClient(MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[command.SenderId], new WorldTransferCommand
            {
                World = SaveHelpers.GetWorldFile()
            });
        }
    }
}
