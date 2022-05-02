using ColossalFramework;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;

namespace CSM.Injections.Tools
{
    public class PlayerCursorManager: MonoBehaviour {

        private CursorInfo cursorInfo;
        private UITextureSprite cursorImage;
        private UILabel playerNameLabel;

        public Vector3 cursorWorldPosition;

        private static float screenEdgeInset = 12;

        private static Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        public void Start() {

            var uiview = ToolBase.cursorInfoLabel.GetUIView();

            playerNameLabel = (UILabel)uiview.AddUIComponent(typeof(UILabel));
            playerNameLabel.textAlignment = UIHorizontalAlignment.Center;
            playerNameLabel.verticalAlignment = UIVerticalAlignment.Middle;
            playerNameLabel.backgroundSprite = "CursorInfoBack";
            playerNameLabel.playAudioEvents = true;
            playerNameLabel.pivot = UIPivotPoint.MiddleLeft;
            playerNameLabel.padding = new RectOffset(23 + 16, 23, 5, 5);
            playerNameLabel.textScale = 0.8f;
            playerNameLabel.eventClicked += this.OnClick;
            playerNameLabel.isVisible = false;

            cursorImage = (UITextureSprite)uiview.AddUIComponent(typeof(UITextureSprite));
            cursorImage.size = new Vector2(24, 24);
            cursorImage.isVisible = false;
        }

        public void Update() {

            var cursorWorldPosition = this.cursorWorldPosition;

            if(cursorWorldPosition != null) {
                UIView uiview = playerNameLabel.GetUIView();
			    Vector2 screenSize = (!(ToolBase.fullscreenContainer != null)) ? uiview.GetScreenResolution() : ToolBase.fullscreenContainer.size;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(cursorWorldPosition);
                screenPosition /= uiview.inputScale;
                Vector3 relativePosition = uiview.ScreenPointToGUI(screenPosition); // - ToolBase.extraInfoLabel.size * 0.5f;

                this.playerNameLabel.textColor = Color.white;

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
            if(playerNameLabel) {
                this.playerNameLabel.text = playerName;
                this.playerNameLabel.isVisible = playerName != null && worldPosition != null;
            }
        }

        public void SetCursor(CursorInfo cursorInfo) {
            if(cursorInfo != null && this.cursorInfo == null || this.cursorInfo.name != cursorInfo.name) {
                this.cursorInfo = cursorInfo;
                if(this.cursorImage) {
                    this.cursorImage.texture = cursorInfo.m_texture;
                    this.cursorImage.isVisible = true;
                }
            }
        }
    }
}