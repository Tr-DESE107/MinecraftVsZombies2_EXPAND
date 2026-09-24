#nullable enable  
  
using System;  
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;  
using MVZ2.Vanilla.Localization;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Damages;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using UnityEngine;  
using MVZ2Logic.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using MVZ2Logic.Contents.Enemies;
using MVZ2.GameContent.Sprites;  
using MVZ2Logic.Modifiers;  
using PVZEngine.Modifiers;  
using MVZ2.GameContent.Seeds;
  
namespace MVZ2.GameContent.Stages  
{  
    // 参考 HeavyWeaponStageBehaviour：生成铁轨 + 矿车，让活塞发射器坐在矿车上。  
    public class ShootBalloonStageBehaviour : StageBehaviour  
    {  
        public ShootBalloonStageBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
            // 借用星之碎片 UI 当命数显示：禁用其原功能 + 换成命数图标  
            AddModifier(new NamespaceIDModifier(LogicLevelProps.STARSHARD_DISABLE_ID, SetOperator.Set, VanillaBlueprintErrors.locked));  
            AddModifier(new SpriteReferenceModifier(LogicAreaProps.STARSHARD_ICON, SetOperator.Set, VanillaSprites.snipenserLife));  
  
            stageDef.AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreRiderTakeDamageCallback, filter: EntityTypes.PLANT);  
        }  
  
        public override void Start(LevelEngine level)  
        {  
            base.Start(level);  
            level.SetPickaxeActive(false);  
            level.SetTriggerActive(false);  
  
            // 命数  
            level.SetStarshardCount(LIVES);  
            level.SetStarshardSlotCount(LIVES);  
  
            RespawnRider(level);  
        }  
  
        public override void Update(LevelEngine level)  
        {  
            base.Update(level);  
            if (level.CurrentWave >= 1)  
            {  
                var reference = GetRiderReference(level);  
                var rider = reference?.GetEntity(level);  
                if (!rider.ExistsAndAlive())  
                {  
                    if (level.GetStarshardCount() > 0)  
                    {  
                        RespawnRider(level);  
                        level.AddStarshardCount(-1);  
                    }  
                    else  
                    {  
                        level.GameOver(GameOverTypes.NO_ENEMY, null, VanillaStrings.DEATH_MESSAGE_SNIPENSER_LOST);  
                    }  
                }  
                else if (!rider.IsFriendlyEntity())  
                {  
                    var damageEffects = new DamageEffectList(VanillaDamageEffects.INSTA_KILL);  
                    rider.Die(damageEffects);  
                }  
            }  
        }  
  
        private void RespawnRider(LevelEngine level)
        {
            SpawnOrFindRail(level)?.Let(rail =>
            {
                SpawnOrFindMinecart(level, rail)?.Let(cart =>
                {
                    cart.SetParent(rail);
                    SpawnOrFindRider(level, cart)?.Let(rider =>
                    {
                        rider.RideOn(cart);
                        SetRiderReference(level, new EntityID(rider));
                        rider.AddBuff<DreamButterflyShieldBuff>();
                        // 从关卡备份恢复通用射速等级（升级时由 HeavyWeaponUpgradeUtils 同步）
                        HeavyWeaponUpgradeUtils.SetRapidLevel(rider, HeavyWeaponUpgradeUtils.GetLevelRapidBackup(level));
                    });
                });
            });
        }  
  
        private Entity? SpawnOrFindRail(LevelEngine level)  
        {  
            var rail = level.FindFirstEntity(VanillaEffectID.minecartRail);  
            if (rail.ExistsAndAlive())  
                return rail;  
            // 铁轨放在第 0 列、第 2 行（中间行），可按需调整  
            return level.Spawn(VanillaEffectID.minecartRail,  
                new Vector3(level.GetEntityColumnX(0), 0, level.GetLaneZ(2)), null);  
        }  
  
        private Entity? SpawnOrFindMinecart(LevelEngine level, Entity rail)  
        {  
            var minecart = MinecartRideable.FindSingleCart(level);  
            if (minecart != null)  
                return minecart;  
            return level.Spawn(VanillaEffectID.minecartRideable, rail.Position, rail);  
        }  
  
        private Entity? SpawnOrFindRider(LevelEngine level, Entity cart)
        {
            // 已更换器械：复用或生成融合体 EXPANDispenser_Pistenser
            if (GetSwapped(level))
            {
                var swapped = level.FindFirstEntity(VanillaContraptionID.EXPANDispenser_Pistenser);
                if (swapped.ExistsAndAlive())
                    return swapped;
                return SpawnRider(level, cart, VanillaContraptionID.EXPANDispenser_Pistenser);
            }

            // 活塞发射器：如已有就复用
            var rider = level.FindFirstEntity(VanillaContraptionID.pistenser);
            if (rider.ExistsAndAlive())
                return rider;
            return SpawnRider(level, cart, VanillaContraptionID.pistenser);
        }

        private static Entity? SpawnRider(LevelEngine level, Entity cart, NamespaceID contraptionID)
        {
            var param = new SpawnParams();
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());
            param.SetProperty(LogicEntityProps.HP_BAR_VISIBILITY, HPBarVisibility.FORCE);
            return level.Spawn(contraptionID, cart.Position, cart, param);
        }

        // “更换器械”蓝图入口：把骑手替换为融合体 EXPANDispenser_Pistenser，并继承射速等级（子弹数量不继承）。
        // 此后骑手被摧毁重生时也会生成融合体（见 SpawnOrFindRider）。
        public static void ReplaceRider(LevelEngine level)
        {
            var reference = GetRiderReference(level);
            var oldRider = reference?.GetEntity(level);
            if (oldRider == null || !oldRider.ExistsAndAlive())
                return;

            var cart = MinecartRideable.FindSingleCart(level);
            if (cart == null)
                return;

            int rapidLevel = HeavyWeaponUpgradeUtils.GetLevelRapidBackup(level);

            // 先标记为已更换并生成新骑手，最后才让旧骑手消失，
            // 避免行为 Update 检测到“骑手死亡”而扣除命数。
            SetSwapped(level, true);

            var newRider = SpawnRider(level, cart, VanillaContraptionID.EXPANDispenser_Pistenser);
            if (newRider == null || !newRider.ExistsAndAlive())
                return;
            newRider.RideOn(cart);
            SetRiderReference(level, new EntityID(newRider));
            newRider.AddBuff<DreamButterflyShieldBuff>();
            // 继承射速等级（子弹数量不继承，融合体本身弹数就多）
            HeavyWeaponUpgradeUtils.SetRapidLevel(newRider, rapidLevel);
            HeavyWeaponUpgradeUtils.SetLevelRapidBackup(level, rapidLevel);
            newRider.PlaySound(VanillaSoundID.gunReload);
            newRider.PlaySound(VanillaSoundID.powerUp);

            // 旧骑手退场（引用已指向新骑手，不影响命数）
            oldRider.Die(new DamageEffectList());
        }  
  
        private void PreRiderTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)  
        {  
            var input = param.input;  
            var entity = input.Entity;  
            var level = entity.Level;  
            if (level.HasBehaviour(this) && level.IsGodMode())  
            {  
                var reference = GetRiderReference(level);  
                if (reference != null && reference.IsEntity(entity))  
                {  
                    result.SetFinalValue(false);  
                }  
            }  
        }  
  
        public static EntityID? GetRiderReference(LevelEngine level) => level.GetProperty<EntityID>(PROP_RIDER_REFERENCE);
        public static void SetRiderReference(LevelEngine level, EntityID? value) => level.SetProperty(PROP_RIDER_REFERENCE, value);

        // 是否已通过“更换器械”蓝图把骑手更换为融合体
        public static bool GetSwapped(LevelEngine level) => level.GetProperty<bool>(PROP_SWAPPED);
        public static void SetSwapped(LevelEngine level, bool value) => level.SetProperty(PROP_SWAPPED, value);

        public const int LIVES = 3;

        private const string PROP_REGION = "shoot_balloon_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_RIDER_REFERENCE =
            new VanillaLevelPropertyMeta<EntityID>("rider_reference");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> PROP_SWAPPED =
            new VanillaLevelPropertyMeta<bool>("swapped");  
    }  
}
