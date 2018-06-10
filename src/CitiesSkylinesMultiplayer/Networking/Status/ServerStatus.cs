namespace CitiesSkylinesMultiplayer.Networking.Status
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
        ///     The server is current starting. This can only happen for a max of 10 seconds,
        ///     99% of the time it will take less than 500ms.
        /// </summary>
        Starting,

        /// <summary>
        ///     The server is started and is transmitting information.
        /// </summary>
        Started
    }
}
