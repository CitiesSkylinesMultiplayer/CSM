using System;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Helpers
{
    // The simulation thread replaces entries; UI readers use complete snapshots.
    public class BuildingStatusCache
    {
        private class Entry
        {
            public uint LocalBuildIndex;
            public BuildingStatusState State;
            public ulong PendingToken;
            public bool Bound;
            public long LastRequest;
        }

        private Entry[] _entries = new Entry[ushort.MaxValue + 1];
        private ulong _nextToken;
        public int BoundCount { get; private set; }
        public int PendingCount { get; private set; }
        public string Session { get; private set; }
        public string HostSession { get; private set; }

        public BuildingStatusCache()
        {
            Reset();
        }

        public void Reset()
        {
            _entries = new Entry[ushort.MaxValue + 1];
            _nextToken = 0;
            BoundCount = PendingCount = 0;
            Session = Guid.NewGuid().ToString("N");
            HostSession = null;
        }

        public void Invalidate(ushort id)
        {
            Entry entry = _entries[id];
            if (entry != null)
            {
                if (entry.Bound)
                    BoundCount--;
                if (entry.PendingToken != 0)
                    PendingCount--;
            }
            _entries[id] = null;
        }

        public BuildingStatusState Get(ushort id, ref Building building)
        {
            Entry entry = _entries[id];
            return entry != null && entry.LocalBuildIndex == building.m_buildIndex && entry.State.MatchesGeometry(ref building)
                ? entry.State : null;
        }

        public BuildingStatusState Seed(ushort id, ref Building building)
        {
            BuildingStatusState state = Get(id, ref building);
            if (state != null || !BuildingStatusState.IsLive(ref building))
                return state;
            Invalidate(id);
            state = BuildingStatusState.Capture(id, ref building, null);
            _entries[id] = new Entry { LocalBuildIndex = building.m_buildIndex, State = state };
            return state;
        }

        public BuildingStatusRequest Request(ushort id, ref Building building, long now, long retryTicks)
        {
            if (id == 0 || Seed(id, ref building) == null)
                return null;
            Entry entry = _entries[id];
            if (entry.Bound || (entry.PendingToken != 0 && now - entry.LastRequest < retryTicks))
                return null;
            if (entry.PendingToken == 0)
                PendingCount++;
            ulong token = entry.PendingToken != 0 ? entry.PendingToken : ++_nextToken;
            _entries[id] = new Entry
            {
                LocalBuildIndex = entry.LocalBuildIndex,
                State = entry.State,
                PendingToken = token,
                LastRequest = now
            };
            return new BuildingStatusRequest
            {
                BuildingId = id,
                Token = token,
                InfoIndex = building.m_infoIndex,
                Position = building.m_position,
                Angle = building.m_angle,
                Width = building.m_width,
                Length = building.m_length
            };
        }

        public bool Accept(BuildingStatusState state, string requestSession, string hostSession, ref Building building)
        {
            if (state == null || !state.IsValid() || string.IsNullOrEmpty(hostSession) || !state.MatchesGeometry(ref building))
                return false;
            Entry entry = _entries[state.BuildingId];
            if (entry == null || entry.LocalBuildIndex != building.m_buildIndex || !entry.State.MatchesGeometry(ref building))
                return false;
            if (!string.IsNullOrEmpty(requestSession))
            {
                if (requestSession != Session || state.RequestToken == 0 || state.RequestToken != entry.PendingToken ||
                    (HostSession != null && HostSession != hostSession))
                    return false;
                HostSession = hostSession;
            }
            else if (!entry.Bound || HostSession != hostSession || entry.State.HostBuildIndex != state.HostBuildIndex || state.RequestToken != 0)
            {
                return false;
            }
            if (!entry.Bound)
                BoundCount++;
            if (entry.PendingToken != 0)
                PendingCount--;
            _entries[state.BuildingId] = new Entry { LocalBuildIndex = entry.LocalBuildIndex, State = state, Bound = true };
            return true;
        }
    }
}
