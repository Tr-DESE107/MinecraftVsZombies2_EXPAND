#nullable enable  
  
using MVZ2.GameContent.Buffs;  
using MVZ2.GameContent.Buffs.Enemies;  
using MVZ2.GameContent.Contraptions;  
using MVZ2.GameContent.Enemies;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Enemies;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using PVZEngine.Buffs;  
using PVZEngine.Callbacks;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using MVZ2.GameContent.Seeds;   // VanillaBlueprintID  
using UnityEngine;  
using PVZEngine;  
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.ShootBalloon)]  
    public partial class ShootBalloon : StageDefinition  
    {  
        public ShootBalloon(string nsp, string name) : base(nsp, name)  
        {  
            AddBehaviour(new WaveStageBehaviour(this));  
            AddBehaviour(new FinalWaveClearBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
            // 重装兵器：铁轨 + 矿车 + 活塞发射器（见 ShootBalloonStageBehaviour）  
            AddBehaviour(new ShootBalloonStageBehaviour(this));  
  
            // 怪物初始化时挂气球，只处理敌方怪物（同 BalloonParty）  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);  
        }  
  
        // 关卡开始铺设场地：后三列钻石地刺、中三列普通地刺、前三列无地刺、第二列一列木风扇  
        public override void OnSetup(LevelEngine level)  
        {  
            base.OnSetup(level);  
            SetupField(level);  
        }  
  
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
  
        private void SetupField(LevelEngine level)  
        {  
            int columns = level.GetMaxColumnCount(); // 通常 9 列（0..8）  
            int lanes = level.GetMaxLaneCount();     // 通常 5 行（0..4）  
  
            for (int col = 0; col < columns; col++)  
            {  
                for (int lane = 0; lane < lanes; lane++)  
                {  
                    float x = level.GetEntityColumnX(col);  
                    float z = level.GetEntityLaneZ(lane);  
                    float y = level.GetGroundY(x, z);  
                    var pos = new Vector3(x, y, z);  
  
                    if (col >= 6)  
                    {  
                        // 后三列：钻石地刺  
                        level.Spawn(VanillaContraptionID.diamondSpikes, pos, null);  
                    }  
                    else if (col >= 3)  
                    {  
                        // 中三列：普通地刺  
                        level.Spawn(VanillaContraptionID.spikeBlock, pos, null);  
                    }  
                    else  
                    {  
                        // 前三列（0,1,2）：无地刺；仅第二列（col==1）放一列木风扇  
                        if (col == 1)  
                        {  
                            level.Spawn(VanillaContraptionID.woodenFan, pos, null);  
                        }  
                        if (col == 2)  
                        {  
                            level.Spawn(VanillaContraptionID.obsidian, pos, null);  
                        }  
                    }  
                }  
            }  
        }  
  
        // ===== 出场挂气球（复刻 BalloonParty）=====  
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
  
        private const float BALLOON_TARGET_HEIGHT = 120f;  
  
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