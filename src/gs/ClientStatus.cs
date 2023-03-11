namespace CSM.GS
{
    /// <summary>
    ///     Different client connection states
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        ///     The client is not connected to a server
        ///     and will not process any network events.
        /// </summary>
        Disconnected,

        /// <summary>
        ///     Before starting to actually connect, the global server
        ///     is queried and NAT punchthrough is attempted.
        /// </summary>
        PreConnecting,

        /// <summary>
        ///     The client is trying to connect to the server, this phase
        ///     can take up to 30 seconds.
        /// </summary>
        Connecting,

        /// <summary>
        ///     The client is connected to the server and is
        ///     transmitting information.
        /// </summary>
        Connected
    }
}
