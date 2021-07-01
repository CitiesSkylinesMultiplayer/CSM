namespace CSM.API.Networking.Status
{
    /// <summary>
    ///     Different server connection states
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        ///     The server is not running at all
        /// </summary>
        Stopped,

        /// <summary>
        ///     The server is started and is transmitting information.
        /// </summary>
        Running
    }
}
