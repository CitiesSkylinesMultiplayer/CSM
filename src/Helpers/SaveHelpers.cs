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
        public static string SYNC_SAVE_NAME = "CSM_SyncSave";

        /// <summary>
        ///     Save a level to the local save folder where it can then be sent to all clients.
        /// </summary>
        /// <returns></returns>
        public static void SaveLevel()
        {
            SavePanel sp = UIView.library.Get<SavePanel>("SavePanel");
            if (sp != null)
            {
                sp.SaveGame(SYNC_SAVE_NAME);
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
                return ReflectionHelper.Call<string>(sp, "GetSavePathName", SYNC_SAVE_NAME, false);
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

        public static void SaveWorldFile(byte[] world) 
        {
            string path = GetSavePath();
            if (path != null) 
            {
                File.WriteAllBytes(path, world);
            }
            // TODO: Print Error Message
        }

        public static void LoadLevel()
        {
            // Build the path where this file is saved
            string path = GetSavePath();

            // First ensure that the downloaded file exists
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            // Load the package
            Package package = new Package(Path.GetFileNameWithoutExtension(path), path);

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
            Singleton<LoadingManager>.instance.LoadLevel(metaData.assetRef, "Game", "InGame", simulation, false);
        }
    }
}