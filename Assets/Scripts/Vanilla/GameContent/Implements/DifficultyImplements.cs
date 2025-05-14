using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Implements
{
    public class DifficultyImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LogicLevelCallbacks.PRE_BATTLE, PreBattleCallback);
            mod.AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
        }
        public void PreBattleCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            EvaluateDifficultyBuff(level);
        }
        public void PostLevelStartCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            EvaluateDifficultyBuff(level);
        }
        private void EvaluateDifficultyBuff(LevelEngine level)
        {
            var difficulty = level.Difficulty;
            var difficultyMeta = Global.Game.GetDifficultyMeta(difficulty);
            BuffDefinition buffDef = null;
            if (difficultyMeta != null)
            {
                var buffId = difficultyMeta.BuffID;
                if (level.IsIZombie())
                {
                    buffId = difficultyMeta.IZombieBuffID;
                }
                buffDef = level.Content.GetBuffDefinition(buffId);
            }


            var buffRef = GetDifficultyBuff(level);
            if (buffRef != null)
            {
                var buff = buffRef.GetBuff(level);
                if (buff.Definition == buffDef)
                {
                    return;
                }
                level.RemoveBuff(buff);
            }
            if (buffDef != null)
            {
                var buff = level.AddBuff(buffDef);
                SetDifficultyBuff(level, level.GetBuffReference(buff));
            }
        }
        public static BuffReference GetDifficultyBuff(LevelEngine level)
        {
            return level.GetProperty<BuffReference>(PROP_DIFFICULTY_BUFF);
        }
        public static void SetDifficultyBuff(LevelEngine level, BuffReference value)
        {
            level.SetProperty(PROP_DIFFICULTY_BUFF, value);
        }
        private const string PROP_REGION = "difficulty";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta PROP_DIFFICULTY_BUFF = new VanillaLevelPropertyMeta("DifficultyBuff");
    }
}
