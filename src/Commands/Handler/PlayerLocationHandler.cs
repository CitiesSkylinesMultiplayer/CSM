using CSM.Panels;
using UnityEngine;

namespace CSM.Commands.Handler
{
    public class PlayerLocationHandler : CommandHandler<PlayerLocationCommand>
    {
        public PlayerLocationHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(PlayerLocationCommand command)
        {
            GameObject _playerLocation = GameObject.Find("/PlayerLocation_" + command.playerName); 
            LineRenderer lineRenderer;
            if (_playerLocation == null)
            {
                _playerLocation = new GameObject("PlayerLocation_" + command.playerName);
                lineRenderer = _playerLocation.AddComponent<LineRenderer>();
            } else {
                lineRenderer = _playerLocation.GetComponent<LineRenderer>();
            }


            // Add Tags for each of the playerMarkers 
            _playerLocation.tag = "PlayerPointerObject";

            // Setup LineRenderer 
            lineRenderer.material = new Material(Shader.Find("Custom/Particles/Alpha Blended"));
            lineRenderer.SetColors(Color.red, Color.blue);

            if (!ConnectionPanel.showPlayerPointers)
            {
                lineRenderer.SetWidth(0,0);
            }
            else
            {
                lineRenderer.SetWidth(1, 1);
            }
            

            // Set cube rotation to match the camera 
            _playerLocation.transform.position = command.playerCameraPosition;
            _playerLocation.transform.rotation = command.playerCameraRotation;

            // Make the LineRendered shoot forward (in the direction of the cube)
            lineRenderer.SetPosition(0, _playerLocation.transform.position);
            lineRenderer.SetPosition(1, _playerLocation.transform.forward * 10000 + _playerLocation.transform.position);
        }
    }
}
