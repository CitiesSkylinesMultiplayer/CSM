using System.Reflection;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using CSM.BaseGame.Helpers;
using UnityEngine;

namespace CSM.BaseGame.Commands.Handler.Campus
{
    public class SetVarsityColorHandler : CommandHandler<SetVarsityColorCommand>
    {
        protected override void Handle(SetVarsityColorCommand command)
        {
            ref DistrictPark districtPark = ref Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus];

            // We need to set the color beforehand, as the call to SetVarsityColor will copy the park struct and not reflect the results on the object
            districtPark.m_varsityColor = command.Color;
            ReflectionHelper.Call(districtPark, "SetVarsityColor", command.Campus, command.Color);

            // Refresh panel if is campus
            if (InfoPanelHelper.IsPark(typeof(CampusWorldInfoPanel), command.Campus, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call((CampusWorldInfoPanel)panel, "OnSetTarget");
                });
            }
        }
    }
}
