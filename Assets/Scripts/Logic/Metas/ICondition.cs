using MVZ2Logic.Games;

namespace MVZ2Logic.Conditions
{
    public interface IConditionList
    {
        bool MeetsConditions(IGameSaveData save);
    }
    public interface ICondition
    {
        bool MeetsCondition(IGameSaveData save);
    }
}
