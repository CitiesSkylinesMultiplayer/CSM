using ColossalFramework;
using CSM.Helpers;
using Harmony;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class PropCreateHandler : CommandHandler<PropCreateCommand>
    {
        private MethodInfo _initializeProp;

        public PropCreateHandler()
        {
            _initializeProp = typeof(PropManager).GetMethod("InitializeProp", AccessTools.all);
        }

        public override void Handle(PropCreateCommand command)
        {
            PropInfo info = PrefabCollection<PropInfo>.GetPrefab(command.infoindex);
            ushort prop = command.PropID;
            PropManager.instance.m_props.RemoveUnused(prop);
            PropManager.instance.m_props.m_buffer[prop].m_flags = 1;
            PropManager.instance.m_props.m_buffer[prop].Info = info;
            PropManager.instance.m_props.m_buffer[prop].Single = command.single;
            PropManager.instance.m_props.m_buffer[prop].Blocked = false;
            PropManager.instance.m_props.m_buffer[prop].Position = command.position;
            PropManager.instance.m_props.m_buffer[prop].Angle = command.angle;
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte park = instance.GetPark(command.position);
            instance.m_parks.m_buffer[park].m_propCount = (ushort) (instance.m_parks.m_buffer[park].m_propCount + 1);
            ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;
            _initializeProp.Invoke(Singleton<PropManager>.instance, new object[] { prop, Singleton<PropManager>.instance.m_props.m_buffer[prop], ((mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None) });
            PropManager.instance.UpdateProp(prop);
            PropManager.instance.m_propCount = ((int) PropManager.instance.m_props.ItemCount()) - 1;



        }
    }
}
