using System.Collections;
using System.Reflection;
using System.Threading;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Util;
using HarmonyLib;

namespace CSM.Injections
{
    /// <summary>
    ///     Replaces the SimulationManager.FixedUpdate method with a method with
    ///     some additional logic for dropping frames when needed.
    /// </summary>
    [HarmonyPatch]
    public class SaveGameHandler
    {
        public static MethodBase TargetMethod()
        {
            return HarmonyLib.AccessTools.FirstMethod(typeof(SavePanel), x => x.Name == "SaveGame" && x.GetParameters().Length == 2);
        }

        public static void Postfix(IEnumerator __result, string saveName, bool useCloud)
        {
            new Thread(() =>
            {
                Log.Info("Saving game, waiting until done.");
                SaveHelpers.WaitUntilSaved();

                Log.Info("Sending save game to server.");
                Command.SendToServer(new WorldTransferCommand()
                {
                    World = SaveHelpers.GetWorldFile()
                });
            }).Start();
        }
    }
}
