using System.Threading;
using CSM.Helpers;
using HarmonyLib;

namespace CSM.Injections
{
    /// <summary>
    ///     Replaces the SimulationManager.FixedUpdate method with a method with
    ///     some additional logic for dropping frames when needed.
    /// </summary>
    [HarmonyPatch(typeof(SimulationManager))]
    [HarmonyPatch("FixedUpdate")]
    public class TickLoopHandler
    {
        public static bool Prefix(object ___m_simulationFrameLock, ref int ___m_updateCounter, int ___m_maxFramesBehind, bool ___m_simulationPaused)
        {
            while (!Monitor.TryEnter(___m_simulationFrameLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
            {
            }
            try
            {
                // Check if a frame should be dropped
                if (!SlowdownHelper.CheckDropAndReduce())
                {
                    // Check if frame should be queued or dropped (original cities code)
                    if (___m_updateCounter < ___m_maxFramesBehind)
                    {
                        ___m_updateCounter++;
                        Monitor.Pulse(___m_simulationFrameLock);
                    }
                    else if (!___m_simulationPaused)
                    {
                        // Increase dropped frame count (only when running, we don't care about dropped frames during pause)
                        SlowdownHelper.FrameDropped();
                    }
                }
            }
            finally
            {
                Monitor.Exit(___m_simulationFrameLock);
            }

            // Don't run original method
            return false;
        }
    }
}
