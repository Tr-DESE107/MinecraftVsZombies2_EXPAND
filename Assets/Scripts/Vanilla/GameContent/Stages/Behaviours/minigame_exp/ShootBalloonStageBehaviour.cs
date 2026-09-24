#nullable enable  
  
using System;  
using MVZ2.GameContent.Buffs.Contraptions;  
using MVZ2.GameContent.Contraptions;  
using MVZ2.GameContent.Damages;  
using MVZ2.GameContent.Effects;  
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
            // 活塞发射器：如已有就复用  
            var rider = level.FindFirstEntity(VanillaContraptionID.pistenser);  
            if (rider.ExistsAndAlive())  
                return rider;  
  
            var param = new SpawnParams();  
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());  
            param.SetProperty(LogicEntityProps.HP_BAR_VISIBILITY, HPBarVisibility.FORCE);  
            return level.Spawn(VanillaContraptionID.pistenser, cart.Position, cart, param);  
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
  
        public const int LIVES = 3;  
  
        private const string PROP_REGION = "shoot_balloon_stage";  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_RIDER_REFERENCE =  
            new VanillaLevelPropertyMeta<EntityID>("rider_reference");  
    }  
}