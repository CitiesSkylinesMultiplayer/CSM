using CSM.Helpers;
using HarmonyLib;

namespace CSM.Injections
{
    // Prevent both the SimulationPaused and SelectedSimulationSpeed setters from running and save the target value
    // for processing.

    [HarmonyPatch(typeof(SimulationManager))]
    [HarmonyPatch("SimulationPaused", MethodType.Setter)]
    public static class SetSimulationPaused
    {
        public static bool Prefix(bool value)
        {
            SpeedPauseHelper.PauseTarget = value;

            // Don't run the original method
            return false;
        }
    }
    
    [HarmonyPatch(typeof(SimulationManager))]
    [HarmonyPatch("SelectedSimulationSpeed", MethodType.Setter)]
    public class SetSelectedSimulationSpeed
    {
        public static bool Prefix(int value)
        {
            SpeedPauseHelper.SpeedTarget = value;
            if (SimulationManager.instance.SimulationPaused)
            {
                SpeedPauseHelper.PauseTarget = false;
            }

            // Don't run the original method
            return false;
        }
    }
}
