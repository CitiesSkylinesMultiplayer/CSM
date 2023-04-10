using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    public class PlayerCursorManager: MonoBehaviour {

        private CursorInfo _cursorInfo;
        private UITextureSprite _cursorImage;
        private UILabel _playerNameLabel;
        private Vector3 _cursorWorldPosition;

        private const float ScreenEdgeInset = 12;

        public void Start() {

            UIView uiView = ToolBase.cursorInfoLabel.GetUIView();

            _playerNameLabel = (UILabel)uiView.AddUIComponent(typeof(UILabel));
            _playerNameLabel.textAlignment = UIHorizontalAlignment.Center;
            _playerNameLabel.verticalAlignment = UIVerticalAlignment.Middle;
            _playerNameLabel.backgroundSprite = "CursorInfoBack";
            _playerNameLabel.playAudioEvents = true;
            _playerNameLabel.pivot = UIPivotPoint.MiddleLeft;
            _playerNameLabel.padding = new RectOffset(23 + 16, 23, 5, 5);
            _playerNameLabel.textScale = 0.8f;
            _playerNameLabel.isVisible = false;
            _playerNameLabel.textColor = Color.white;

            //playerNameLabel.canFocus = false;
            _playerNameLabel.isInteractive = false;

            _cursorImage = (UITextureSprite)uiView.AddUIComponent(typeof(UITextureSprite));
            _cursorImage.size = new Vector2(24, 24);
            _cursorImage.isVisible = false;
            _cursorImage.isInteractive = false;
            if (_cursorInfo)
            {
                _cursorImage.texture = _cursorInfo.m_texture;
                _cursorImage.isVisible = true;
            }
        }

        public void OnDestroy()
        {
            Destroy(_playerNameLabel);
            Destroy(_cursorImage);
        }

        public void Update()
        {
            if (!(_cursorWorldPosition.sqrMagnitude > float.Epsilon)) return;

            UIView uiView = _playerNameLabel.GetUIView();
            Vector2 screenSize = ToolBase.fullscreenContainer == null ? uiView.GetScreenResolution() : ToolBase.fullscreenContainer.size;
            Camera cam = Camera.main;
            if (cam == null) return;
            Vector3 screenPosition = cam.WorldToScreenPoint(_cursorWorldPosition);
            screenPosition /= uiView.inputScale;

            // if the target is behind the camera then flip the screen position vector
            // this will keep the cursor on the border of the screen while the player looks away from the target
            if (screenPosition.z < 0) {
                screenPosition.Scale(-1 * Vector3.one);
            }

            Vector3 relativePosition = uiView.ScreenPointToGUI(screenPosition); // - ToolBase.extraInfoLabel.size * 0.5f;

            if (relativePosition.x < ScreenEdgeInset) {
                relativePosition.x = ScreenEdgeInset;
            }
            if (relativePosition.x + _playerNameLabel.size.x > screenSize.x - ScreenEdgeInset) {
                relativePosition.x = screenSize.x - _playerNameLabel.size.x - ScreenEdgeInset;
            }

            if (relativePosition.y < ScreenEdgeInset) {
                relativePosition.y = ScreenEdgeInset;
            }                
            if (relativePosition.y + _playerNameLabel.size.y > screenSize.y - ScreenEdgeInset) {
                relativePosition.y = screenSize.y - _playerNameLabel.size.y - ScreenEdgeInset;
            }

            this._playerNameLabel.relativePosition = relativePosition;
            this._cursorImage.relativePosition = relativePosition;
        }

        public void SetLabelContent(ToolCommandBase toolCommand) {
            this.SetLabelContent(toolCommand.PlayerName, toolCommand.CursorWorldPosition);
        }

        public void SetLabelContent(string playerName, Vector3 worldPosition) {
            this._cursorWorldPosition = worldPosition;
            if (_playerNameLabel) {
                this._playerNameLabel.text = playerName;
                this._playerNameLabel.isVisible = playerName != null;
            }
        }

        public void SetCursor(CursorInfo newCursorInfo) {
            if (newCursorInfo == null)
            {
                newCursorInfo = ToolsModifierControl.toolController.GetComponent<DefaultTool>().m_cursor;
            }

            if (newCursorInfo != null && (this._cursorInfo == null || this._cursorInfo.name != newCursorInfo.name)) {
                this._cursorInfo = newCursorInfo;
                if (this._cursorImage) {
                    this._cursorImage.texture = newCursorInfo.m_texture;
                    this._cursorImage.isVisible = true;
                }
            }
        }
    }
}
