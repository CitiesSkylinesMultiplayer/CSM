using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using System;
using System.IO;

namespace CSM.Common
{
    /// <summary>
    ///     Class with helpers for loading and saving worlds.
    /// </summary>
    public static class WorldManager
    {
        public static byte[] GetWorld()
        {
            // The actual world?

            //var package = new Package("multiplayer-game");
            //package.packageMainAsset = "multiplayer-game";

            //var saveGameMetaData = new SaveGameMetaData
            //{
            //    mods = EmbedModInfo(),
            //    assets = EmbedAssetInfo(),
            //    timeStamp = DateTime.Now
            //};

            //if (Singleton<SimulationManager>.instance.m_metaData.m_MapThemeMetaData != null)
            //{
            //    saveGameMetaData.mapThemeRef = Singleton<SimulationManager>.instance.m_metaData.m_MapThemeMetaData.mapThemeRef;
            //}
            //saveGameMetaData.cityName = Singleton<SimulationManager>.instance.m_metaData.m_CityName;
            //saveGameMetaData.environment = Singleton<SimulationManager>.instance.m_metaData.m_environment;
            //UIPopulationWrapper uIPopulationWrapper = new UIPopulationWrapper(uint.MaxValue);
            //uIPopulationWrapper.Check(Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_populationData.m_finalCount);
            //saveGameMetaData.population = uIPopulationWrapper.result;
            //UICurrencyWrapper uICurrencyWrapper = new UICurrencyWrapper(0L);
            //uICurrencyWrapper.Check(Singleton<EconomyManager>.instance.LastCashAmount);
            //saveGameMetaData.cash = uICurrencyWrapper.result;
            //saveGameMetaData.achievementsDisabled = (Singleton<SimulationManager>.instance.m_metaData.m_disableAchievements == SimulationMetaData.MetaBool.True);
            //saveGameMetaData.assetRef = package.AddFileAsset(saveName + "_Data", filename, Package.AssetType.Data);
            //saveGameMetaData.steamPreviewRef = package.AddAsset(saveName + "_SteamPreview", m_SnapshotSteamImage, uniqueName: false, Image.BufferFileFormat.PNG, compress: false, forceLinear: false);
            //saveGameMetaData.imageRef = package.AddAsset(saveName + "_Snapshot", m_SnapshotImage, uniqueName: false, Image.BufferFileFormat.PNG, compress: false, forceLinear: false);
            //package.AddAsset(saveName, saveGameMetaData, UserAssetType.SaveGameMetaData);

            //// The simulation data
            //using (var ms = new MemoryStream())
            //{
            //    DataSerializer.Serialize(ms, DataSerializer.Mode.Memory, BuildConfig.SAVE_DATA_FORMAT_VERSION, Singleton<SimulationManager>.instance.m_metaData);
            //    DataSerializer.Serialize(ms, DataSerializer.Mode.Memory, BuildConfig.SAVE_DATA_FORMAT_VERSION, new SimulationManager.Data());
            //}

            return null;
        }

        public static void LoadWorld(byte[] worldBuffer, byte[] simulationBuffer)
        {
            using (var ms = new MemoryStream(simulationBuffer))
            {
                var simulationData = DataSerializer.Deserialize<SimulationMetaData>(ms, DataSerializer.Mode.Memory);

                Package.Asset game = new Package.Asset("multiplayer-game", worldBuffer, Package.AssetType.Unknown, false);

                // Load the level
                Singleton<LoadingManager>.instance.LoadLevel(game, "Game", "InGame", simulationData, false);
            }
        }
    }
}