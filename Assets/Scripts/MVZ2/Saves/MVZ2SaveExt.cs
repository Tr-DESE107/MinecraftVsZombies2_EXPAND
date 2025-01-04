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
            if (condition.Required != null && condition.Required.Any(c => !save.IsUnlocked(c)))
                return false;
            if (condition.RequiredNot != null && condition.RequiredNot.Any(c => save.IsUnlocked(c)))
                return false;
            return true;
        }
    }
}
