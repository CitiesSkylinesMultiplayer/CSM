using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using System;
using System.IO;

namespace CSM.Helpers
{
    /// <summary>
    ///     Helpers for loading and saving games, especially over network.
    /// </summary>
    public static class SaveHelpers
    {
        /// <summary>
        ///     Save a level to the temp folder where it can then be sent to all clients.
        /// </summary>
        /// <returns></returns>
        public static Guid SaveLevel()
        {
            // TODO: All of this
            return Guid.NewGuid();
        }

        /// <summary>
        ///     Load a downloaded level from the server.
        /// </summary>
        /// <param name="id">The ID of the world that was sent over</param>
        public static void LoadLevel(Guid id)
        {
            // Build the path where this file is saved
            var path = $"csm-client/{id}";

            // First ensure that the downloaded file exists
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            // Load the package
            var package = new Package(Path.GetFileNameWithoutExtension(path), path);

            // Ensure that the LoadingManager is ready.
            // Don't know if thats really necessary but doesn't hurt either. - root#0042
            Singleton<LoadingManager>.Ensure();

            // Get the meta data
            var asset = package.Find(package.packageMainAsset);
            var metaData = asset.Instantiate<SaveGameMetaData>();

            // Build the simulation
            var simulation = new SimulationMetaData()
            {
                m_CityName = metaData.cityName,
                m_updateMode = SimulationManager.UpdateMode.LoadGame,
                m_environment = UIView.GetAView().panelsLibrary.Get<LoadPanel>("LoadPanel").m_forceEnvironment
            };

            // Load the level
            Singleton<LoadingManager>.instance.LoadLevel(metaData.assetRef, "Game", "InGame", simulation, false);
        }
    }
}