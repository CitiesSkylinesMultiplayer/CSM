using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Economy;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Economy
{
    public class EconomyResourceHandler : CommandHandler<EconomyResourceCommand>
    {
        protected override void Handle(EconomyResourceCommand command)
        {
            if (command.Action == ResourceAction.ADD)
            {
                AddResource.DontRunPatch = true;

                EconomyManager.instance.AddResource(command.ResourceType, command.ResourceAmount, command.Service,
                    command.SubService, command.Level, command.Taxation);

                AddResource.DontRunPatch = false;
            }
            else if (command.Action == ResourceAction.FETCH)
            {
                FetchResource.DontRunPatch = true;

                EconomyManager.instance.FetchResource(command.ResourceType, command.ResourceAmount, command.Service,
                    command.SubService, command.Level);

                FetchResource.DontRunPatch = false;
            }
            else if (command.Action == ResourceAction.PRIVATE)
            {
                AddPrivateIncome.DontRunPatch = true;

                EconomyManager.instance.AddPrivateIncome(command.ResourceAmount, command.Service, command.SubService,
                    command.Level, command.TaxRate);

                AddPrivateIncome.DontRunPatch = false;
            }
        }
    }
}
