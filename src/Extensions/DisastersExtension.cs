using CSM.Commands;
using CSM.Injections;
using CSM.Networking;
using ICities;
using System;
using CSM.Helpers;

namespace CSM.Extensions
{
    public class DisastersExtension : IDisasterExtension
    {
        private IDisaster manager;

        public void OnCreated(IDisaster disaster)
        {
            // Do nothing
            this.manager = disaster;
        }

        public void OnReleased()
        {
            // Do nothing
        }

        public void OnDisasterCreated(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            DisasterSettings settings = manager.GetDisasterSettings(disasterID);

            Command.SendToAll(new DisasterCreateCommand
            {
                Id = disasterID,
                Type = settings.type,
                Name = settings.name,
                TargetX = settings.targetX,
                TargetY = settings.targetY,
                TargetZ = settings.targetZ,
                Angle = settings.angle,
                Intensity = settings.intensity,
                ClientId = MultiplayerManager.Instance.CurrentClient.ClientId
            });
        }

        public void OnDisasterStarted(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new DisasterStartCommand
            {
                Id = disasterID,
                ClientId = MultiplayerManager.Instance.CurrentClient.ClientId
            });
        }

        public void OnDisasterDetected(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;
        }

        public void OnDisasterActivated(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;
        }

        public void OnDisasterDeactivated(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;
        }

        public void OnDisasterFinished(ushort disasterID)
        {
            if (IgnoreHelper.IsIgnored())
                return;
        }
    }
}
