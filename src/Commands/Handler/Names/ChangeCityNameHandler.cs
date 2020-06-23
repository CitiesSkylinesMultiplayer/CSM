using ColossalFramework.UI;
using CSM.Commands.Data.Names;
using CSM.Helpers;
using NLog;

namespace CSM.Commands.Handler.Names
{
    public class ChangeCityNameHandler : CommandHandler<ChangeCityNameCommand>
    {
        public static InfoPanel Panel;

        protected override void Handle(ChangeCityNameCommand command)
        {
            IgnoreHelper.StartIgnore();

            // Update name internally
            CityInfoPanel.instance.SetCityName(command.Name).MoveNext();

            // Update name in panel
            ReflectionHelper.GetAttr<UITextField>(CityInfoPanel.instance, "m_CityName").text = command.Name;

            // Update name in bottom bar
            if (Panel != null)
                ReflectionHelper.Call(Panel, "SetName");
            else
                LogManager.GetCurrentClassLogger().Warn("Bottom bar city name not found. Can't update name!");

            IgnoreHelper.EndIgnore();
        }
    }
}
