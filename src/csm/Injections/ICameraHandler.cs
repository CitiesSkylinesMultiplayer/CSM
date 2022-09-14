using CSM.API.Commands;
using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Panels;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(CameraController))]
    [HarmonyPatch("LateUpdate")]
    public class ICameraUpdateCurrentPosition
    {
        private static Vector3 playerCameraPosition_last;
        private static Quaternion playerCameraRotation_last;

        public static void Postfix(CameraController __instance, Camera ___m_camera)
        {
            // Make sure the camera object is defined, this should always happen but for some strange reason it didnt (look bellow)
            // This is not happening if i test it localy. (debug using AllocConsole in CSM.cs)
            // https://github.com/DominicMaas/Tango/issues/155
            if (___m_camera && MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None)
            {
                // Get camera rotation, angle and position
                Transform transform = ___m_camera.transform;
                Quaternion _rotation = transform.rotation;
                Vector3 _position = transform.position;

                // Update the player camera position every time the camera moves OR rotates
                // This should not be that demanding on the server (might be demanding if you are playing with a lot of players
                // We could also make the server update the player locations every x seconds by storing all the info on the server and send it to all clients every 1 second?
                if (Vector3.Distance(_position, playerCameraPosition_last) > 1 || playerCameraRotation_last != _rotation)
                {
                    // Store camera rotation and position
                    playerCameraPosition_last = _position;
                    playerCameraRotation_last = _rotation;


                    // Set the correct playerName if our currentRole is SERVER, else use the CurrentClient Username
                    string playerName;
                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                    {
                        playerName = MultiplayerManager.Instance.CurrentServer.Config.Username;
                    }
                    else
                    {
                        playerName = MultiplayerManager.Instance.CurrentClient.Config.Username;
                    }

                    // Send info to all clients
                    /*Command.SendToAll(new PlayerLocationCommand
                    {
                        PlayerName = playerName,
                        PlayerCameraPosition = _position,
                        PlayerCameraRotation = _rotation,
                        PlayerCameraHeight = __instance.m_currentHeight,
                        PlayerColor = JoinGamePanel.playerColor
                    });*/
                }
            }
        }
    }
}
