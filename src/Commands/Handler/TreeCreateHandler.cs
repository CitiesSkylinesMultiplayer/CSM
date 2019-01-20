using ColossalFramework;
using CSM.Helpers;
using Harmony;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class TreeCreateHandler : CommandHandler<TreeCreateCommand>
    {
        private MethodInfo _initializeTree;

        public TreeCreateHandler()
        {
            _initializeTree = typeof(TreeManager).GetMethod("InitializeTree", AccessTools.all);
        }

        public override void Handle(TreeCreateCommand command)
        {
            TreeInfo info = PrefabCollection<TreeInfo>.GetPrefab(command.InfoIndex);
            uint tree = command.TreeID;
            TreeManager.instance.m_trees.RemoveUnused(tree);

            TreeManager.instance.m_trees.m_buffer[tree].m_flags = 1;
            TreeManager.instance.m_trees.m_buffer[tree].Info = info;
            TreeManager.instance.m_trees.m_buffer[tree].Single = command.Single;
            TreeManager.instance.m_trees.m_buffer[tree].GrowState = 15;
            TreeManager.instance.m_trees.m_buffer[tree].Position = command.Position;
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte park = instance.GetPark(command.Position);
            instance.m_parks.m_buffer[park].m_treeCount++;
            ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;
            _initializeTree.Invoke(Singleton<TreeManager>.instance, new object[] { tree, Singleton<TreeManager>.instance.m_trees.m_buffer[tree], ((mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None) });
            TreeManager.instance.UpdateTree(tree);
            TreeManager.instance.m_treeCount = ((int) TreeManager.instance.m_trees.ItemCount()) - 1;
        }
    }
}
