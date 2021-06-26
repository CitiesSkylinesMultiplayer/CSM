using HarmonyLib;
using NLog;
using SampleExternalMod;
using SampleExternalMod.Commands;

namespace CSM.Injections
{

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");
        
        public static void Postfix(bool __result, ref byte district)
        {
            _logger.Info("Doing something");
            CSMConnection.Instance.SentToAll(new TestCommand());
        }
    }
    
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("Awake")]
    public class MainMenuAwake
    {
        /// <summary>
        ///     This handler is used to place the 'JOIN GAME' button to the start
        ///     of the main menu (handles reloading the intro etc.
        /// </summary>
        public static void Prefix()
        {
            MainMenuHandler.CreateOrUpdateJoinGameButton();
        }
    }

    public static class MainMenuHandler
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");

        public static void CreateOrUpdateJoinGameButton()
        {
            _logger.Info("Doing something");
            CSMConnection.Instance.SentToAll(new TestCommand());
        }
    }
}