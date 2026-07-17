#nullable enable // 自动生成  

using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Enemies;
using MVZ2Logic;
using MVZ2Logic.Blueprints;
using MVZ2Logic.IZombie;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class IZombieEndlessBehaviour : IZombieEndlessBaseBehaviour
    {
        public IZombieEndlessBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override IEnumerable<IZELayoutItem> GetNormalLayouts()
        {
            // A阵容  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeComposite, 1.5f); // 混合型  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeControl, 1.5f); // 控制型  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeInstakill, 1f); // 秒杀型  

            // B阵容  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeSpikes, 0.2f); // 木刺类  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeDispensers, 0.2f); // 发射器类  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeExplosives, 0.2f); // 爆炸类  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeFire, 0.2f); // 火焰类  
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeAwards, 0.2f); // 恢复类  
        }
        protected override IEnumerable<IZELayoutItem> GetAwardLayouts()
        {
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeAwards);
            yield return new IZELayoutItem(VanillaIZombieLayoutID.izeError, 0.2f);
        }
        protected override NamespaceID GetFirstLayoutID()
        {
            return VanillaIZombieLayoutID.izeComposite;
        }

        public const int ROUNDS_PER_PICKAXE = 1;
        public const int MAX_PICKAXE_COUNT = 99;

        protected override void ReplaceBlueprints(LevelEngine level, IZombieLayoutDefinition layout)
        {
            // 获取已解锁的敌人（该缓存本身只包含 EntityTypes.ENEMY 的实体）  
            NamespaceID[] unlockedEnemies = Global.Saves.GetUnlockedEnemies();

            // 筛选出在图鉴中的敌人（排除障碍物等非图鉴条目）  
            var validEnemies = unlockedEnemies
                .Where(id => id != null && Global.Almanac.IsEnemyInAlmanac(id))
                .ToArray();

            // 没有有效敌人时，回退到默认蓝图列表  
            if (validEnemies.Length <= 0)
            {
                level.FillSeedPacks(GetBlueprints());
                return;
            }

            // 随机选取 10 个敌人（数量不足则取全部），转换为蓝图 ID  
            var selectedBlueprints = validEnemies
                .RandomTake(10, level.GetRoundRNG())
                .Select(id => LogicBlueprintID.FromEntity(id))
                .ToArray();

            level.FillSeedPacks(selectedBlueprints);
        }

        protected override IEnumerable<NamespaceID> GetBlueprints()
        {
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.imp);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.leatherCappedZombie);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.ghost);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.skeletonHorse);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.reflectiveBarrierZombie);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.gargoyle);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.ironHelmettedZombie);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.wickedHermitZombie);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.skeletonWarrior);
            yield return LogicBlueprintID.FromEntity(VanillaEnemyID.dullahan);
        }
    }
}
