using System.Collections.Generic;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingUpdateIndustryLastIndexHandler : CommandHandler<BuildingUpdateIndustryLastIndexCommand>
    {
        protected override void Handle(BuildingUpdateIndustryLastIndexCommand command)
        {
            Dictionary<uint,int> lastTableIndex = ReflectionHelper.GetAttr<Dictionary<uint, int>>(typeof(IndustryBuildingAI), "m_lastTableIndex");
            lastTableIndex[command.Key] = command.Value;
        }
    }
}
