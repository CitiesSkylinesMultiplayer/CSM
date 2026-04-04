using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Disasters
{
    [ProtoContract]
    public class EvacuationCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort BuildingID { get; set; }

        [ProtoMember(2)]
        public bool Evacuating { get; set; }
    }
}
