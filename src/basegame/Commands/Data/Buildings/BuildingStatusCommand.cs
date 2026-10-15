using System.Collections.Generic;
using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    [ProtoContract]
    public class BuildingStatusCommand : CommandBase
    {
        [ProtoMember(1)]
        public string HostSession { get; set; }

        [ProtoMember(2)]
        public string RequestSession { get; set; }

        [ProtoMember(3)]
        public List<BuildingStatusState> Buildings { get; set; }
    }

    [ProtoContract]
    public class RequestBuildingStatusCommand : CommandBase
    {
        [ProtoMember(1)]
        public string Session { get; set; }

        [ProtoMember(2)]
        public List<BuildingStatusRequest> Buildings { get; set; }
    }

    [ProtoContract]
    public class BuildingStatusRequest
    {
        [ProtoMember(1)]
        public ushort BuildingId { get; set; }

        [ProtoMember(2)]
        public ulong Token { get; set; }

        [ProtoMember(3)]
        public ushort InfoIndex { get; set; }

        [ProtoMember(4)]
        public Vector3 Position { get; set; }

        [ProtoMember(5)]
        public float Angle { get; set; }

        [ProtoMember(6)]
        public byte Width { get; set; }

        [ProtoMember(7)]
        public byte Length { get; set; }

        public bool Matches(ref Building building)
        {
            return BuildingStatusState.IsLive(ref building) && building.m_infoIndex == InfoIndex &&
                building.m_position.Equals(Position) && building.m_angle == Angle &&
                building.m_width == Width && building.m_length == Length;
        }
    }

    [ProtoContract]
    public class BuildingHomeHealth
    {
        [ProtoMember(1)]
        public int Sick { get; set; }

        [ProtoMember(2)]
        public int TotalSick { get; set; }

        [ProtoMember(3)]
        public int SickChildren { get; set; }

        [ProtoMember(4)]
        public int SickSeniors { get; set; }

        [ProtoMember(5)]
        public int Dead { get; set; }

        public bool SameAs(BuildingHomeHealth other)
        {
            return other != null && Sick == other.Sick && TotalSick == other.TotalSick &&
                SickChildren == other.SickChildren && SickSeniors == other.SickSeniors && Dead == other.Dead;
        }
    }

    [ProtoContract]
    public class BuildingStatusState
    {
        // Never replace structural flags, settings, IDs, buffer links, or the local build index.
        public const Building.Flags ConditionMask = Building.Flags.Active | Building.Flags.Abandoned |
            Building.Flags.Collapsed | Building.Flags.Flooded | Building.Flags.RoadAccessFailed | Building.Flags.RateReduced;
        public const int TimerCount = 11;

        [ProtoMember(1)]
        public ushort BuildingId { get; set; }

        [ProtoMember(2)]
        public uint HostBuildIndex { get; set; }

        [ProtoMember(3)]
        public ushort InfoIndex { get; set; }

        [ProtoMember(4)]
        public Vector3 Position { get; set; }

        [ProtoMember(5)]
        public float Angle { get; set; }

        [ProtoMember(6)]
        public byte Width { get; set; }

        [ProtoMember(7)]
        public byte Length { get; set; }

        [ProtoMember(8)]
        public uint Conditions { get; set; }

        [ProtoMember(9)]
        public ulong Problems1 { get; set; }

        [ProtoMember(10)]
        public ulong Problems2 { get; set; }

        [ProtoMember(11)]
        public byte FireIntensity { get; set; }

        [ProtoMember(12)]
        public byte FireDamage { get; set; }

        [ProtoMember(13)]
        public byte ConstructState { get; set; }

        [ProtoMember(14)]
        public byte Health { get; set; }

        [ProtoMember(15)]
        public byte ChildHealth { get; set; }

        [ProtoMember(16)]
        public byte SeniorHealth { get; set; }

        [ProtoMember(17)]
        public byte[] ProblemTimers { get; set; }

        [ProtoMember(18)]
        public BuildingHomeHealth HomeHealth { get; set; }
        // Only baseline replies carry a token; broadcasts cannot bind a new building lifetime.
        [ProtoMember(19)]
        public ulong RequestToken { get; set; }

        [ProtoMember(20)]
        public byte WaterPollution { get; set; }

        [ProtoMember(21)]
        public byte Happiness { get; set; }

        public static bool IsLive(ref Building building)
        {
            return (building.m_flags & (Building.Flags.Created | Building.Flags.Deleted)) == Building.Flags.Created;
        }

        public static BuildingStatusState Capture(ushort id, ref Building building, BuildingHomeHealth homeHealth)
        {
            Building.Frame frame = building.GetLastFrameData();
            return new BuildingStatusState
            {
                BuildingId = id,
                HostBuildIndex = building.m_buildIndex,
                InfoIndex = building.m_infoIndex,
                Position = building.m_position,
                Angle = building.m_angle,
                Width = building.m_width,
                Length = building.m_length,
                Conditions = (uint)(building.m_flags & ConditionMask),
                Problems1 = (ulong)building.m_problems.m_Problems1,
                Problems2 = (ulong)building.m_problems.m_Problems2,
                FireIntensity = building.m_fireIntensity,
                FireDamage = frame.m_fireDamage,
                ConstructState = frame.m_constructState,
                Health = building.m_health,
                ChildHealth = building.m_childHealth,
                SeniorHealth = building.m_seniorHealth,
                WaterPollution = building.m_waterPollution,
                Happiness = building.m_happiness,
                HomeHealth = homeHealth,
                ProblemTimers = new[] { building.m_electricityProblemTimer, building.m_heatingProblemTimer,
                    building.m_waterProblemTimer, building.m_workerProblemTimer, building.m_incomingProblemTimer,
                    building.m_outgoingProblemTimer, building.m_healthProblemTimer, building.m_deathProblemTimer,
                    building.m_serviceProblemTimer, building.m_taxProblemTimer, building.m_majorProblemTimer }
            };
        }

        public bool MatchesGeometry(ref Building building)
        {
            return IsLive(ref building) && building.m_infoIndex == InfoIndex && building.m_position.Equals(Position) &&
                building.m_angle == Angle && building.m_width == Width && building.m_length == Length;
        }

        public bool IsValid()
        {
            return BuildingId != 0 && ProblemTimers != null && ProblemTimers.Length == TimerCount &&
                (Conditions & ~(uint)ConditionMask) == 0 &&
                !float.IsNaN(Position.x) && !float.IsNaN(Position.y) && !float.IsNaN(Position.z) &&
                !float.IsInfinity(Position.x) && !float.IsInfinity(Position.y) && !float.IsInfinity(Position.z) &&
                !float.IsNaN(Angle) && !float.IsInfinity(Angle) &&
                (HomeHealth == null || (HomeHealth.Sick >= 0 && HomeHealth.TotalSick >= HomeHealth.Sick &&
                    HomeHealth.SickChildren >= 0 && HomeHealth.SickSeniors >= 0 && HomeHealth.Dead >= 0));
        }

        public bool SameAs(BuildingStatusState other)
        {
            if (other == null || HostBuildIndex != other.HostBuildIndex || InfoIndex != other.InfoIndex ||
                !Position.Equals(other.Position) || Angle != other.Angle || Width != other.Width || Length != other.Length ||
                Conditions != other.Conditions || Problems1 != other.Problems1 || Problems2 != other.Problems2 ||
                FireIntensity != other.FireIntensity || FireDamage != other.FireDamage ||
                (((Conditions | other.Conditions) & (uint)Building.Flags.Collapsed) != 0 && ConstructState != other.ConstructState) ||
                Health != other.Health || ChildHealth != other.ChildHealth || SeniorHealth != other.SeniorHealth ||
                WaterPollution != other.WaterPollution || Happiness != other.Happiness ||
                (HomeHealth == null ? other.HomeHealth != null : !HomeHealth.SameAs(other.HomeHealth)))
                return false;
            for (int i = 0; i < TimerCount; i++)
                if (ProblemTimers[i] != other.ProblemTimers[i])
                    return false;
            return true;
        }

        public void ApplyHomeHealth(ref Citizen.BehaviourData behaviour, int aliveCount)
        {
            if (HomeHealth == null)
                return;
            behaviour.m_sickCount = HomeHealth.Sick;
            behaviour.m_totalSickCount = HomeHealth.TotalSick;
            behaviour.m_sickChildCount = HomeHealth.SickChildren;
            behaviour.m_sickSeniorCount = HomeHealth.SickSeniors;
            behaviour.m_deadCount = HomeHealth.Dead;
            // Preserve local population/identity while keeping the building average health authoritative.
            behaviour.m_healthAccumulation = Health * aliveCount;
            behaviour.m_childHealthAccumulation = ChildHealth * (behaviour.m_childCount + behaviour.m_teenCount);
            behaviour.m_seniorHealthAccumulation = SeniorHealth * behaviour.m_seniorCount;
        }

        public void ApplyFields(ref Building building)
        {
            bool wasCollapsed = (building.m_flags & Building.Flags.Collapsed) != 0;
            building.m_flags = (building.m_flags & ~ConditionMask) | (Building.Flags)Conditions;
            building.m_problems = new Notification.ProblemStruct((Notification.Problem1)Problems1, (Notification.Problem2)Problems2);
            building.m_fireIntensity = FireIntensity;
            // Fill the interpolation history too, so a repaired fire cannot leave a stale burning frame behind.
            building.m_frame0.m_fireDamage = building.m_frame1.m_fireDamage =
                building.m_frame2.m_fireDamage = building.m_frame3.m_fireDamage = FireDamage;
            if (wasCollapsed || (building.m_flags & Building.Flags.Collapsed) != 0)
                building.m_frame0.m_constructState = building.m_frame1.m_constructState =
                    building.m_frame2.m_constructState = building.m_frame3.m_constructState = ConstructState;
            building.m_health = Health;
            building.m_childHealth = ChildHealth;
            building.m_seniorHealth = SeniorHealth;
            building.m_electricityProblemTimer = ProblemTimers[0];
            building.m_heatingProblemTimer = ProblemTimers[1];
            building.m_waterProblemTimer = ProblemTimers[2];
            building.m_workerProblemTimer = ProblemTimers[3];
            building.m_incomingProblemTimer = ProblemTimers[4];
            building.m_outgoingProblemTimer = ProblemTimers[5];
            building.m_healthProblemTimer = ProblemTimers[6];
            building.m_deathProblemTimer = ProblemTimers[7];
            building.m_serviceProblemTimer = ProblemTimers[8];
            building.m_taxProblemTimer = ProblemTimers[9];
            building.m_majorProblemTimer = ProblemTimers[10];
            building.m_waterPollution = WaterPollution;
            building.m_happiness = Happiness;
        }
    }
}
