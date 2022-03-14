using System;
using ColossalFramework.UI;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Names;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Names
{
    public class ChangeNameHandler : CommandHandler<ChangeNameCommand>
    {
        protected override void Handle(ChangeNameCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            // The type where the text box is located (can be a base class of the actual panel)
            Type renameType = null;
            // List of types where the text box is potentially located (length needs to be equal to length of panelTypes). Leave empty to use renameType.
            Type[] renameTypes = new Type[0];
            // List of panels which potentially can be open. Leave empty when equal to renameType.
            Type[] panelTypes = new Type[0];
            // The instance type the currently opened panel needs to correspond to
            InstanceType instanceType = command.Type;
            // The instance id the currently opened panel needs to correspond to
            int id = command.Id;
            // The name of the text box field
            string nameFieldVar = "m_NameField";
            // List of names of the text box field (length needs to be equal to length of panelTypes). Leave empty to use nameFieldVar.
            string[] nameFieldVarArr = new string[0];

            switch (command.Type)
            {
                case InstanceType.Building:
                    BuildingManager.instance.SetBuildingName((ushort)command.Id, command.Name).MoveNext();

                    renameType = typeof(BuildingWorldInfoPanel);
                    panelTypes = new Type[]
                    {
                        typeof(CityServiceWorldInfoPanel), typeof(ChirpXPanel), typeof(FestivalPanel),
                        typeof(FootballPanel), typeof(VarsitySportsArenaPanel), typeof(ShelterWorldInfoPanel),
                        typeof(UniqueFactoryWorldInfoPanel), typeof(WarehouseWorldInfoPanel),
                        typeof(ZonedBuildingWorldInfoPanel)
                    };
                    break;

                case InstanceType.Citizen:
                case InstanceType.CitizenInstance:

                    if (command.Type == InstanceType.Citizen)
                        CitizenManager.instance.SetCitizenName((uint)command.Id, command.Name).MoveNext();
                    else
                        CitizenManager.instance.SetInstanceName((ushort)command.Id, command.Name).MoveNext();

                    renameType = typeof(LivingCreatureWorldInfoPanel);
                    panelTypes = new Type[]
                    {
                        typeof(AnimalWorldInfoPanel), typeof(CitizenWorldInfoPanel), typeof(TouristWorldInfoPanel),
                        typeof(ServicePersonWorldInfoPanel)
                    };
                    break;

                case InstanceType.Disaster:
                    DisasterManager.instance.SetDisasterName((ushort)command.Id, command.Name).MoveNext();

                    if (!HandleSpecialMeteorRename((ushort)command.Id, command.Name))
                    {
                        renameType = typeof(DisasterWorldInfoPanel);
                        nameFieldVar = "m_nameField";
                    }

                    break;

                case InstanceType.District:
                    DistrictManager.instance.SetDistrictName(command.Id, command.Name).MoveNext();

                    renameType = typeof(DistrictWorldInfoPanel);
                    nameFieldVar = "m_DistrictName";
                    break;

                case InstanceType.Park:
                    DistrictManager.instance.SetParkName(command.Id, command.Name).MoveNext();

                    panelTypes = new Type[]
                    {
                        typeof(IndustryWorldInfoPanel), typeof(CampusWorldInfoPanel), typeof(ParkWorldInfoPanel)
                    };

                    nameFieldVarArr = new string[]
                    {
                        "m_ParkName", "m_campusName", "m_ParkName"
                    };
                    break;

                case InstanceType.Event:
                    EventManager.instance.SetEventName((ushort)command.Id, command.Name).MoveNext();

                    renameType = typeof(ChirpXPanel);
                    nameFieldVar = "m_rocketName";
                    break;

                case InstanceType.NetSegment:
                    NetManager.instance.SetSegmentName((ushort)command.Id, command.Name).MoveNext();

                    renameType = typeof(RoadWorldInfoPanel);
                    break;

                case InstanceType.TransportLine:
                    TransportManager.instance.SetLineName((ushort)command.Id, command.Name).MoveNext();

                    // Works fine :)
                    break;

                case InstanceType.Vehicle:
                case InstanceType.ParkedVehicle:

                    if (command.Type == InstanceType.Vehicle)
                        VehicleManager.instance.SetVehicleName((ushort)command.Id, command.Name).MoveNext();
                    else
                        VehicleManager.instance.SetParkedVehicleName((ushort)command.Id, command.Name).MoveNext();

                    renameTypes = new Type[]
                    {
                        typeof(VehicleWorldInfoPanel), typeof(VehicleWorldInfoPanel),
                        typeof(VehicleWorldInfoPanel), typeof(VehicleWorldInfoPanel),
                        typeof(MeteorWorldInfoPanel)
                    };

                    panelTypes = new Type[]
                    {
                        typeof(CitizenVehicleWorldInfoPanel), typeof(CityServiceWorldInfoPanel),
                        typeof(PublicTransportVehicleWorldInfoPanel), typeof(TouristVehicleWorldInfoPanel),
                        typeof(MeteorWorldInfoPanel)
                    };
                    break;

                default:
                    Log.Warn("Unknown instance type in ChangeNameHandler received!");
                    break;
            }

            if (panelTypes.Length == 0 && renameType != null)
            {
                panelTypes = new Type[] { renameType };
            }

            int i = -1;
            foreach (Type panel in panelTypes)
            {
                i++;
                InstanceID instanceId = InfoPanelHelper.GetInstanceID(panel, out WorldInfoPanel infoPanel);

                if (instanceId.Type != instanceType && !(instanceId.Type == InstanceType.Building && panel == typeof(ChirpXPanel)))
                    continue;

                bool isEqual = false;
                switch (instanceType)
                {
                    case InstanceType.Building:
                        isEqual = instanceId.Building == (ushort)id;
                        break;

                    case InstanceType.Citizen:
                        isEqual = instanceId.Citizen == (uint)id;
                        break;

                    case InstanceType.CitizenInstance:
                        isEqual = instanceId.CitizenInstance == (ushort)id;
                        break;

                    case InstanceType.Disaster:
                        isEqual = instanceId.Disaster == (ushort)id;
                        break;

                    case InstanceType.District:
                        isEqual = instanceId.District == (byte)id;
                        break;

                    case InstanceType.Park:
                        isEqual = instanceId.Park == (byte)id;
                        break;

                    case InstanceType.Event:
                        ushort curEventId = ReflectionHelper.GetAttr<ushort>(infoPanel, "m_currentEventID");
                        isEqual = curEventId == (ushort)id;
                        break;

                    case InstanceType.NetSegment:
                        isEqual = instanceId.NetSegment == (ushort)id;
                        break;

                    case InstanceType.Vehicle:
                        isEqual = instanceId.Vehicle == (ushort)id;
                        break;

                    case InstanceType.ParkedVehicle:
                        isEqual = instanceId.ParkedVehicle == (ushort)id;
                        break;
                }

                if (!isEqual)
                    continue;

                Type renameVarType = panel;
                if (renameTypes.Length > i)
                    renameVarType = renameTypes[i];
                else if (renameType != null)
                    renameVarType = renameType;

                string varName = nameFieldVar;
                if (nameFieldVarArr.Length > i)
                    varName = nameFieldVarArr[i];

                SetNameField(renameVarType, infoPanel, varName, command.Name);
                break;
            }

            IgnoreHelper.Instance.EndIgnore();
        }

        private bool HandleSpecialMeteorRename(ushort disaster, string newName)
        {
            InstanceID instanceId = InfoPanelHelper.GetInstanceID(typeof(MeteorWorldInfoPanel), out WorldInfoPanel panel);

            InstanceManager.Group group = InstanceManager.instance.GetGroup(instanceId);
            if (group == null)
                return false;

            ushort disasterId = group.m_ownerInstance.Disaster;
            if (disasterId != disaster)
                return false;

            SetNameField(panel.GetType(), panel, "m_NameField", newName);
            return true;
        }

        private void SetNameField(Type nameType, WorldInfoPanel panel, string nameFieldVar, string newName)
        {
            UITextField nameField = nameType.GetField(nameFieldVar, ReflectionHelper.AllAccessFlags)?.GetValue(panel) as UITextField;

            if (nameField == null)
                return;

            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                nameField.text = newName;
            });
        }
    }
}
