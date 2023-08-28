using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Campus
{
    public class SetVarsityIdentityHandler : CommandHandler<SetVarsityIdentityCommand>
    {
        protected override void Handle(SetVarsityIdentityCommand command)
        {
            ref DistrictPark districtPark = ref Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus];

            // We need to set the values beforehand, as the call to SetVarsityIdentity will copy the park struct and not reflect the results on the object
            districtPark.m_varsityIdentityIndex = command.Identity;
            districtPark.m_eventPolicies &= ~(DistrictPolicies.Event.Team01Ad | DistrictPolicies.Event.Team02Ad | DistrictPolicies.Event.Team03Ad | DistrictPolicies.Event.Team04Ad | DistrictPolicies.Event.Team05Ad | DistrictPolicies.Event.Team06Ad | DistrictPolicies.Event.Team07Ad);
            districtPark.m_eventPolicies |= DistrictPark.TeamToEventPolicy((DistrictPark.Team) command.Identity);
            ReflectionHelper.Call(districtPark, "SetVarsityIdentity", command.Campus, command.Identity);

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
