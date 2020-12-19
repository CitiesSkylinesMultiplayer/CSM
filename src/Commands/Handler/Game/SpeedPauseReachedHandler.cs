using CSM.Commands.Data.Game;
using CSM.Helpers;

namespace CSM.Commands.Handler.Game
{
    public class SpeedPauseReachedHandler : CommandHandler<SpeedPauseReachedCommand>
    {
        private static int _currentWaitingId;
        private static int _maxNumberOfClients;
        private static int _numberOfClients;

        public SpeedPauseReachedHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(SpeedPauseReachedCommand command)
        {
            if (command.RequestId == _currentWaitingId)
            {
                _numberOfClients++;
                if (_numberOfClients == _maxNumberOfClients)
                {
                    SpeedPauseHelper.StateReached();
                    _currentWaitingId = -1;

                    // Allow the response handler to process new requests
                    // We need to wait until now because responses of conflicting requests
                    // could arrive during the waiting phase, but need to be discarded.
                    SpeedPauseResponseHandler.ResetWaitingId();
                }
            }
        }

        public static void SetWaitingFor(int currentWaitingId, int maxNumberOfClients)
        {
            _currentWaitingId = currentWaitingId;
            _maxNumberOfClients = maxNumberOfClients;
            _numberOfClients = 0;
        }

        public static int GetCurrentId()
        {
            return _currentWaitingId;
        }
    }
}
