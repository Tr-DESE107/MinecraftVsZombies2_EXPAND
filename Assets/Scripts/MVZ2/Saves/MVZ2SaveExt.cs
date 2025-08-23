using MVZ2Logic.Conditions;
using MVZ2Logic.Games;

namespace MVZ2.Saves
{
    public static class MVZ2SaveExt
    {
        public static bool IsNullOrMeetsConditions(this IConditionList conditions, IGameSaveData save)
        {
            return conditions == null || conditions.MeetsConditions(save);
        }
        public static bool MeetsXMLConditions(this IGameSaveData save, IConditionList conditions)
        {
            if (conditions == null)
                return false;
            return conditions.MeetsConditions(save);
        }
        public static bool MeetsXMLCondition(this IGameSaveData save, ICondition condition)
        {
            if (condition == null)
                return false;
            return condition.MeetsCondition(save);
        }
    }
}
