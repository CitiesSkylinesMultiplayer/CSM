using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSM.Server.Util
{
    public class SaveHelpers
    {
        public const string WorldFileName = "world.crp";

        internal static bool WorldFileExists()
        {
            return File.Exists(WorldFileName);
        }

        internal static byte[] GetWorldFile()
        {
            return File.ReadAllBytes(WorldFileName);
        }

        internal static void SaveWorkFile(byte[] world)
        {
            File.WriteAllBytes(WorldFileName, world);
        }
    }
}
