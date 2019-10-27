using UnityEngine;
using CSM.Networking;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Panels;
using HarmonyLib;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("UpdateTransform")]
    public class ICameraUpdateCurrentPosition
    {
        private static Vector3 playerCameraPosition_last;
        private static Quaternion playerCameraRotation_last;

        public static void Postfix(CameraController __instance, Camera ___m_camera)
        {
            // Get camera rotation, angle and position
            Transform transform = ___m_camera.transform;
            Quaternion _rotation = transform.rotation;
            Vector3 _position = transform.position;

            // Check if the camera moved
            if (Vector3.Distance(_position, playerCameraPosition_last) > 1 || playerCameraRotation_last != _rotation)
            {
                // Store camera rotation and position
                playerCameraPosition_last = _position;
                playerCameraRotation_last = _rotation;

                // Send info to all clients
                Command.SendToAll(new PlayerLocationCommand
                {
                    PlayerName = MultiplayerManager.Instance.CurrentClient.Config.Username,
                    PlayerCameraPosition = _position,
                    PlayerCameraRotation = _rotation,
                    PlayerCameraHeight = __instance.m_currentHeight,
                    PlayerColor = JoinGamePanel.playerColor
                });
            }
        }
    }
}
