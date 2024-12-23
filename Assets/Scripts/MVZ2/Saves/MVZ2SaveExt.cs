using System.Linq;
using MVZ2.Metas;
using MVZ2Logic.Games;

namespace MVZ2.Saves
{
    public static class MVZ2SaveExt
    {
        public static bool MeetsXMLConditions(this IGameSaveData save, XMLConditionList conditions)
        {
            if (conditions == null)
                return false;
            return conditions.Conditions.Any(c => save.MeetsXMLCondition(c));
        }
        public static bool MeetsXMLCondition(this IGameSaveData save, XMLCondition condition)
        {
            if (condition == null)
                return false;
            return condition.Required.All(c => save.IsUnlocked(c)) && condition.RequiredNot.All(c => !save.IsUnlocked(c));
        }
    }
}
