using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Weather
{
    /// <summary>
    ///     Called when the weather has been changed.
    /// </summary>
    /// Sent by:
    /// - WeatherHandler
    [ProtoContract]
    public class WeatherCommand : CommandBase
    {
        /// <summary>
        ///     The current cloud
        /// </summary>
        [ProtoMember(1)]
        public float CurrentCloud { get; set; }

        /// <summary>
        ///     The target cloud
        /// </summary>
        [ProtoMember(2)]
        public float TargetCloud { get; set; }

        /// <summary>
        ///     The current fog
        /// </summary>
        [ProtoMember(3)]
        public float CurrentFog { get; set; }

        /// <summary>
        ///     The target fog
        /// </summary>
        [ProtoMember(4)]
        public float TargetFog { get; set; }

        /// <summary>
        ///     The current nothern lights
        /// </summary>
        [ProtoMember(5)]
        public float CurrentNothernLights { get; set; }

        /// <summary>
        ///     The target nothern lights
        /// </summary>
        [ProtoMember(6)]
        public float TargetNothernLights { get; set; }

        /// <summary>
        ///     The current rain
        /// </summary>
        [ProtoMember(7)]
        public float CurrentRain { get; set; }

        /// <summary>
        ///     The target rain
        /// </summary>
        [ProtoMember(8)]
        public float TargetRain { get; set; }

        /// <summary>
        ///     The current rainbow
        /// </summary>
        [ProtoMember(9)]
        public float CurrentRainbow { get; set; }

        /// <summary>
        ///     The target rainbow
        /// </summary>
        [ProtoMember(10)]
        public float TargetRainbow { get; set; }

        /// <summary>
        ///     The current temperature
        /// </summary>
        [ProtoMember(11)]
        public float CurrentTemperature { get; set; }

        /// <summary>
        ///     The target temperature
        /// </summary>
        [ProtoMember(12)]
        public float TargetTemperature { get; set; }
    }
}
