using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Weather;

namespace CSM.BaseGame.Commands.Handler.Weather
{
    public class WeatherHandler : CommandHandler<WeatherCommand>
    {
        protected override void Handle(WeatherCommand command)
        {
            if (Command.CurrentRole == MultiplayerRole.Server)
                return;

            IgnoreHelper.Instance.StartIgnore();

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

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
