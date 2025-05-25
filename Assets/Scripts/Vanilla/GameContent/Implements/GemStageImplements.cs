using MukioI18n;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Implements
{
    public class GemStageImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, Gem_PostPickupInitCallback, filter: EntityTypes.PICKUP);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, Gem_PostEnemyInitCallback, filter: EntityTypes.ENEMY);
            mod.AddTrigger(LevelCallbacks.POST_LEVEL_UPDATE, Gem_PostUpdateCallback);
        }
        private void Gem_PostUpdateCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            var firstGem = level.GetProperty<EntityID>(FIRST_GEM);
            if (firstGem == null)
                return;
            var gem = firstGem.GetEntity(level);
            if (gem == null || !gem.Exists() || gem.IsCollected())
            {
                var adviceContext = CONTEXT_ADVICE_COLLECT_MONEY;
                var adviceText = ADVICE_COLLECT_MONEY_1;
                level.ShowAdvice(adviceContext, adviceText, 1000, 90);
                level.SetProperty(FIRST_GEM, null);
                level.HideHintArrow();
            }
        }
        private void Gem_PostPickupInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var pickup = param.entity;
            var gem = pickup.Definition.GetBehaviour<Gem>();
            if (gem == null)
                return;
            var level = pickup.Level;
            if (!level.HasBehaviour<GemStageBehaviour>())
                return;
            if (!Global.Game.IsUnlocked(VanillaUnlockID.money))
            {
                Global.Game.Unlock(VanillaUnlockID.money);
                level.SetHintArrowPointToEntity(pickup);
                level.SetProperty(FIRST_GEM, new EntityID(pickup));
                var adviceContext = CONTEXT_ADVICE_COLLECT_MONEY;
                var adviceText = ADVICE_COLLECT_MONEY_0;
                level.ShowAdvice(adviceContext, adviceText, 1000, -1);
            }
        }
        private void Gem_PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var enemy = param.entity;
            var level = enemy.Level;
            if (!level.HasBehaviour<GemStageBehaviour>())
                return;
            bool spawnGem = false;
            if (Global.Game.IsUnlocked(VanillaUnlockID.money))
            {
                spawnGem = enemy.DropRNG.Next(10) < 1;
            }
            else
            {
                var currentWave = level.CurrentWave;
                var totalFlags = level.GetTotalFlags();
                if (currentWave >= totalFlags * level.GetWavesPerFlag() / 2)
                {
                    spawnGem = true;
                }
            }
            if (spawnGem)
            {
                enemy.AddBuff<GemCarrierBuff>();
            }
        }
        public const string REGION_NAME = "gem_stage";
        [LevelPropertyRegistry(REGION_NAME)]
        public static readonly VanillaLevelPropertyMeta FIRST_GEM = new VanillaLevelPropertyMeta("firstGem");
        public const string CONTEXT_ADVICE_COLLECT_MONEY = "advice.collect_money";
        [TranslateMsg("拾取宝石的帮助提示", CONTEXT_ADVICE_COLLECT_MONEY)]
        public const string ADVICE_COLLECT_MONEY_0 = "点击收集宝石！";
        [TranslateMsg("拾取宝石的帮助提示", CONTEXT_ADVICE_COLLECT_MONEY)]
        public const string ADVICE_COLLECT_MONEY_1 = "收集宝石来获得更酷的道具吧！";
    }
}
