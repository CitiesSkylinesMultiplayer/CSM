using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSM.API
{
    public abstract class Connection
    {
        /// <summary>
        ///     The name of this mod support instance.
        ///     Used for displaying messages to the user.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     If this instance should be enabled, only then
        ///     the register/unregister methods are called and
        ///     the command handlers are registered.
        /// </summary>
        public bool Enabled { get; protected set; }

        /// <summary>
        ///     The class type of the supported mod.
        ///     This has to be set if Enabled is true.
        ///     It is used for detecting compatibility.
        /// </summary>
        public Type ModClass { get; protected set; }

        /// <summary>
        ///     A list of assemblies to search for command handlers.
        ///     If this instance is enabled, all found commands
        ///     are added to the protocol.
        /// </summary>
        public List<Assembly> CommandAssemblies { get; } = new List<Assembly>();

        /// <summary>
        ///     Register all handlers for changes to send to other players.
        ///     This method can for example be used to setup Harmony patches.
        ///     If will be called in the LevelLoaded handler of the LoadingExtension.
        /// </summary>
        public abstract void RegisterHandlers();

        /// <summary>
        ///     Unregister all previously registered handlers.
        ///     This method can for example be used to remove Harmony patches.
        ///     If will be called in the LevelUnloading handler of the LoadingExtension.
        /// </summary>
        public abstract void UnregisterHandlers();
    }
}
