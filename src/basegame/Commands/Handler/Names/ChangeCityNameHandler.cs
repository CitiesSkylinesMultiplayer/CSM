﻿using ColossalFramework.UI;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Names;

namespace CSM.BaseGame.Commands.Handler.Names
{
    public class ChangeCityNameHandler : CommandHandler<ChangeCityNameCommand>
    {
        public static InfoPanel Panel;

        protected override void Handle(ChangeCityNameCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            // Update name internally
            CityInfoPanel.instance.SetCityName(command.Name).MoveNext();

            // Update name in panel
            ReflectionHelper.GetAttr<UITextField>(CityInfoPanel.instance, "m_CityName").text = command.Name;

            // Update name in bottom bar
            if (Panel != null)
            {
                ReflectionHelper.Call(Panel, "SetName");
            }
            else
            {
                Log.Warn("Bottom bar city name not found. Can't update name!");
            }                

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
