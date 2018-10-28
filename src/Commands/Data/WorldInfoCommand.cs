using ProtoBuf;
using System;

namespace CSM.Commands
{
    /// <summary>
    ///     Contains generic information about the world. Sent after a client is connected to
    ///     ensure everything is in sync.
    /// </summary>
    [ProtoContract]
    public class WorldInfoCommand : CommandBase
    {
        /// <summary>
        ///     The current day time hour. Set under SimulationManager.m_currentDayTimeHour.
        /// </summary>
        [ProtoMember(1)]
        public float CurrentDayTimeHour { get; set; }

        /// <summary>
        ///     The current game time. Set under SimulationManager.m_currentDayTime.
        /// </summary>
        [ProtoMember(2)]
        public DateTime CurrentGameTime { get; set; }

        /// <summary>
        ///     The name of the city.
        /// </summary>
        [ProtoMember(3)]
        public string CityName { get; set; }
    }
}