using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;

namespace CSM.BaseGame.Commands.Handler.TransportLines
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
