using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Threading;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Mods;
using CSM.Networking;
using CSM.Panels;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionResultHandler : CommandHandler<ConnectionResultCommand>
    {
        public ConnectionResultHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ConnectionResultCommand command)
        {
            // We only want this message while connecting
            if (MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connecting)
                return;

            // If we are allowed to connect
            if (command.Success)
            {
                // Log and set that we are connected.
                Log.Info("Successfully connected to server. Downloading world...");
                MultiplayerManager.Instance.CurrentClient.ClientPlayer = new Player();
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Downloading;
                MultiplayerManager.Instance.CurrentClient.ClientId = command.ClientId;
            }
            else
            {
                Log.Info($"Could not connect: {command.Reason}");
                MultiplayerManager.Instance.CurrentClient.ConnectionMessage = command.Reason;
                MultiplayerManager.Instance.CurrentClient.ConnectRejected();
                if (command.Reason.Contains("DLC")) // No other way to detect if we should display the box
                {
                    DLCHelper.DLCComparison compare = DLCHelper.Compare(command.ExpansionBitMask, DLCHelper.GetOwnedExpansions(), command.ModderPackBitMask, DLCHelper.GetOwnedModderPacks());

                    ThreadHelper.dispatcher.Dispatch(() =>
                    {
                        MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                        panel.DisplayDlcMessage(compare);
                    });
                }
                else if (command.Reason.Contains("Mods"))
                {
                    List<string> clientMods = ModSupport.Instance.RequiredModsForSync;
                    List<string> serverMods = command.Mods ?? new List<string>();

                    IEnumerable<string> clientNotServer = clientMods.Where(mod => !serverMods.Contains(mod));
                    IEnumerable<string> serverNotClient = serverMods.Where(mod => !clientMods.Contains(mod));

                    ThreadHelper.dispatcher.Dispatch(() =>
                    {
                        MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                        panel.DisplayModsMessage(serverNotClient, clientNotServer);
                    });
                }
            }
        }
    }
}
