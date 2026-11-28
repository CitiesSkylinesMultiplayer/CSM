using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Commands.Handler.Buildings;
using CSM.BaseGame.Helpers;
using CSM.BaseGame.Injections;
using CSM.Models;
using ProtoBuf.Meta;
using UnityEngine;

internal static class BuildingStatusSyncTests
{
    private static int _passed;
    private const ushort Id = 42;
    private static readonly string Host = new string('a', 32);

    private static void Check(bool result, string message)
    {
        if (!result)
            throw new Exception(message);
    }

    private static void Run(string name, Action action)
    {
        action();
        Console.WriteLine("PASS " + name);
        _passed++;
    }

    private static Building CreateBuilding()
    {
        return new Building
        {
            m_flags = Building.Flags.Created | Building.Flags.Active | Building.Flags.Completed,
            m_buildIndex = 70,
            m_infoIndex = 3,
            m_position = new Vector3(10, 12, 14),
            m_angle = 90,
            m_width = 2,
            m_length = 4,
            m_frame0 = new Building.Frame { m_constructState = 255 }
        };
    }

    private static BuildingStatusState State(ref Building building, ulong token = 0)
    {
        BuildingStatusState state = BuildingStatusState.Capture(Id, ref building, new BuildingHomeHealth());
        state.HostBuildIndex = 7000;
        state.RequestToken = token;
        return state;
    }

    private static RuntimeTypeModel Model()
    {
        RuntimeTypeModel model = RuntimeTypeModel.Create();
        model[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
        model.Add(typeof(CommandBase), true);
        model[typeof(CommandBase)].AddSubType(100, typeof(BuildingStatusCommand));
        model[typeof(CommandBase)].AddSubType(101, typeof(RequestBuildingStatusCommand));
        model.Add(typeof(BuildingStatusCommand), true);
        model.Add(typeof(RequestBuildingStatusCommand), true);
        model.CompileInPlace();
        return model;
    }

    private static T RoundTrip<T>(RuntimeTypeModel model, T value) where T : CommandBase
    {
        using (MemoryStream stream = new MemoryStream())
        {
            model.Serialize(stream, value);
            stream.Position = 0;
            return (T)model.Deserialize(stream, null, typeof(CommandBase));
        }
    }

    private static BuildingStatusState Bind(BuildingStatusCache cache, ref Building building)
    {
        BuildingStatusRequest request = cache.Request(Id, ref building, 100, 20);
        BuildingStatusState state = State(ref building, request.Token);
        Check(cache.Accept(state, cache.Session, Host, ref building), "baseline was rejected");
        return state;
    }

    private static void VerifyArguments(Type patch, MethodBase target)
    {
        Check(target != null, patch.Name + " has a missing game target");
        foreach (MethodInfo hook in patch.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "Prefix" || m.Name == "Postfix" || m.Name == "Finalizer"))
        {
            foreach (ParameterInfo parameter in hook.GetParameters())
            {
                if (parameter.Name.StartsWith("__"))
                    continue;
                ParameterInfo original = target.GetParameters().FirstOrDefault(p => p.Name == parameter.Name);
                Check(original != null, patch.Name + " references missing argument " + parameter.Name);
                Type parameterType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                Type originalType = original.ParameterType.IsByRef ? original.ParameterType.GetElementType() : original.ParameterType;
                Check(parameterType == originalType, patch.Name + " argument type mismatch: " + parameter.Name);
            }
        }
    }

    private static void PacketRoundTrip()
    {
        Building b = CreateBuilding();
        BuildingStatusState state = State(ref b, 9);
        state.Health = 80;
        state.ChildHealth = 90;
        state.SeniorHealth = 60;
        state.Problems1 = (1UL << 63) | (ulong)Notification.Problem1.Fire;
        state.Problems2 = (1UL << 63) | 5;
        state.HomeHealth = new BuildingHomeHealth { Sick = 3, TotalSick = 5, SickChildren = 2, SickSeniors = 1 };
        RuntimeTypeModel model = Model();
        BuildingStatusCommand decoded = RoundTrip(model, new BuildingStatusCommand
        {
            SenderId = -1,
            HostSession = Host,
            RequestSession = Host,
            Buildings = new List<BuildingStatusState> { state }
        });
        Check(decoded.SenderId == -1 && decoded.Buildings[0].SameAs(state), "state changed over wire");
        Check(decoded.Buildings[0].RequestToken == 9 && decoded.Buildings[0].Position.Equals(b.m_position), "identity lost");
        state.Problems1 = state.Problems2 = 0;
        state.HomeHealth = new BuildingHomeHealth();
        decoded = RoundTrip(model, new BuildingStatusCommand { HostSession = Host, Buildings = new List<BuildingStatusState> { state } });
        Check(decoded.Buildings[0].Problems1 == 0 && decoded.Buildings[0].Problems2 == 0 && decoded.Buildings[0].HomeHealth.Sick == 0,
            "default-valued clears were not transmitted");
    }

    private static void FireRecovery()
    {
        Building b = CreateBuilding();
        b.m_problems = new Notification.ProblemStruct(Notification.Problem1.Fire | Notification.Problem1.MajorProblem,
            (Notification.Problem2)4);
        b.m_fireIntensity = 50;
        b.m_frame0.m_fireDamage = b.m_frame1.m_fireDamage = b.m_frame2.m_fireDamage = b.m_frame3.m_fireDamage = 200;
        Building host = CreateBuilding();
        BuildingStatusState repaired = State(ref host);
        repaired.ApplyFields(ref b);
        Check(b.m_problems.IsNone && b.m_fireIntensity == 0 && b.m_frame0.m_fireDamage == 0 &&
            b.m_frame1.m_fireDamage == 0 && b.m_frame2.m_fireDamage == 0 && b.m_frame3.m_fireDamage == 0,
            "a client frame retained fire or a warning");
    }

    private static void PreserveUnrelatedFields()
    {
        Building b = CreateBuilding();
        b.m_flags |= Building.Flags.Historical | Building.Flags.CustomName | Building.Flags.Incoming;
        b.m_citizenUnits = 1234;
        b.m_guestVehicles = 5;
        b.m_ownVehicles = 6;
        b.m_subBuilding = 10;
        b.m_productionRate = 75;
        b.m_frame0.m_constructState = 123;
        Building host = CreateBuilding();
        BuildingStatusState state = State(ref host);
        state.ConstructState = 255;
        state.ApplyFields(ref b);
        Check((b.m_flags & (Building.Flags.Historical | Building.Flags.CustomName | Building.Flags.Incoming)) ==
            (Building.Flags.Historical | Building.Flags.CustomName | Building.Flags.Incoming), "settings replaced");
        Check(b.m_citizenUnits == 1234 && b.m_guestVehicles == 5 && b.m_ownVehicles == 6 && b.m_subBuilding == 10 &&
            b.m_buildIndex == 70 && b.m_productionRate == 75 && b.m_frame0.m_constructState == 123,
            "unrelated game state overwritten");
    }

    private static void CollapseAndRebuild()
    {
        Building b = CreateBuilding();
        Building host = CreateBuilding();
        host.m_flags |= Building.Flags.Collapsed;
        host.m_frame0.m_constructState = 0;
        host.m_frame0.m_fireDamage = 255;
        State(ref host).ApplyFields(ref b);
        Check((b.m_flags & Building.Flags.Collapsed) != 0 && b.m_frame3.m_constructState == 0, "collapse history not applied");
        host = CreateBuilding();
        State(ref host).ApplyFields(ref b);
        Check((b.m_flags & Building.Flags.Collapsed) == 0 && b.m_frame3.m_constructState == 255 && b.m_frame3.m_fireDamage == 0,
            "rebuild retained collapse damage");
    }

    private static void HomeHealthRecovery()
    {
        Building b = CreateBuilding();
        BuildingStatusState state = State(ref b);
        state.Health = 70;
        state.ChildHealth = 80;
        state.SeniorHealth = 60;
        state.HomeHealth = new BuildingHomeHealth { Sick = 2, TotalSick = 3, SickChildren = 1, SickSeniors = 1, Dead = 1 };
        Citizen.BehaviourData behaviour = new Citizen.BehaviourData
        {
            m_sickCount = 9,
            m_totalSickCount = 10,
            m_childCount = 1,
            m_teenCount = 2,
            m_seniorCount = 3,
            m_incomeAccumulation = 500,
            m_waterConsumption = 7
        };
        state.ApplyHomeHealth(ref behaviour, 10);
        Check(behaviour.m_sickCount == 2 && behaviour.m_totalSickCount == 3 && behaviour.m_deadCount == 1 &&
            behaviour.m_healthAccumulation == 700 && behaviour.m_childHealthAccumulation == 240 && behaviour.m_seniorHealthAccumulation == 180,
            "host health summary did not replace local sickness");
        Check(behaviour.m_childCount == 1 && behaviour.m_teenCount == 2 && behaviour.m_seniorCount == 3 &&
            behaviour.m_incomeAccumulation == 500 && behaviour.m_waterConsumption == 7, "population or economy aggregates replaced");
        state.HomeHealth = new BuildingHomeHealth();
        state.ApplyHomeHealth(ref behaviour, 10);
        Check(behaviour.m_sickCount == 0 && behaviour.m_totalSickCount == 0 && behaviour.m_deadCount == 0, "recovery left stale sickness");
    }

    private static void BaselineBuildIndices()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        Bind(cache, ref b);
        Check(cache.Get(Id, ref b).HostBuildIndex == 7000 && b.m_buildIndex == 70 && cache.BoundCount == 1 && cache.PendingCount == 0,
            "binding assumed equal build counters");
    }

    private static void RejectStaleBroadcasts()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        cache.Seed(Id, ref b);
        Check(!cache.Accept(State(ref b), null, Host, ref b), "unsolicited status bound a building");
        Bind(cache, ref b);
        BuildingStatusState stale = State(ref b);
        stale.HostBuildIndex++;
        Check(!cache.Accept(stale, null, Host, ref b), "different host lifetime accepted");
        stale.HostBuildIndex--;
        Check(cache.Accept(stale, null, Host, ref b), "valid delta rejected");
    }

    private static void BuildingIdReuse()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        BuildingStatusRequest old = cache.Request(Id, ref b, 100, 20);
        cache.Invalidate(Id);
        BuildingStatusRequest next = cache.Request(Id, ref b, 101, 20);
        Check(next.Token != old.Token && cache.PendingCount == 1, "reuse retained request identity");
        Check(!cache.Accept(State(ref b, old.Token), cache.Session, Host, ref b), "old reply attached to reused slot");
        Check(cache.Accept(State(ref b, next.Token), cache.Session, Host, ref b), "new lifetime failed to bind");
    }

    private static void ReloadSession()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        string oldSession = cache.Session;
        BuildingStatusRequest old = cache.Request(Id, ref b, 100, 20);
        cache.Reset();
        BuildingStatusRequest next = cache.Request(Id, ref b, 100, 20);
        Check(old.Token == next.Token && oldSession != cache.Session, "test did not exercise token reuse");
        Check(!cache.Accept(State(ref b, old.Token), oldSession, Host, ref b), "old save reply accepted");
        Check(cache.Accept(State(ref b, next.Token), cache.Session, Host, ref b), "new save baseline rejected");
    }

    private static void RejectMalformedState()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        Bind(cache, ref b);
        BuildingStatusState state = State(ref b);
        b.m_buildIndex++;
        Check(!cache.Accept(state, null, Host, ref b), "local reuse was not detected");
        b.m_buildIndex--;
        state.Position = new Vector3(11, 12, 14);
        Check(!cache.Accept(state, null, Host, ref b), "wrong location accepted");
        state = State(ref b);
        state.ProblemTimers = new byte[1];
        Check(!cache.Accept(state, null, Host, ref b), "truncated timers accepted");
        state = State(ref b);
        state.Conditions |= (uint)Building.Flags.Deleted;
        Check(!cache.Accept(state, null, Host, ref b), "structural flag injection accepted");
        b.m_flags |= Building.Flags.Deleted;
        Check(cache.Get(Id, ref b) == null, "deleted building retained live status");
    }

    private static void BaselineRetry()
    {
        Building b = CreateBuilding();
        BuildingStatusCache cache = new BuildingStatusCache();
        BuildingStatusRequest first = cache.Request(Id, ref b, 0, 100);
        Check(cache.Request(Id, ref b, 99, 100) == null, "retry too early");
        Check(cache.Request(Id, ref b, 100, 100).Token == first.Token && cache.PendingCount == 1, "retry lost nonce/accounting");
        BuildingStatusState sick = State(ref b);
        sick.HomeHealth.Sick = sick.HomeHealth.TotalSick = 1;
        Check(!sick.SameAs(State(ref b)), "recovery did not cause a delta");
        sick.Problems2 = 1;
        Check(!sick.SameAs(State(ref b)), "secondary warning did not cause a delta");
    }

    private static void HostFireAuthority()
    {
        Building b = CreateBuilding();
        Building.Frame frame = b.GetLastFrameData();
        bool result = false;
        foreach (MultiplayerRole role in new[] { MultiplayerRole.None, MultiplayerRole.Server })
        {
            Command.CurrentRole = role;
            Check(HostFireSimulationPatch.Prefix(Id, ref b, ref frame) && HostIgnitionPatch.Prefix(false, ref result) &&
                HostExtinguishingPatch.Prefix(ref b, ref result), "host/single-player fire blocked");
        }
        Command.CurrentRole = MultiplayerRole.Client;
        Check(!HostFireSimulationPatch.Prefix(Id, ref b, ref frame) && !HostIgnitionPatch.Prefix(false, ref result), "client can ignite/progress");
        Check(HostIgnitionPatch.Prefix(true, ref result), "test-only ignition query blocked");
        b.m_fireIntensity = 80;
        Check(!HostExtinguishingPatch.Prefix(ref b, ref result) && !result && b.m_fireIntensity == 80,
            "client extinguished host fire");
        b.m_fireIntensity = 0;
        Check(!HostExtinguishingPatch.Prefix(ref b, ref result) && result, "local truck cannot finish after host clear");
        Command.CurrentRole = MultiplayerRole.None;
    }

    private static void PreserveExplicitDemolition()
    {
        Command.CurrentRole = MultiplayerRole.Client;
        bool result = true;
        Check(HostSimulationReleasePatch.Prefix(null, Id), "tool release blocked outside simulation");
        BuildingStatusSync.EnterClientSimulation();
        try
        {
            Check(!HostSimulationReleasePatch.AllowSimulationRelease(Building.Flags.Created), "client simulation can delete host building");
            Check(HostSimulationReleasePatch.AllowSimulationRelease(Building.Flags.Created | Building.Flags.Demolishing),
                "explicit demolition cannot finish inside simulation");
            Check(!HostSimulationCollapsePatch.Prefix(false, ref result) && !result, "client can collapse independently");
            Check(HostSimulationCollapsePatch.Prefix(true, ref result), "test-only collapse query blocked");
        }
        finally { BuildingStatusSync.ExitClientSimulation(); Command.CurrentRole = MultiplayerRole.None; }
    }

    private static void PatchTargetMetadata()
    {
        List<MethodBase> wrappers = BuildingSimulationStatusPatch.TargetMethods().ToList();
        Check(wrappers.Count >= 4 && wrappers.Distinct().Count() == wrappers.Count, "simulation targets missing/duplicated");
        foreach (MethodBase m in wrappers)
            VerifyArguments(typeof(BuildingSimulationStatusPatch), m);
        foreach (MethodBase m in HostExtinguishingPatch.TargetMethods())
            VerifyArguments(typeof(HostExtinguishingPatch), m);
        foreach (MethodBase m in BuildingStatusRebuildPatch.TargetMethods())
        {
            VerifyArguments(typeof(BuildingStatusRebuildPatch), m);
            Check(m.DeclaringType.GetField("buildingID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null,
                "rebuild identity field missing");
        }
        VerifyArguments(typeof(HostFireSimulationPatch), typeof(CommonBuildingAI).GetMethod("HandleFire", BindingFlags.Instance | BindingFlags.NonPublic));
        VerifyArguments(typeof(HostHomeHealthPatch), typeof(CommonBuildingAI).GetMethod("GetHomeBehaviour", BindingFlags.Instance | BindingFlags.NonPublic));
        VerifyArguments(typeof(HostIgnitionPatch), typeof(CommonBuildingAI).GetMethod("BurnBuilding"));
        VerifyArguments(typeof(HostSimulationCollapsePatch), typeof(CommonBuildingAI).GetMethod("CollapseBuilding"));
        VerifyArguments(typeof(BuildingStatusCreatePatch), typeof(BuildingManager).GetMethod("CreateBuilding"));
        VerifyArguments(typeof(BuildingStatusReleasePatch), typeof(BuildingManager).GetMethod("ReleaseBuildingImplementation", BindingFlags.Instance | BindingFlags.NonPublic));
        VerifyArguments(typeof(BuildingStatusRelocatePatch), typeof(BuildingManager).GetMethod("RelocateBuildingImpl", BindingFlags.Instance | BindingFlags.NonPublic));
        VerifyArguments(typeof(BuildingStatusInfoPatch), typeof(BuildingManager).GetMethod("UpdateBuildingInfo", new[] { typeof(ushort), typeof(BuildingInfo) }));
        Console.WriteLine("Verified " + wrappers.Count + " simulation wrappers and rebuild/fire metadata.");
    }

    private static void PacketBatchSize()
    {
        Check(!new BuildingStatusHandler().RelayOnServer && !new RequestBuildingStatusHandler().RelayOnServer,
            "a client can relay status authority");
        Building b = CreateBuilding();
        List<BuildingStatusState> batch = new List<BuildingStatusState>();
        for (int i = 1; i <= BuildingStatusSync.BatchSize; i++)
        {
            BuildingStatusState state = State(ref b, ulong.MaxValue);
            state.BuildingId = (ushort)i;
            state.Problems1 = state.Problems2 = ulong.MaxValue;
            state.HomeHealth = new BuildingHomeHealth
            {
                Sick = int.MaxValue,
                TotalSick = int.MaxValue,
                SickChildren = int.MaxValue,
                SickSeniors = int.MaxValue,
                Dead = int.MaxValue
            };
            state.ProblemTimers = Enumerable.Repeat((byte)255, BuildingStatusState.TimerCount).ToArray();
            batch.Add(state);
        }
        using (MemoryStream stream = new MemoryStream())
        {
            Model().Serialize(stream, new BuildingStatusCommand { SenderId = -1, HostSession = Host, RequestSession = Host, Buildings = batch });
            Check(stream.Length < 16384, "full packet exceeds bounded test size");
            Console.WriteLine("Full 64-building test packet: " + stream.Length + " bytes.");
        }
    }

    public static int Main()
    {
        try
        {
            Run("wire round-trip preserves full warning masks, zero clears, health and identity", PacketRoundTrip);
            Run("a repaired fire clears every interpolation frame and both warning fields", FireRecovery);
            Run("condition updates preserve structure, player settings and local population/vehicle links", PreserveUnrelatedFields);
            Run("collapse and rebuild update construction history without retaining a collapsed frame", CollapseAndRebuild);
            Run("building sickness/recovery follows host summaries without changing unrelated population aggregates", HomeHealthRecovery);
            Run("baseline binds different local and host build counters without changing local identity", BaselineBuildIndices);
            Run("broadcasts cannot bind an unknown building and stale host lifetimes cannot overwrite a binding", RejectStaleBroadcasts);
            Run("ID reuse at the same position and prefab rejects an old baseline token", BuildingIdReuse);
            Run("a save reload rejects late replies even when token numbers restart", ReloadSession);
            Run("local identity, moved geometry, malformed timers and unknown condition flags are rejected", RejectMalformedState);
            Run("baseline retry is throttled and zero sickness counts cause a delta", BaselineRetry);
            Run("fire authority leaves host/single-player intact and clients cannot extinguish independently", HostFireAuthority);
            Run("client simulation cannot delete/collapse a building but explicit tool actions still run", PreserveExplicitDemolition);
            Run("all game patch targets exist and named arguments match the original DLL metadata", PatchTargetMetadata);
            Run("status packets never relay client authority and a full batch remains bounded", PacketBatchSize);
            Console.WriteLine(_passed + " regression groups passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            ReflectionTypeLoadException load = ex as ReflectionTypeLoadException;
            if (load != null)
            foreach (string message in load.LoaderExceptions.Select(e => e.Message).Distinct())
                Console.Error.WriteLine(message);
            return 1;
        }
    }
}
