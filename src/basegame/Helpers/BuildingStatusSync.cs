using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Helpers
{
    public static class BuildingStatusSync
    {
        public const int BatchSize = 64;
        private const int SweepSize = 512;
        private static readonly long FlushTicks = Math.Max(1, Stopwatch.Frequency / 10);
        private static readonly long RefreshTicks = Stopwatch.Frequency * 10;
        private static readonly long RetryTicks = Stopwatch.Frequency * 2;
        private static readonly BuildingStatusCache ClientCache = new BuildingStatusCache();
        private static readonly HashSet<ushort> Dirty = new HashSet<ushort>();
        private static BuildingStatusState[] _sent = new BuildingStatusState[ushort.MaxValue + 1];
        private static long[] _lastSent = new long[ushort.MaxValue + 1];
        private static string _hostSession = Guid.NewGuid().ToString("N");
        private static MultiplayerRole _role;
        private static bool _connected;
        private static long _lastFlush;
        private static int _cursor = 1;
        private static int _sentStates;
        private static int _receivedStates;
        private static int _resetRequested;
        private static int _resetApplied;

        [ThreadStatic]
        private static int _clientSimulationDepth;

        private static bool ResetPending
        {
            get
            {
                return Interlocked.CompareExchange(ref _resetRequested, 0, 0) != _resetApplied;
            }
        }

        public static string Diagnostics()
        {
            if (!_connected || ResetPending)
                return "Building sync: waiting for a connected multiplayer session.";
            if (Command.CurrentRole == MultiplayerRole.Client)
                return "Building sync: " + ClientCache.BoundCount + " buildings bound to host, " +
                    ClientCache.PendingCount + " baseline requests pending, " + _receivedStates + " states received.";
            return "Building sync: host authority active, " + _sentStates + " building states sent.";
        }

        public static bool InClientSimulation
        {
            get
            {
                return _clientSimulationDepth != 0;
            }
        }

        public static void EnterClientSimulation()
        {
            _clientSimulationDepth++;
        }

        public static void ExitClientSimulation()
        {
            _clientSimulationDepth--;
        }

        // Loading callbacks can run on the UI thread. Only the simulation thread clears mutable caches.
        public static void Reset()
        {
            Interlocked.Increment(ref _resetRequested);
        }

        private static void Clear()
        {
            ClientCache.Reset();
            Dirty.Clear();
            _sent = new BuildingStatusState[ushort.MaxValue + 1];
            _lastSent = new long[ushort.MaxValue + 1];
            _hostSession = Guid.NewGuid().ToString("N");
            _lastFlush = 0;
            _cursor = 1;
            _sentStates = _receivedStates = 0;
            _connected = false;
            _role = MultiplayerRole.None;
        }

        public static void UpdateConnection(MultiplayerRole role, bool connected)
        {
            int resetVersion = Interlocked.CompareExchange(ref _resetRequested, 0, 0);
            if (resetVersion != _resetApplied || _role != role || (_connected && !connected))
            {
                Clear();
                _resetApplied = resetVersion;
            }
            _role = role;
            _connected = connected;
        }

        public static void Invalidate(ushort id)
        {
            if (id == 0)
                return;
            ClientCache.Invalidate(id);
            _sent[id] = null;
            MarkDirty(id);
        }

        public static void MarkDirty(ushort id)
        {
            if (_connected && !ResetPending && Command.CurrentRole == MultiplayerRole.Server && id != 0)
                Dirty.Add(id);
        }

        public static void MarkFamilyDirty(ushort id)
        {
            if (!_connected || ResetPending || Command.CurrentRole != MultiplayerRole.Server)
                return;
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            int visits = 0;
            while (id != 0 && id < buildings.Length && visits++ < buildings.Length)
            {
                MarkDirty(id);
                id = buildings[id].m_subBuilding;
            }
        }

        public static BuildingStatusState GetClientState(ushort id, ref Building building)
        {
            return _connected && !ResetPending && Command.CurrentRole == MultiplayerRole.Client ? ClientCache.Get(id, ref building) : null;
        }

        public static void Enforce(ushort id, ref Building building)
        {
            BuildingStatusState state = _connected && !ResetPending && Command.CurrentRole == MultiplayerRole.Client
                ? ClientCache.Seed(id, ref building) : null;
            if (state != null)
                Apply(id, state, ref building);
        }

        private static void Apply(ushort id, BuildingStatusState state, ref Building building)
        {
            BuildingManager manager = BuildingManager.instance;
            Building.Flags oldFlags = building.m_flags;
            Notification.ProblemStruct oldProblems = building.m_problems;
            Building.Frame oldFrame = building.GetLastFrameData();
            byte oldFire = building.m_fireIntensity;
            byte oldHealth = building.m_health, oldChild = building.m_childHealth, oldSenior = building.m_seniorHealth;
            byte oldHappiness = building.m_happiness;
            state.ApplyFields(ref building);
            IgnoreHelper.Instance.StartIgnore();
            try
            {
                if (oldProblems != building.m_problems)
                {
                    manager.UpdateNotifications(id, oldProblems, building.m_problems);
                    // Clearing a major warning calls CheckRoadAccess, which may otherwise replace the host flags/problems.
                    state.ApplyFields(ref building);
                }
                Building.Flags changedFlags = oldFlags ^ building.m_flags;
                if (changedFlags != 0)
                    manager.UpdateFlags(id, changedFlags);
                if (oldFire != building.m_fireIntensity || oldFrame.m_fireDamage != state.FireDamage ||
                    ((oldFlags | building.m_flags) & Building.Flags.Collapsed) != 0 && oldFrame.m_constructState != state.ConstructState ||
                    (changedFlags & (Building.Flags.Active | Building.Flags.Abandoned | Building.Flags.Collapsed | Building.Flags.Flooded)) != 0)
                {
                    manager.UpdateBuildingRenderer(id, true);
                    manager.UpdateBuildingColors(id);
                }
                else if (oldHealth != building.m_health || oldChild != building.m_childHealth ||
                    oldSenior != building.m_seniorHealth || oldHappiness != building.m_happiness)
                {
                    manager.UpdateBuildingColors(id);
                }
            }
            finally
            {
                IgnoreHelper.Instance.EndIgnore();
            }
        }

        public static void Receive(BuildingStatusCommand command)
        {
            if (!_connected || ResetPending || Command.CurrentRole != MultiplayerRole.Client || command.SenderId != -1 ||
                command.Buildings == null || command.Buildings.Count > BatchSize)
                return;
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            foreach (BuildingStatusState state in command.Buildings)
            {
                if (state == null || state.BuildingId == 0 || state.BuildingId >= buildings.Length)
                    continue;
                ushort id = state.BuildingId;
                if (ClientCache.Accept(state, command.RequestSession, command.HostSession, ref buildings[id]))
                {
                    Apply(id, state, ref buildings[id]);
                    _receivedStates++;
                }
            }
        }

        public static void Answer(RequestBuildingStatusCommand command)
        {
            if (!_connected || ResetPending || Command.CurrentRole != MultiplayerRole.Server || Command.SendToClient == null ||
                string.IsNullOrEmpty(command.Session) || command.Session.Length != 32 ||
                command.Buildings == null || command.Buildings.Count > BatchSize)
                return;
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            List<BuildingStatusState> replies = new List<BuildingStatusState>();
            foreach (BuildingStatusRequest request in command.Buildings)
            {
                if (request == null || request.BuildingId == 0 || request.BuildingId >= buildings.Length || request.Token == 0 ||
                    !request.Matches(ref buildings[request.BuildingId]))
                    continue;
                BuildingStatusState state = Capture(request.BuildingId, ref buildings[request.BuildingId]);
                state.RequestToken = request.Token;
                replies.Add(state);
            }
            if (replies.Count != 0)
                Command.SendToClient(command.SenderId, new BuildingStatusCommand
                {
                    HostSession = _hostSession,
                    RequestSession = command.Session,
                    Buildings = replies
                });
        }

        public static void SimulationStep()
        {
            if (!_connected || ResetPending)
                return;
            long now = Stopwatch.GetTimestamp();
            if (_lastFlush != 0 && now - _lastFlush < FlushTicks)
                return;
            _lastFlush = now;
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            int limit = Math.Min(buildings.Length, ushort.MaxValue + 1);
            if (limit <= 1)
                return;
            if (Command.CurrentRole == MultiplayerRole.Server)
            {
                for (int i = 0; i < SweepSize; i++)
                {
                    if (_cursor >= limit)
                        _cursor = 1;
                    Dirty.Add((ushort)_cursor++);
                }
                List<BuildingStatusState> batch = new List<BuildingStatusState>(BatchSize);
                foreach (ushort id in Dirty)
                {
                    if (id >= limit || !BuildingStatusState.IsLive(ref buildings[id]))
                    {
                        _sent[id] = null;
                        continue;
                    }
                    BuildingStatusState state = Capture(id, ref buildings[id]);
                    if (state.SameAs(_sent[id]) && now - _lastSent[id] < RefreshTicks)
                        continue;
                    batch.Add(state);
                    _sent[id] = state;
                    _lastSent[id] = now;
                    if (batch.Count == BatchSize)
                    {
                        Send(batch);
                        batch = new List<BuildingStatusState>(BatchSize);
                    }
                }
                Dirty.Clear();
                if (batch.Count != 0)
                    Send(batch);
            }
            else if (Command.CurrentRole == MultiplayerRole.Client)
            {
                List<BuildingStatusRequest> batch = new List<BuildingStatusRequest>(BatchSize);
                for (int i = 0; i < SweepSize; i++)
                {
                    if (_cursor >= limit)
                        _cursor = 1;
                    ushort id = (ushort)_cursor++;
                    BuildingStatusRequest request = ClientCache.Request(id, ref buildings[id], now, RetryTicks);
                    if (request == null)
                        continue;
                    batch.Add(request);
                    if (batch.Count == BatchSize)
                    {
                        Request(batch);
                        batch = new List<BuildingStatusRequest>(BatchSize);
                    }
                }
                if (batch.Count != 0)
                    Request(batch);
            }
        }

        private static void Send(List<BuildingStatusState> batch)
        {
            Command.SendToClients(new BuildingStatusCommand { HostSession = _hostSession, Buildings = batch });
            _sentStates += batch.Count;
        }

        private static void Request(List<BuildingStatusRequest> batch)
        {
            Command.SendToServer(new RequestBuildingStatusCommand { Session = ClientCache.Session, Buildings = batch });
        }

        private static BuildingStatusState Capture(ushort id, ref Building building)
        {
            CitizenManager citizens = CitizenManager.instance;
            Citizen.BehaviourData behaviour = default(Citizen.BehaviourData);
            uint unit = building.m_citizenUnits;
            int visits = 0;
            while (unit != 0)
            {
                if (unit >= citizens.m_units.m_buffer.Length || visits++ >= citizens.m_units.m_buffer.Length)
                {
                    Log.Warn("Invalid citizen unit chain while reading building health: " + id);
                    return BuildingStatusState.Capture(id, ref building, null);
                }
                if ((citizens.m_units.m_buffer[unit].m_flags & CitizenUnit.Flags.Home) != 0)
                {
                    int alive = 0, total = 0;
                    citizens.m_units.m_buffer[unit].GetCitizenHomeBehaviour(ref behaviour, ref alive, ref total);
                }
                unit = citizens.m_units.m_buffer[unit].m_nextUnit;
            }
            return BuildingStatusState.Capture(id, ref building, new BuildingHomeHealth
            {
                Sick = behaviour.m_sickCount,
                TotalSick = behaviour.m_totalSickCount,
                SickChildren = behaviour.m_sickChildCount,
                SickSeniors = behaviour.m_sickSeniorCount,
                Dead = behaviour.m_deadCount
            });
        }
    }
}
