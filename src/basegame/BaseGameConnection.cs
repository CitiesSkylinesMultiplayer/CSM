using System;
using System.Reflection;
using CSM.API;
using HarmonyLib;

namespace CSM.BaseGame
{
    public class BaseGameConnection : Connection
    {
        private readonly Harmony _harmony;

        public BaseGameConnection()
        {
            Name = "Cities: Skylines";
            Enabled = true;
            try
            {
                _harmony = new Harmony("com.citiesskylinesmultiplayer.basegame");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception)
            {
                Log.Info("[CSM BaseGame] Patching failed");
            }
        }

        ~BaseGameConnection()
        {
            _harmony.UnpatchAll("com.citiesskylinesmultiplayer.basegame");
        }
    }
}
