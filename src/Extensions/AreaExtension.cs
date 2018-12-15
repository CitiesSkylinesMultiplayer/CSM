using CSM.Commands;
using CSM.Models;
using ICities;
using UnityEngine;

namespace CSM.Extensions
{
    public class AreaExtension :AreasExtensionBase
    {
        public override void OnUnlockArea(int x, int z)
        {
            Command.SendToAll(new UnlockAreaCommand
            {
                X = x,
                Z = z
            });
        }
    }
}
