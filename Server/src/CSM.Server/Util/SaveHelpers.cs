using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSM.Server.Util
{
    public class SaveHelpers
    {
        internal static byte[] GetWorldFile()
        {
            return File.ReadAllBytes("world.crp");
        }

        internal static void SaveWorkFile(byte[] world)
        {
            File.WriteAllBytes("world.crp", world);
        }
    }
}
