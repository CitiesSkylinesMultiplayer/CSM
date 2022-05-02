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
            PlayerCursorManager newView = this.gameObject.AddComponent<PlayerCursorManager>();
            playerCursorViews[sender] = newView;
            return newView;
        }

        public void RemoveCursorView(int sender) {
            playerCursorViews.Remove(sender);
        }

    }
}