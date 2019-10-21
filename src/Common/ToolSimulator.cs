using System;
using System.Collections.Generic;

namespace CSM.Common
{
    public static class ToolSimulator
    {
        private static readonly Dictionary<int, ToolBase> _currentTools = new Dictionary<int, ToolBase>();

        public static T GetTool<T>(int sender) where T : ToolBase
        {
            ToolBase tool;
            if (_currentTools.ContainsKey(sender))
            {
                tool = _currentTools[sender];
                if (tool.GetType() == typeof(T))
                {
                    return (T)tool;
                }
            }

            tool = (ToolBase)Activator.CreateInstance(typeof(T));

            ToolController controller = new ToolController();
            ReflectionHelper.SetAttr(tool, "m_toolController", controller);

            // See ToolController::Awake
            ReflectionHelper.SetAttr(controller, "m_brushData", new float[4096]);

            _currentTools[sender] = tool;
            return (T)tool;
        }

        public static void RemoveSender(int sender)
        {
            _currentTools.Remove(sender);
        }

        public static void Clear()
        {
            _currentTools.Clear();
        }
    }
}
