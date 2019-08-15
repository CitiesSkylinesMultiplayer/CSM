using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class ChangeNameHandler : CommandHandler<ChangeNameCommand>
    {
        public override void Handle(ChangeNameCommand command)
        {
            NameHandler.IgnoreAll = true;
            switch (command.Type)
            {
                case InstanceType.Building:
                    BuildingManager.instance.SetBuildingName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.Citizen:
                    CitizenManager.instance.SetCitizenName((uint) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.CitizenInstance:
                    CitizenManager.instance.SetInstanceName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.Disaster:
                    DisasterManager.instance.SetDisasterName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.District:
                    DistrictManager.instance.SetDistrictName(command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.Park:
                    DistrictManager.instance.SetParkName(command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.Event:
                    EventManager.instance.SetEventName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.NetSegment:
                    NetManager.instance.SetSegmentName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.TransportLine:
                    TransportManager.instance.SetLineName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.Vehicle:
                    VehicleManager.instance.SetVehicleName((ushort) command.Id, command.Name).MoveNext();
                    break;
                case InstanceType.ParkedVehicle:
                    VehicleManager.instance.SetParkedVehicleName((ushort) command.Id, command.Name).MoveNext();
                    break;
            }
            NameHandler.IgnoreAll = false;
        }
    }
}
