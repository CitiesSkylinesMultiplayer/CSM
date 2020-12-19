using CSM.Commands.Data.Game;
using CSM.Helpers;

namespace CSM.Commands.Handler.Game
{
    public class SpeedPauseResponseHandler : CommandHandler<SpeedPauseResponseCommand>
    {
        private static int _currentWaitingId;
        private int _maxNumberOfClients;
        private int _numberOfClients;
        private long _highestLatency;
        private long _highestTime;

        public SpeedPauseResponseHandler()
        {
            TransactionCmd = false;
            ResetValues();
        }
        
        protected override void Handle(SpeedPauseResponseCommand command)
        {
            int requestId = command.RequestId;

            // Use highest request id in case of conflicting requests
            if (requestId < _currentWaitingId)
            {
                return;
            }

            // If the new id is new, reset all other values
            if (requestId != _currentWaitingId)
            {
                ResetValues();
            }

            _currentWaitingId = requestId;

            if (command.NumberOfClients != -1)
            {
                _maxNumberOfClients = command.NumberOfClients;
            }

            if (command.MaxLatency != -1 && command.MaxLatency > _highestLatency)
            {
                _highestLatency = command.MaxLatency;
            }

            if (command.CurrentTime > _highestTime)
            {
                _highestTime = command.CurrentTime;
            }

            _numberOfClients++;

            // Check if all clients have answered
            if (_numberOfClients == _maxNumberOfClients)
            {
                SpeedPauseHelper.SpeedPauseResponseReceived(_highestTime, _highestLatency);
                
                // Set waiting target for the SpeedPauseReachedCommand
                SpeedPauseReachedHandler.SetWaitingFor(_currentWaitingId, _maxNumberOfClients);
            }
        }

        private void ResetValues()
        { 
            _currentWaitingId = -1;
            _maxNumberOfClients = -1;
            _numberOfClients = 0;
            _highestLatency = -1;
            _highestTime = -1;
        }

        public static void ResetWaitingId()
        {
            _currentWaitingId = -1;
        }
    }
}
