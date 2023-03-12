using System;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(DistrictTool))]
    [HarmonyPatch("SimulationStep")]
    public class DistrictToolHandler {

        private static PlayerDistrictToolCommand _lastCommand;

        public static void Postfix(DistrictTool __instance, ToolController ___m_toolController, Vector3 ___m_mousePosition)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                PlayerDistrictToolCommand newCommand = new PlayerDistrictToolCommand
                {                    
                    Layer = (int) __instance.m_layer,
                    Mode = (int) __instance.m_mode,
                    ParkType = (int) __instance.m_parkType,
                    Specialization = (int) __instance.m_specialization,
                    BrushSize = __instance.m_brushSize,
                    CursorWorldPosition = ___m_mousePosition,
                    PlayerName = Chat.Instance.GetCurrentUsername()
                };
                if (!newCommand.Equals(_lastCommand)) {
                    _lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
            }
        }    
    }
    
    [ProtoContract]
    public class PlayerDistrictToolCommand : ToolCommandBase, IEquatable<PlayerDistrictToolCommand>
    {
        [ProtoMember(1)]
        public int Layer { get; set; }
        [ProtoMember(2)]
        public int Mode { get; set; }
        [ProtoMember(3)]
        public int ParkType { get; set; }
        [ProtoMember(4)]
        public int Specialization { get; set; }
        [ProtoMember(5)]
        public float BrushSize { get; set; }


        public bool Equals(PlayerDistrictToolCommand other)
        {
	        return base.Equals(other) &&
	               Equals(this.Layer, other.Layer) &&
	               Equals(this.Mode, other.Mode) &&
	               Equals(this.ParkType, other.ParkType) &&
	               Equals(this.Specialization, other.Specialization) &&
	               Equals(this.BrushSize, other.BrushSize);
        }
            
    }

    public class PlayerDistrictToolCommandHandler : BaseToolCommandHandler<PlayerDistrictToolCommand, DistrictTool>
    {
        protected override void Configure(DistrictTool tool, ToolController toolController, PlayerDistrictToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            tool.m_layer = (DistrictTool.Layer) command.Layer;
            tool.m_mode = (DistrictTool.Mode) command.Mode;
            tool.m_parkType = (DistrictPark.ParkType) command.ParkType;
            tool.m_specialization = (DistrictPolicies.Policies) command.Specialization;
            tool.m_brushSize = command.BrushSize;
            ReflectionHelper.SetAttr(tool, "m_mousePosition", command.CursorWorldPosition);

            if (tool.m_mode == DistrictTool.Mode.Select || tool.m_mode == DistrictTool.Mode.Specialize || tool.m_mode == DistrictTool.Mode.Unspecialize)
            {
	            toolController.SetBrush(null, Vector3.zero, 1f);
            }
            else
            {
	            toolController.SetBrush(tool.m_brush, command.CursorWorldPosition, command.BrushSize);
            }
        }

        protected override CursorInfo GetCursorInfo(DistrictTool tool)
        {
	        DistrictTool existing = ToolsModifierControl.toolController.GetComponent<DistrictTool>();
            if (tool.m_parkType != DistrictPark.ParkType.Airport || (tool.m_layer & DistrictTool.Layer.Districts) != 0)
            {
	            switch (tool.m_mode)
	            {
		            case DistrictTool.Mode.Specialize:
			            switch (tool.m_specialization)
			            {
				            case DistrictPolicies.Policies.Oil:
					            return existing.m_oilSpecializationCursor;
				            case DistrictPolicies.Policies.Ore:
					            return existing.m_oreSpecializationCursor;
				            case DistrictPolicies.Policies.Forest:
					            return existing.m_forestSpecializationCursor;
				            case DistrictPolicies.Policies.Farming:
					            return existing.m_farmingSpecializationCursor;
				            case DistrictPolicies.Policies.Leisure:
					            return existing.m_leisureSpecializationCursor;
				            case DistrictPolicies.Policies.Tourist:
					            return existing.m_touristSpecializationCursor;
				            case DistrictPolicies.Policies.Organic:
					            return existing.m_organicSpecializationCursor;
				            case DistrictPolicies.Policies.Selfsufficient:
					            return existing.m_selfsufficientSpecializationCursor;
				            case DistrictPolicies.Policies.Hightech:
					            return existing.m_hightechSpecializationCursor;
				            case DistrictPolicies.Policies.ResidentialWallToWall:
					            return existing.m_wallToWallResidentialSpecializationCursor;
				            case DistrictPolicies.Policies.CommercialWallToWall:
					            return existing.m_wallToWallCommercialSpecializationCursor;
				            case DistrictPolicies.Policies.OfficeWallToWall:
					            return existing.m_wallToWallOfficeSpecializationCursor;
				            case DistrictPolicies.Policies.Financial:
					            return existing.m_financialOfficeSpecializationCursor;
				            default:
					            return null;
			            }
		            case DistrictTool.Mode.Unspecialize:
			            switch (tool.m_specialization)
			            {
				            case DistrictPolicies.Policies.Forest:
				            case DistrictPolicies.Policies.Farming:
				            case DistrictPolicies.Policies.Oil:
				            case DistrictPolicies.Policies.Ore:
					            return existing.m_genericSpecializationCursor;
				            case DistrictPolicies.Policies.Leisure:
				            case DistrictPolicies.Policies.Tourist:
				            case DistrictPolicies.Policies.Organic:
					            return existing.m_genericCommercialSpecializationCursor;
				            case DistrictPolicies.Policies.Selfsufficient:
					            return existing.m_genericResidentialSpecializationCursor;
				            case DistrictPolicies.Policies.Hightech:
					            return existing.m_genericOfficeSpecializationCursor;
				            case DistrictPolicies.Policies.ResidentialWallToWall:
					            return existing.m_wallToWallResidentialSpecializationCursor;
				            case DistrictPolicies.Policies.CommercialWallToWall:
					            return existing.m_wallToWallCommercialSpecializationCursor;
				            case DistrictPolicies.Policies.OfficeWallToWall:
					            return existing.m_wallToWallOfficeSpecializationCursor;
				            default:
					            return null;
			            }
		            case DistrictTool.Mode.Paint:
			            return existing.m_paintCursor;
		            case DistrictTool.Mode.Erase:
			            return existing.m_eraseCursor;
		            default:
			            return null;
	            }
            }
			if (tool.m_mode == DistrictTool.Mode.Paint)
			{
				return existing.m_airportAreaCursor;
			}
			else
			{
				return existing.m_eraseCursor;
			}
        }
    }
}
