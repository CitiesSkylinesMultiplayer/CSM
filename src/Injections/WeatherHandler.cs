using CSM.Commands;
using CSM.Helpers;
using HarmonyLib;
using CSM.Commands.Data.Weather;
using CSM.Networking;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(WeatherManager))]
    [HarmonyPatch("SimulationStepImpl")]
    public class SimulationStepImpl
    {
        public static void Prefix(WeatherManager __instance, out DataStore __state)
        {
            __state = new DataStore();

            if (IgnoreHelper.IsIgnored())
                return;

            // store weather properties in a state to prvent sending a command for each simulation tick
            __state.TargetCloud = __instance.m_targetCloud;
            __state.TargetFog = __instance.m_targetFog;
            __state.TargetNothernLights = __instance.m_targetNorthernLights;
            __state.TargetRain = __instance.m_targetRain;
            __state.TargetRainbow = __instance.m_targetRainbow;
            __state.TargetTemperature = __instance.m_targetTemperature;
        }

        public static void Postfix(WeatherManager __instance, ref DataStore __state)
        {
            if (IgnoreHelper.IsIgnored() || MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                return;

            // don't send command if target values have not been changed
            if (!__state.HasChanged(__instance))
                return;

            Command.SendToAll(new WeatherCommand
            {
                CurrentCloud = __instance.m_currentCloud,
                TargetCloud = __instance.m_targetCloud,
                CurrentFog = __instance.m_currentFog,
                TargetFog = __instance.m_targetFog,
                CurrentNothernLights = __instance.m_currentNorthernLights,
                TargetNothernLights = __instance.m_targetNorthernLights,
                CurrentRain = __instance.m_currentRain,
                TargetRain = __instance.m_targetRain,
                CurrentRainbow = __instance.m_currentRainbow,
                TargetRainbow = __instance.m_targetRainbow,
                CurrentTemperature = __instance.m_currentTemperature,
                TargetTemperature = __instance.m_targetTemperature,
            });
        }

        public class DataStore
        {
            public float TargetCloud;
            public float TargetFog;
            public float TargetNothernLights;
            public float TargetRain;
            public float TargetRainbow;
            public float TargetTemperature;

            // check if weather properties have been changed since last update
            public bool HasChanged(WeatherManager instance)
            {
                return TargetCloud != instance.m_targetCloud ||
                    TargetFog != instance.m_targetFog ||
                    TargetNothernLights != instance.m_targetNorthernLights ||
                    TargetRain != instance.m_targetRain ||
                    TargetRainbow != instance.m_targetRainbow ||
                    TargetTemperature != instance.m_targetTemperature;
            }
        }
    }
}