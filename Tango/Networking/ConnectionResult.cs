namespace Tango.Networking
{
    public class ConnectionResult
    {
        public ConnectionResult(bool isConnected, string errorMessage = "")
        {
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }

        public bool IsConnected { get; }
        public string ErrorMessage { get; }
    }
}
