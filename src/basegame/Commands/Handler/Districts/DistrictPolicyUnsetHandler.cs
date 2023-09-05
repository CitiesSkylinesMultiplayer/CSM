using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictPolicyUnsetHandler : CommandHandler<DistrictPolicyUnsetCommand>
    {
        protected override void Handle(DistrictPolicyUnsetCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            if (command.IsPark)
            {
                DistrictManager.instance.UnsetParkPolicy(command.Policy, command.DistrictId);

                // Refresh panel if is campus
                if (InfoPanelHelper.IsPark(typeof(CampusWorldInfoPanel), command.DistrictId, out WorldInfoPanel panel))
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        ReflectionHelper.Call((CampusWorldInfoPanel)panel, "OnSetTarget");
                    });
                }
            }
            else
            {
                DistrictManager.instance.UnsetDistrictPolicy(command.Policy, command.DistrictId);
            }
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
