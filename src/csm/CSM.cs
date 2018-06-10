using System;
using UnityEngine;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        public static bool IsUnity = true;

        public string Name => "CSM";

        public string Description => "Muiltiplayer mod for Cities: Skylines.";

        public static void Log(string message)
        {
            if (IsUnity)
            {
                Debug.Log($"[CSM] {message}");
            }
            else
            {
                Console.WriteLine($"[CSM] {message}");
            }
        }
    }
}