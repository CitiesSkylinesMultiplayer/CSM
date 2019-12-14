using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;

namespace CSM.Commands.Handler.Internal
{
    class ClientJoiningHandler : CommandHandler<ClientJoiningCommand>
    {
        public ClientJoiningHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ClientJoiningCommand command)
        {
            if (command.JoiningFinished)
            {
                ThreadHelper.dispatcher.Dispatch(() =>
                {
                    ClientJoinPanel clientJoinPanel = UIView.GetAView().FindUIComponent<ClientJoinPanel>("MPClientJoinPanel");
                    if (clientJoinPanel != null)
                    {
                        clientJoinPanel.isVisible = false;
                    }
                });
                MultiplayerManager.Instance.GameBlocked = false;
            }
            else
            {
                ThreadHelper.dispatcher.Dispatch(() =>
                {
                    ClientJoinPanel clientJoinPanel = UIView.GetAView().FindUIComponent<ClientJoinPanel>("MPClientJoinPanel");
                    if (clientJoinPanel != null)
                    {
                        clientJoinPanel.isVisible = true;
                    }
                    else 
                    {
                        clientJoinPanel = (ClientJoinPanel)UIView.GetAView().AddUIComponent(typeof(ClientJoinPanel));
                    }
                    clientJoinPanel.Focus();
                });
                MultiplayerManager.Instance.GameBlocked = true;
            }
        }

        public override void OnClientDisconnect(Player player)
        {
            if (player.Status != ClientStatus.Connected)
            {
                Command.SendToClients(new ClientJoiningCommand
                {
                    JoiningFinished = true
                });
                MultiplayerManager.Instance.GameBlocked = false;
            }
        }
    }
}
