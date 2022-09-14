using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using CSM.API;
using CSM.API.Networking;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    public class PlayerCursorManager: MonoBehaviour {

        private CursorInfo cursorInfo;
        private UITextureSprite cursorImage;
        private UILabel playerNameLabel;

        public Vector3 cursorWorldPosition;

        private static float screenEdgeInset = 12;

        public void Start() {

            UIView uiView = ToolBase.cursorInfoLabel.GetUIView();

            playerNameLabel = (UILabel)uiView.AddUIComponent(typeof(UILabel));
            playerNameLabel.textAlignment = UIHorizontalAlignment.Center;
            playerNameLabel.verticalAlignment = UIVerticalAlignment.Middle;
            playerNameLabel.backgroundSprite = "CursorInfoBack";
            playerNameLabel.playAudioEvents = true;
            playerNameLabel.pivot = UIPivotPoint.MiddleLeft;
            playerNameLabel.padding = new RectOffset(23 + 16, 23, 5, 5);
            playerNameLabel.textScale = 0.8f;
            playerNameLabel.eventClicked += this.OnClick;
            playerNameLabel.isVisible = false;
            playerNameLabel.textColor = Color.white;

            //playerNameLabel.canFocus = false;
            playerNameLabel.isInteractive = false;

            cursorImage = (UITextureSprite)uiView.AddUIComponent(typeof(UITextureSprite));
            cursorImage.size = new Vector2(24, 24);
            cursorImage.isVisible = false;
            cursorImage.isInteractive = false;
            if (cursorInfo)
            {
                cursorImage.texture = cursorInfo.m_texture;
                cursorImage.isVisible = true;
            }
        }

        public void Update()
        {
            if (!(cursorWorldPosition.sqrMagnitude > float.Epsilon)) return;

            UIView uiView = playerNameLabel.GetUIView();
            Vector2 screenSize = ToolBase.fullscreenContainer == null ? uiView.GetScreenResolution() : ToolBase.fullscreenContainer.size;
            Camera cam = Camera.main;
            if (cam == null) return;
            Vector3 screenPosition = cam.WorldToScreenPoint(cursorWorldPosition);
            screenPosition /= uiView.inputScale;

            // if the target is behind the camera then flip the screen position vector
            // this will keep the cursor on the border of the screen while the player looks away from the target
            if (screenPosition.z < 0) {
                screenPosition.Scale(-1 * Vector3.one);
            }

            Vector3 relativePosition = uiView.ScreenPointToGUI(screenPosition); // - ToolBase.extraInfoLabel.size * 0.5f;

            if (relativePosition.x < screenEdgeInset) {
                relativePosition.x = screenEdgeInset;
            }
            if (relativePosition.x + playerNameLabel.size.x > screenSize.x - screenEdgeInset) {
                relativePosition.x = screenSize.x - playerNameLabel.size.x - screenEdgeInset;
            }

            if (relativePosition.y < screenEdgeInset) {
                relativePosition.y = screenEdgeInset;
            }                
            if (relativePosition.y + playerNameLabel.size.y > screenSize.y - screenEdgeInset) {
                relativePosition.y = screenSize.y - playerNameLabel.size.y - screenEdgeInset;
            }

            this.playerNameLabel.relativePosition = relativePosition;
            this.cursorImage.relativePosition = relativePosition;
        }

        public void OnClick(UIComponent component, UIMouseEventParameter eventParam) {
            var cameraController = Singleton<CameraController>.instance;
            cameraController.m_targetPosition = this.cursorWorldPosition;
        }

        public void SetLabelContent(ToolCommandBase toolCommand) {
            this.SetLabelContent(toolCommand.PlayerName, toolCommand.CursorWorldPosition);
        }

        public void SetLabelContent(string playerName, Vector3 worldPosition) {
            this.cursorWorldPosition = worldPosition;
            if (playerNameLabel) {
                this.playerNameLabel.text = playerName;
                this.playerNameLabel.isVisible = playerName != null;
            }
        }

        public void SetCursor(CursorInfo newCursorInfo) {
            if (newCursorInfo != null && this.cursorInfo == null || this.cursorInfo.name != newCursorInfo.name) {
                this.cursorInfo = newCursorInfo;
                if (this.cursorImage) {
                    this.cursorImage.texture = newCursorInfo.m_texture;
                    this.cursorImage.isVisible = true;
                }
            }
        }
    }
}
