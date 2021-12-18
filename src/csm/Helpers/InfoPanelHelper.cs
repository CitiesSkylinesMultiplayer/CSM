using ColossalFramework.UI;
using System;
using System.Reflection;

namespace CSM.Helpers
{
    public class InfoPanelHelper
    {
        private static FieldInfo _instanceIdField = null;

        public static InstanceID GetInstanceID(Type panelType, out WorldInfoPanel panel)
        {
            panel = UIView.library.Get<WorldInfoPanel>(panelType.Name);
            if (panel == null)
                return InstanceID.Empty;

            if (_instanceIdField == null)
                _instanceIdField = typeof(WorldInfoPanel).GetField("m_InstanceID", ReflectionHelper.AllAccessFlags);

            if (_instanceIdField == null)
                return InstanceID.Empty;

            return (InstanceID)_instanceIdField.GetValue(panel);
        }

        public static bool IsBuilding(Type panelType, ushort buildingId, out WorldInfoPanel panel)
        {
            InstanceID instance = GetInstanceID(panelType, out panel);
            return instance.Type == InstanceType.Building && instance.Building == buildingId;
        }

        public static bool IsEventBuilding(Type panelType, ushort eventId, out WorldInfoPanel panel)
        {
            InstanceID instance = GetInstanceID(panelType, out panel);
            if (instance.Type != InstanceType.Building)
                return false;

            ushort eventIndex = BuildingManager.instance.m_buildings.m_buffer[instance.Building].m_eventIndex;
            return eventIndex == eventId;
        }
    }
}
