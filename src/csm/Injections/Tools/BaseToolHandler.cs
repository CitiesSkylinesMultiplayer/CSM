using CSM.API;
using CSM.API.Commands;
using CSM.Networking;
using CSM.BaseGame.Helpers;
using CSM.API.Helpers;
using ColossalFramework;
using ICities;
using CSM.BaseGame;
using CSM.Helpers;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;

namespace CSM.Injections.Tools
{
    public abstract class BaseToolCommandHandler<Command, Tool> : CommandHandler<Command> where Command: ToolCommandBase where Tool: ToolBase 
    {

        protected override void Handle(Command command)
        {
            if (!MultiplayerManager.Instance.IsConnected())
            {
                // Ignore packets while not connected
                return;
            }

            Tool tool;
            ToolController controller;
            Singleton<ToolSimulator>.instance.GetToolAndController<Tool>(command.SenderId, out tool, out controller);

            Configure(tool, controller, command);

            var cursorView = Singleton<ToolSimulatorCursorManager>.instance.GetCursorView(command.SenderId);
            cursorView.SetLabelContent(command);
            cursorView.SetCursor(this.GetCursorInfo(tool));
            
        }

        protected abstract void Configure(Tool tool, ToolController toolController, Command command);

        protected abstract CursorInfo GetCursorInfo(Tool tool);
    }

    public class ToolSimulatorCursorManager: Singleton<ToolSimulatorCursorManager> {

        public static bool test = false;
        public static bool ShouldTest() {
            var result = test;
            test = false;
            return result;
        }

        private Dictionary<int, PlayerCursorManager> playerCursorViews = new Dictionary<int, PlayerCursorManager>();

        public PlayerCursorManager GetCursorView(int sender) {
            if(playerCursorViews.TryGetValue(sender, out PlayerCursorManager view)) {
                return view;
            }
            var newGameObject = new GameObject("PlayerCursorManager_"+sender);
            PlayerCursorManager newView = newGameObject.AddComponent<PlayerCursorManager>();
            playerCursorViews[sender] = newView;
            return newView;
        }

    }

    public class PlayerCursorManager: MonoBehaviour {

        private CursorInfo cursorInfo;
        public UITextureSprite cursorImage;
        public UILabel playerNameLabel;

        public Vector3 cursorWorldPosition;

        private static float screenEdgeInset = 8;

        private static Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        public void Start() {

            var uiview = ToolBase.cursorInfoLabel.GetUIView();

            playerNameLabel = (UILabel)uiview.AddUIComponent(typeof(UILabel));
            playerNameLabel.textAlignment = UIHorizontalAlignment.Center;
            playerNameLabel.verticalAlignment = UIVerticalAlignment.Middle;
            playerNameLabel.backgroundSprite = "CursorInfoBack";
            playerNameLabel.pivot = UIPivotPoint.MiddleLeft;
            playerNameLabel.padding = new RectOffset(23 + 16, 23, 5, 5);
            playerNameLabel.textScale = 0.8f;
            playerNameLabel.eventClicked += this.OnClick;

            cursorImage = (UITextureSprite)uiview.AddUIComponent(typeof(UITextureSprite));
            cursorImage.size = new Vector2(24, 24);            
        }

        public void Update() {

            var cursorWorldPosition = this.cursorWorldPosition;

            if(cursorWorldPosition != null) {
                UIView uiview = playerNameLabel.GetUIView();
			    Vector2 screenSize = (!(ToolBase.fullscreenContainer != null)) ? uiview.GetScreenResolution() : ToolBase.fullscreenContainer.size;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(cursorWorldPosition);
                screenPosition /= uiview.inputScale;
                Vector3 relativePosition = uiview.ScreenPointToGUI(screenPosition) - ToolBase.extraInfoLabel.size * 0.5f;

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
            cameraController.SetOverrideModeOn(this.cursorWorldPosition, cameraController.m_currentAngle, cameraController.m_currentSize);      
            cameraController.SetOverrideModeOff();
        }

        public void SetLabelContent(ToolCommandBase toolCommand) {
            this.SetLabelContent(toolCommand.PlayerName, toolCommand.CursorWorldPosition);
        }

        public void SetLabelContent(string playerName, Vector3 worldPosition) {
            this.playerNameLabel.text = playerName;
            this.cursorWorldPosition = worldPosition;
            this.playerNameLabel.isVisible = playerName != null && playerName != null;
        }

        public void SetCursor(CursorInfo cursorInfo) {
            if(this.cursorInfo == null || this.cursorInfo.name != cursorInfo.name) {
                this.cursorInfo = cursorInfo;
                this.cursorImage.texture = cursorInfo.m_texture;
            }
        }
    }
}