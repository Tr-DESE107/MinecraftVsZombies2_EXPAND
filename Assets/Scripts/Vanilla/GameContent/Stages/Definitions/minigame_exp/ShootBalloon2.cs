#nullable enable  
  
using MVZ2.GameContent.Buffs;  
using MVZ2.GameContent.Buffs.Enemies;   // FlyBuff, BoatBuff  
using MVZ2.GameContent.Enemies;  
using MVZ2.GameContent.Seeds;            // VanillaBlueprintID  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Enemies;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Buffs;  
using PVZEngine.Callbacks;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using MVZ2.GameContent.Effects;   // VanillaEffectID.fallenEndShip  
using UnityEngine;  
using MVZ2.GameContent.Areas;     // VanillaAreaID  
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.ShootBalloon2)]  
    public partial class ShootBalloon2 : StageDefinition  
    {  
        public ShootBalloon2(string nsp, string name) : base(nsp, name)  
        {  
            AddBehaviour(new WaveStageBehaviour(this));  
            AddBehaviour(new FinalWaveClearBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
            // 复用射气球 1 的行为：铁轨 + 矿车 + 活塞发射器  
            AddBehaviour(new ShootBalloonStageBehaviour(this));  
  
            // 敌人初始化后挂气球 + 移除船 buff（只处理敌方怪物）  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);  
        }  
  
        // 复刻红龙砸船  
        public override void OnSetup(LevelEngine level)  
        {  
            base.OnSetup(level);  
  
            // 逻辑层：右侧列铺破船格（保留左两列 0、1）  
            for (int x = 2; x < level.GetMaxColumnCount(); x++)  
            {  
                for (int z = 0; z < level.GetMaxLaneCount(); z++)  
                {  
                    var grid = level.GetGrid(x, z);  
                    if (grid == null)  
                        continue;  
                    grid.RemoveBuffs(VanillaBuffID.Grid.goldenGrid);  
                    grid.AddBuff(VanillaBuffID.Grid.shipBrokenGrid);  
                }  
            }  
  
            // 视觉层：断裂船模型 + 掉落的半船（仅 ship 区域有意义）  
            if (level.AreaID == VanillaAreaID.ship)  
            {  
                var areaModel = level.GetAreaModelInterface();  
                areaModel?.SetModelProperty("Broken", true);  
                level.Spawn(VanillaEffectID.fallenEndShip,  
                    new Vector3(LevelPositions.LEVEL_WIDTH, 0, 0), null);  
            }  
        }  
  
        // 跳过选卡：直接铺满重装兵器蓝图  
        public override void OnStart(LevelEngine level)  
        {  
            base.OnStart(level);  
            var blueprints = new NamespaceID[]  
            {  
                VanillaBlueprintID.heavyWeaponFlashbang,  
                VanillaBlueprintID.HeavyWeaponGrenade,  
                VanillaBlueprintID.HeavyWeaponMolotov,  
                VanillaBlueprintID.HeavyWeaponRegen,  
                VanillaBlueprintID.HeavyWeaponExtraLife,  
                VanillaBlueprintID.HeavyWeaponSelfDestruct,  
            };  
            level.SetSeedSlotCount(blueprints.Length);  
            level.FillSeedPacks(blueprints);  
        }  
  
        private void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)  
        {  
            var entity = param.entity;  
            var level = entity.Level;  
  
            if (level.StageDefinition != this)  
                return;  
            if (entity.IsEntityOf(VanillaEnemyID.lockedChestBalloon))  
                return;  
            if (entity.Definition.IsFlyingEnemy())  
                return;  
            if (entity.HasBuff(VanillaBuffID.Entity.draggedByBalloon))  
                return;  
  
            AttachBalloon(entity);  
  
            // 关键：去掉自动挂上的船 buff，这样气球没了、在空路会掉下去  
            entity.RemoveBuffs<BoatBuff>();  
            entity.SetModelProperty("HasBoat", false);  
  
            // 头颅类怪物失去飞行效果：抹掉它们 Init 时挂的 FlyBuff  
            if (IsSkullEnemy(entity))  
            {  
                entity.RemoveBuffs<FlyBuff>();  
            }  
        }  
  
        private static readonly NamespaceID[] SKULL_ENEMIES = new NamespaceID[]  
        {  
            VanillaEnemyID.SkeletonHead,  
            VanillaEnemyID.ZombieHead,  
            VanillaEnemyID.dullahanHead,  
            VanillaEnemyID.BerserkerHead,  
            VanillaEnemyID.RaiderSkull,  
        };  
  
        private static bool IsSkullEnemy(Entity entity)  
        {  
            foreach (var id in SKULL_ENEMIES)  
            {  
                if (entity.IsEntityOf(id))  
                    return true;  
            }  
            return false;  
        }  
  
        private const float BALLOON_TARGET_HEIGHT = 90f;  
  
        private void AttachBalloon(Entity enemy)  
        {  
            var level = enemy.Level;  
            level.Spawn(VanillaEnemyID.lockedChestBalloon, enemy.GetCenter(), null)?.Let(balloon =>  
            {  
                balloon.SetParent(enemy);  
                balloon.PlaySound(VanillaSoundID.balloonInflate);  
                foreach (var fly in balloon.GetBuffs<FlyBuff>())  
                {  
                    fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, BALLOON_TARGET_HEIGHT);  
                }  
            });  
        }  
    }  
}