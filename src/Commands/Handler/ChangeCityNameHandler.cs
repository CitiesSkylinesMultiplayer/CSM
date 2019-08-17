using ColossalFramework.UI;
using CSM.Common;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ChangeCityNameHandler : CommandHandler<ChangeCityNameCommand>
    {
        public static InfoPanel Panel;
        
        public override void Handle(ChangeCityNameCommand command)
        {
            NameHandler.IgnoreAll = true;
            
            // Update name internally
            CityInfoPanel.instance.SetCityName(command.Name).MoveNext();
            
            // Update name in panel
            ReflectionHelper.GetAttr<UITextField>(CityInfoPanel.instance, "m_CityName").text = command.Name;
            
            // Update name in bottom bar
            if (Panel != null)
                ReflectionHelper.Call(Panel, "SetName");

            NameHandler.IgnoreAll = false;
        }
    }
}
