using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using MVZ2.UI;
using PVZEngine;

namespace MVZ2
{
    public static class BlueprintHelper
    {
        public static BlueprintViewData[] GetBlueprintsViewData(MainManager main, IEnumerable<NamespaceID> blueprints)
        {
            return blueprints.Select(id =>
            {
                if (!NamespaceID.IsValid(id))
                    return BlueprintViewData.Empty;
                var blueprintDef = main.Game.GetSeedDefinition(id);
                return main.ResourceManager.GetBlueprintViewData(blueprintDef);
            }).ToArray();
        }
    }
}
