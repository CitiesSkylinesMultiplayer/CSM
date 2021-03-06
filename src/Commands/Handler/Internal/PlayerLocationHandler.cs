using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using UnityEngine;

namespace CSM.Commands.Handler.Internal
{
    public class PlayerLocationHandler : CommandHandler<PlayerLocationCommand>
    {
        public PlayerLocationHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(PlayerLocationCommand command)
        {
            if (!MultiplayerManager.Instance.IsConnected())
            {
                // Ignore packets while not connected
                return;
            }

            GameObject _playerLocation = GameObject.Find("/PlayerLocation_" + command.PlayerName);
            LineRenderer lineRenderer;
            if (_playerLocation == null)
            {
                _playerLocation = new GameObject("PlayerLocation_" + command.PlayerName);
                lineRenderer = _playerLocation.AddComponent<LineRenderer>();
            }
            else
            {
                lineRenderer = _playerLocation.GetComponent<LineRenderer>();
            }

            // Add Tags for each of the playerMarkers
            _playerLocation.tag = "PlayerPointerObject";

            // Setup LineRenderer
            lineRenderer.material = new Material(Shader.Find("Custom/Particles/Alpha Blended"));
            lineRenderer.startColor = command.PlayerColor;
            lineRenderer.endColor = command.PlayerColor;

            if (!ConnectionPanel.showPlayerPointers)
            {
                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;
            }
            else
            {
                lineRenderer.startWidth = 1;
                lineRenderer.endWidth = 1;
            }

            // Set cube rotation to match the camera
            Transform playerLocation = _playerLocation.transform;
            playerLocation.position = command.PlayerCameraPosition;
            playerLocation.rotation = command.PlayerCameraRotation;

            // Make the LineRendered shoot forward (in the direction of the cube)
            Vector3 position = playerLocation.position;
            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, playerLocation.forward * 10000 + position);
        }
    }
}
