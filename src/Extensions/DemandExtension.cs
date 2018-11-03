using ColossalFramework;
using CSM.Commands;
using CSM.Networking;
using ICities;

namespace CSM.Extensions
{
    /// <summary>
    ///    This lets the serverside handle demand by setting the actual demand on the clients to 0, it also sync the demand shown on the UI by sending the servers UIdata to the Client.
    /// </summary>
    public class DemandExtension : DemandExtensionBase
    {
        public override int OnCalculateCommercialDemand(int originalDemand)
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    {
                        Singleton<ZoneManager>.instance.m_actualCommercialDemand = 0;
                        break;
                    }
            }
            return base.OnCalculateCommercialDemand(originalDemand);
        }

        public override int OnCalculateResidentialDemand(int originalDemand)
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    {
                        Singleton<ZoneManager>.instance.m_actualResidentialDemand = 0;
                        break;
                    }

                case MultiplayerRole.Server: //Sends the UI data of the different types of Demand
                    {
                        var ResidentialDemand = Singleton<ZoneManager>.instance.m_residentialDemand;
                        var CommercialDemand = Singleton<ZoneManager>.instance.m_commercialDemand;
                        var WorkplaceDemant = Singleton<ZoneManager>.instance.m_workplaceDemand;

                        Command.SendToClients(new DemandDisplayedCommand
                        {
                            ResidentialDemand = ResidentialDemand,
                            CommercialDemand = CommercialDemand,
                            WorkplaceDemand = WorkplaceDemant
                        });
                        break;
                    }
            }
            return base.OnCalculateResidentialDemand(originalDemand);
        }

        public override int OnCalculateWorkplaceDemand(int originalDemand)
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    {
                        Singleton<ZoneManager>.instance.m_actualWorkplaceDemand = 0;
                        break;
                    }
            }
            return base.OnCalculateWorkplaceDemand(originalDemand);
        }
    }
}