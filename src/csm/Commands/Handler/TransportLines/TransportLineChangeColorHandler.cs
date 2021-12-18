using CSM.API.Commands;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;

namespace CSM.Commands.Handler.TransportLines
{
    public class TransportLineChangeColorHandler : CommandHandler<TransportLineChangeColorCommand>
    {
        protected override void Handle(TransportLineChangeColorCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            TransportManager.instance.SetLineColor(command.LineId, command.Color).MoveNext();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
