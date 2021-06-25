using NLog;

namespace CSM.API.Commands
{
    class ConnectionTestHandler : CommandHandler<ConnectionTestCommand>
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");
        protected override void Handle(ConnectionTestCommand command)
        {
            if(command.connectionTestInt == 1)
            _logger.Info("Mod Connection success");
        }
    }
}
