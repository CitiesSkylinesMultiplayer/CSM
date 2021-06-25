using CSM.API.Commands;
using CSM.Commands.Data.Weather;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler.Weather
{
    public class WeatherHandler : CommandHandler<WeatherCommand>
    {
        protected override void Handle(WeatherCommand command)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                return;

            IgnoreHelper.StartIgnore();

            WeatherManager.instance.m_currentCloud = command.CurrentCloud;
            WeatherManager.instance.m_targetCloud = command.TargetCloud;

            WeatherManager.instance.m_currentFog = command.CurrentFog;
            WeatherManager.instance.m_targetFog = command.TargetFog;

            WeatherManager.instance.m_currentNorthernLights = command.CurrentNothernLights;
            WeatherManager.instance.m_targetNorthernLights = command.TargetNothernLights;

            WeatherManager.instance.m_currentRain = command.CurrentRain;
            WeatherManager.instance.m_targetRain = command.TargetRain;

            WeatherManager.instance.m_currentRainbow = command.CurrentRainbow;
            WeatherManager.instance.m_targetRainbow = command.TargetRainbow;

            WeatherManager.instance.m_currentTemperature = command.CurrentTemperature;
            WeatherManager.instance.m_targetTemperature = command.TargetTemperature;

            IgnoreHelper.EndIgnore();
        }
    }
}
