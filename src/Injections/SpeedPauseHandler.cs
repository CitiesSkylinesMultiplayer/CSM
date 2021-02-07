using ColossalFramework;
using CSM.Helpers;
using HarmonyLib;

namespace CSM.Injections
{
    // Prevent both the SimulationPaused and SelectedSimulationSpeed setters from running and save the target value
    // for processing.
    // Note that these setters can be called in a different thread (ui thread) than the simulation is running in.
    // This means that the PauseTarget and SpeedTarget variables are written and read in different threads respectively.
    // This is not a problem because we only set the values in the ui thread, but always reset them back to null
    // in the same thread they are read (simulation thread).
    // In the worst case, the value is set after the null check in SpeedPauseHelper::SimulationStep was performed,
    // which means that it is not considered and reset back to null.
    // This does not cause a problem, but only requires the user to press the button again.

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
