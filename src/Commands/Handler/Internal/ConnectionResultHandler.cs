using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionResultHandler : CommandHandler<ConnectionResultCommand>
    {
        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
                _logger.Info("Successfully connected to server.");
                ChatLogPanel.PrintGameMessage("Successfully connected to server.");
                if (command.WaitForWorld)
                {
                    MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Downloading;
                }
                else
                {
                    MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                }
                MultiplayerManager.Instance.CurrentClient.ClientId = command.ClientId;
            }
            else
            {
                _logger.Info($"Could not connect: {command.Reason}");
                MultiplayerManager.Instance.CurrentClient.ConnectionMessage = command.Reason;
                MultiplayerManager.Instance.CurrentClient.Disconnect();
                if (command.DLCBitMask != SteamHelper.DLC_BitMask.None)
                {
                    DLCHelper.DLCComparison compare = DLCHelper.Compare(command.DLCBitMask, DLCHelper.GetOwnedDLCs());
                    if (compare.ClientMissing != SteamHelper.DLC_BitMask.None)
                    {
                        ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, $"You are missing the following DLCs: {compare.ClientMissing}");
                    }
                    if (compare.ServerMissing != SteamHelper.DLC_BitMask.None)
                    {
                        ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, $"The server doesn't have the following DLCs: {compare.ServerMissing}");
                    }
                    ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Normal, "DLCs can be disabled via checkbox in Steam");
                }
            }
        }
    }
}
