using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Trees;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("CreateTree")]
    public class CreateTree
    {
        public static void Postfix(bool __result, ref uint tree, Vector3 position, bool single)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (__result)
            {
                TreeInstance treeInstance = Singleton<TreeManager>.instance.m_trees.m_buffer[tree];

                Command.SendToAll(new TreeCreateCommand
                {
                    Position = position,
                    TreeId = tree,
                    Single = single,
                    InfoIndex = treeInstance.m_infoIndex
                });
            }
        }
    }

    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("MoveTree")]
    public class MoveTree
    {
        public static void Postfix(uint tree, Vector3 position)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new TreeMoveCommand
            {
                TreeId = tree,
                Position = position
            });
        }
    }

    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("ReleaseTree")]
    public class ReleaseTree
    {
        public static void Prefix(uint tree)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new TreeReleaseCommand
            {
                TreeId = tree
            });
        }
    }
}
