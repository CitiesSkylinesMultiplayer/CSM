using ColossalFramework;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using CSM.Util;
using System.IO;
using ColossalFramework.Threading;
using CSM.API;
using CSM.API.Helpers;

namespace CSM.Helpers
{
    /// <summary>
    ///     Helpers for loading and saving games, especially over network.
    /// </summary>
    public static class SaveHelpers
    {
        private const string SYNC_NAME = "CSM_SyncSave";

        /// <summary>
        ///     Save a level to the local save folder where it can then be sent to all clients.
        /// </summary>
        public static void SaveServerLevel()
        {
            SavePanel sp = UIView.library.Get<SavePanel>("SavePanel");
            if (sp != null)
            {
                sp.SaveGame(SYNC_NAME);
            }
        }

        public static bool IsSaving()
        {
            return SavePanel.isSaving;
        }

        public static string GetSavePath()
        {
            SavePanel sp = UIView.library.Get<SavePanel>("SavePanel");
            if (sp != null)
            {
                return ReflectionHelper.Call<string>(sp, "GetSavePathName", SYNC_NAME, false);
            }
            return null;
        }

        public static byte[] GetWorldFile()
        {
            string path = GetSavePath();
            if (path != null)
            {
                return File.ReadAllBytes(path);
            }
            return null;
        }

        /// <summary>
        ///     Load the world byte array sent by the server
        /// </summary>
        public static void LoadLevel(byte[] world)
        {
            Log.Info($"Preparing to load world (of size {world.Length})...");

            // Load the package
            Package package = new Package(SYNC_NAME, world);

            // Ensure that the LoadingManager is ready.
            // Don't know if thats really necessary but doesn't hurt either. - root#0042
            Singleton<LoadingManager>.Ensure();

            // Get the meta data
            Package.Asset asset = package.Find(package.packageMainAsset);
            SaveGameMetaData metaData = asset.Instantiate<SaveGameMetaData>();

            // Build the simulation
            SimulationMetaData simulation = new SimulationMetaData()
            {
                m_CityName = metaData.cityName,
                m_updateMode = SimulationManager.UpdateMode.LoadGame,
                m_environment = UIView.GetAView().panelsLibrary.Get<LoadPanel>("LoadPanel").m_forceEnvironment
            };

            // Load the level
            Log.Info("Telling the loading manager to load the level");
            ThreadHelper.dispatcher.Dispatch(() =>
                Singleton<LoadingManager>.instance.LoadLevel(metaData.assetRef, "Game", "InGame", simulation)
            );
        }
    }
}
