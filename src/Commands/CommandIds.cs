namespace CSM.Commands
{
    /// <summary>
    ///     This class contains a list of command IDs used in this mod,
    ///     please create a new ID here when you create a new command to ensure
    ///     no overlap. Ids range from 0-255
    /// </summary>
    public class CommandIds
    {
        // 0 - 9: Connection handling packets
        public const byte ConnectionRequestCommand = 0;
        public const byte ConnectionResultCommand = 1;

        // 10 - 29: Non-game commands
        public const byte ClientConnectCommand = 10;
        public const byte ClientDisconnectCommand = 11;

        public const byte PlayerListCommand = 12;
        public const byte ChatMessageCommand = 13;

        public const byte FinishTransactionCommand = 14;

        // 30 - 39: Game management commands
        public const byte WorldInfoCommand = 30;
        public const byte SpeedCommand = 31;
        public const byte PauseCommand = 32;
        public const byte DemandDisplayedCommand = 33;

        // 40 - 44: Money and tax commands
        public const byte MoneyCommand = 40;
        public const byte TaxRateChangeCommand = 41;
        public const byte BudgetChangeCommand = 42;

        // 45 - 49: Building commands
        public const byte BuildingCreateCommand = 45;
        public const byte BuildingRemoveCommand = 46;
        public const byte BuildingRelocateCommand = 47;

        // 50 - 59: Net commands
        public const byte NodeCreateCommand = 50;
        public const byte NodeReleaseCommand = 51;
        public const byte NodeUpdateCommand = 52;
        public const byte SegmentCreateCommand = 53;
        public const byte SegmentReleaseCommand = 54;

        // 60 - 64: Zone and area commands
        public const byte ZoneUpdateCommand = 60;
        public const byte UnlockAreaCommand = 61;

        // 65 - 69: Tree commands
        public const byte TreeCreateCommand = 65;
        public const byte TreeMoveCommand = 66;
        public const byte TreeReleaseCommand = 67;

        // 70 - 79: District commands
        public const byte DistrictCreateCommand = 70;
        public const byte DistrictReleaseCommand = 71;
        public const byte DistrictAreaModifyCommand = 72;
        public const byte DistrictPolicyCommand = 73;
        public const byte DistrictPolicyUnsetCommand = 74;
        public const byte DistrictCityPolicyCommand = 75;
        public const byte DistrictCityPolicyUnsetCommand = 76;

        // 80 - 84: Terrain commands
        public const byte TerrainModificationCommand = 80;

        // 85 - 89: Prop commands
        public const byte PropCreateCommand = 85;
        public const byte PropReleaseCommand = 86;
        public const byte PropMoveCommand = 87;

        // 90 - 94: Park commands
        public const byte ParkCreateCommand = 90;
        public const byte ParkReleaseCommand = 91;

    }
}
