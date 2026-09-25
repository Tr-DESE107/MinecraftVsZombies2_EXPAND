#nullable enable  
  
using MVZ2.GameContent.Buffs;  
using MVZ2.GameContent.Buffs.Enemies;  
using MVZ2.GameContent.Enemies;  
using MVZ2.GameContent.Seeds;  
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
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.HeavyWeaponShootBalloon)]  
    public partial class HeavyWeaponShootBalloon : StageDefinition  
    {  
        public HeavyWeaponShootBalloon(string nsp, string name) : base(nsp, name)  
        {  
            AddBehaviour(new WaveStageBehaviour(this));  
            AddBehaviour(new FinalWaveClearBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
            AddBehaviour(new SpeedUpStageBehaviour(this, 1.5f, 2f));  
            // 隐形矿车 + 超级狙击发射器（自由移动射击）  
            AddBehaviour(new HeavyWeaponShootBalloonStageBehaviour(this));  
  
            // 敌人初始化时挂气球，只处理敌方怪物（复刻 BalloonParty）  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);  
        }  
  
        public override void OnStart(LevelEngine level)  
        {  
            base.OnStart(level);  
            var blueprints = new NamespaceID[]  
            {  
                VanillaBlueprintID.heavyWeaponFlashbang,  
                VanillaBlueprintID.HeavyWeaponGrenade,  
                VanillaBlueprintID.HeavyWeaponMolotov,  
                VanillaBlueprintID.HeavyWeaponEnderPearl,  
                VanillaBlueprintID.HeavyWeaponInvincible,  
                VanillaBlueprintID.heavyWeaponRapid,  
                VanillaBlueprintID.HeavyWeaponRegen,  
                VanillaBlueprintID.HeavyWeaponContraptionSwap,  
                VanillaBlueprintID.HeavyWeaponSelfDestruct,  
            };  
            level.SetSeedSlotCount(blueprints.Length);  
            level.FillSeedPacks(blueprints);  
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
        }  
  
        private const float BALLOON_TARGET_HEIGHT = 50f;  
  
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