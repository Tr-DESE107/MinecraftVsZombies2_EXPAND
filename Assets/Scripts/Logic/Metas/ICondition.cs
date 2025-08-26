using MVZ2Logic.Games;

namespace MVZ2Logic.Conditions
{
    public interface IConditionList
    {
        bool MeetsConditions(IGlobalSaveData save);
    }
    public interface ICondition
    {
        bool MeetsCondition(IGlobalSaveData save);
    }
}
