using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Difficulties;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Implements
{
    public class DifficultyImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostLevelStart, PostLevelStartCallback);
        }
        public void PostLevelStartCallback(LevelEngine level)
        {
            var difficulty = level.Difficulty;
            bool easy = difficulty == VanillaDifficulties.easy;
            var easyDef = level.ContentProvider.GetBuffDefinition(VanillaBuffID.Level.levelEasy);
            if (easy != level.HasBuff(easyDef))
            {
                if (easy)
                    level.AddBuff(easyDef);
                else
                    level.RemoveBuffs(easyDef);
            }
            bool hard = difficulty == VanillaDifficulties.hard;
            var hardDef = level.ContentProvider.GetBuffDefinition(VanillaBuffID.Level.levelHard);
            if (hard != level.HasBuff(hardDef))
            {
                if (hard)
                    level.AddBuff(hardDef);
                else
                    level.RemoveBuffs(hardDef);
            }
        }
    }
}
