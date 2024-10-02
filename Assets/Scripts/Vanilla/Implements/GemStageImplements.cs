using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Games;
using MVZ2.Modding;
using PVZEngine;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Vanilla
{
    public class GemStageImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostEntityInit, Gem_PostPickupInitCallback, filter: EntityTypes.PICKUP);
            mod.RegisterCallback(LevelCallbacks.PostLevelUpdate, Gem_PostUpdateCallback);
            mod.RegisterCallback(LevelCallbacks.PostEntityDeath, Gem_PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
        }
        private void Gem_PostUpdateCallback(LevelEngine level)
        {
            var firstGem = level.GetProperty<EntityID>(VanillaLevelProps.FIRST_GEM);
            if (firstGem == null)
                return;
            var gem = firstGem.GetEntity(level);
            if (gem == null || !gem.Exists() || gem.IsCollected())
            {
                var adviceContext = CONTEXT_ADVICE_COLLECT_MONEY;
                var adviceText = ADVICE_COLLECT_MONEY_1;
                level.ShowAdvice(adviceContext, adviceText, 1000, 90);
                level.SetProperty(VanillaLevelProps.FIRST_GEM, null);
                level.HideHintArrow();
            }
        }
        private void Gem_PostPickupInitCallback(Entity pickup)
        {
            if (pickup.Definition is not Gem gem)
                return;
            var level = pickup.Level;
            if (!level.HasBehaviour<GemStageBehaviour>())
                return;
            Game game = Global.Game;
            if (!game.IsUnlocked(VanillaUnlockID.money))
            {
                game.Unlock(VanillaUnlockID.money);
                level.SetHintArrowPointToEntity(pickup);
                level.SetProperty(VanillaLevelProps.FIRST_GEM, new EntityID(pickup));
                var adviceContext = CONTEXT_ADVICE_COLLECT_MONEY;
                var adviceText = ADVICE_COLLECT_MONEY_0;
                level.ShowAdvice(adviceContext, adviceText, 1000, -1);
            }
        }
        private void Gem_PostEnemyDeathCallback(Entity enemy, DamageInfo info)
        {
            var level = enemy.Level;
            if (!level.HasBehaviour<GemStageBehaviour>())
                return;
            Game game = Global.Game;
            bool spawnGem = false;
            if (game.IsUnlocked(VanillaUnlockID.money))
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
                var weights = gemWeights.Select(g => g.weight).ToArray();
                var index = enemy.DropRNG.WeightedRandom(weights);
                var gemID = gemWeights[index].id;
                enemy.Produce(gemID);
            }
        }
        public const string CONTEXT_ADVICE_COLLECT_MONEY = "advice.collect_money";
        [TranslateMsg("拾取宝石的帮助提示", CONTEXT_ADVICE_COLLECT_MONEY)]
        public const string ADVICE_COLLECT_MONEY_0 = "点击收集宝石！";
        [TranslateMsg("拾取宝石的帮助提示", CONTEXT_ADVICE_COLLECT_MONEY)]
        public const string ADVICE_COLLECT_MONEY_1 = "收集宝石来获得更酷的道具吧！";
        private static readonly List<(NamespaceID id, float weight)> gemWeights = new List<(NamespaceID, float)>()
        {
            ( VanillaPickupID.emerald, 100 ),
            ( VanillaPickupID.ruby, 20 ),
            ( VanillaPickupID.diamond, 1 ),
        };
    }
}
