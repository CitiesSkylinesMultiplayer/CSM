using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{

    public class ToolSimulatorCursorManager: Singleton<ToolSimulatorCursorManager> {

        private readonly Dictionary<int, PlayerCursorManager> _playerCursorViews = new Dictionary<int, PlayerCursorManager>();

        public PlayerCursorManager GetCursorView(int sender) {
            if (_playerCursorViews.TryGetValue(sender, out PlayerCursorManager view)) {
                return view;
            }
            PlayerCursorManager newView = this.gameObject.AddComponent<PlayerCursorManager>();
            _playerCursorViews[sender] = newView;
            return newView;
        }

        public void RemoveCursorView(int sender) {
            Destroy(_playerCursorViews[sender]);
            _playerCursorViews.Remove(sender);
        }

        public void Clear()
        {
            foreach (PlayerCursorManager playerCursorManager in _playerCursorViews.Values)
            {
                Destroy(playerCursorManager);
            }
            _playerCursorViews.Clear();
        }

        private MethodInfo _rayCast;

        public Vector3 DoRaycast(Ray mouseRay, float mouseRayLenght)
        {
            if (_rayCast == null)
            {
                _rayCast = typeof(ToolBase).GetMethod("RayCast", BindingFlags.Static | BindingFlags.NonPublic);                
            }
            ToolBase.RaycastInput input = new ToolBase.RaycastInput(mouseRay, mouseRayLenght);
            object[] parameters = { input, null };
            if (_rayCast != null && (bool)_rayCast.Invoke(null, parameters))
            {
                ToolBase.RaycastOutput output = (ToolBase.RaycastOutput) parameters[1];
                return output.m_hitPos;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}
