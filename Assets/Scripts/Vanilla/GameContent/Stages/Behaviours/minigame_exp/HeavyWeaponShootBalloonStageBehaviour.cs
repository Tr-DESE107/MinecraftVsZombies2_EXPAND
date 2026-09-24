#nullable enable  
  
using System;  
using MVZ2.GameContent.Buffs.Contraptions;  
using MVZ2.GameContent.Contraptions;  
using MVZ2.GameContent.Effects;  
using MVZ2.GameContent.Pickups;  
using MVZ2.GameContent.Seeds;  
using MVZ2.GameContent.Sprites;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Localization;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic.Blueprints;  
using MVZ2Logic.Contents.Enemies;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using MVZ2Logic.Modifiers;  
using PVZEngine;  
using PVZEngine.Buffs;  
using PVZEngine.Callbacks;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using PVZEngine.Modifiers;  
using UnityEngine;  
using MVZ2.Vanilla.Pickups;
  
namespace MVZ2.GameContent.Stages  
{  
    public class HeavyWeaponShootBalloonStageBehaviour : StageBehaviour  
    {  
        public HeavyWeaponShootBalloonStageBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
            // 借用星之碎片 UI 当命数显示  
            AddModifier(new NamespaceIDModifier(LogicLevelProps.STARSHARD_DISABLE_ID, SetOperator.Set, VanillaBlueprintErrors.locked));  
            AddModifier(new SpriteReferenceModifier(LogicAreaProps.STARSHARD_ICON, SetOperator.Set, VanillaSprites.snipenserLife));  
            stageDef.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostContraptionDeathCallback, filter: EntityTypes.PLANT);  
        }  
  
        public override void Start(LevelEngine level)  
        {  
            base.Start(level);  
            level.SetPickaxeActive(false);  
            level.SetTriggerActive(false);  
  
            level.SetStarshardCount(LIVES);  
            level.SetStarshardSlotCount(LIVES);  
  
            RespawnRider(level);  
        }  
  
        public override void Update(LevelEngine level)  
        {  
            base.Update(level);  
            if (!level.IsGameRunning())  
                return;  
  
            var riderRef = GetRiderReference(level);  
            var rider = riderRef?.GetEntity(level);  
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
        }  
  
        // 生成/复活：隐形矿车（无铁轨、null 父节点 -> 自由移动）+ 骑乘超级狙击发射器  
        private void RespawnRider(LevelEngine level)  
        {  
            var cart = SpawnOrFindCart(level);  
            if (cart == null)  
                return;  
  
            var param = new SpawnParams();  
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());  
            var rider = level.Spawn(VanillaContraptionID.MegaSnipenser, cart.Position, cart, param);  
            rider?.AddBuff<DreamButterflyShieldBuff>();  
            if (rider != null)  
            {  
                rider.RideOn(cart);  
                SetRiderReference(level, new EntityID(rider));  
            }  
        }  
  
        // 矿车死了通常还在；没有才新建，避免复活时多出一辆  
        private Entity? SpawnOrFindCart(LevelEngine level)  
        {  
            var cart = MinecartRideable.FindSingleCart(level);  
            if (cart != null)  
                return cart;  
            var pos = new Vector3(level.GetEntityColumnX(2), 0, level.GetLaneZ(2));  
            cart = level.Spawn(VanillaEffectID.minecartRideable, pos, null);  
            if (cart != null)  
                MinecartRideable.SetInvisible(cart);  
            return cart;  
        }  
  
        private void PostContraptionDeathCallback(LevelCallbacks.EntityDeathParams param, CallbackResult result)  
        {  
            var entity = param.entity;  
            var level = entity.Level;  
            if (level.HasBehaviour(this))  
            {  
                var snipenserReference = GetRiderReference(level);  
                if (snipenserReference != null && snipenserReference.IsEntity(entity))  
                {  
                    var rapidUpgrade = MegaSnipenser.GetRapidLevel(entity);  
                    var spreadUpgrade = MegaSnipenser.GetSpreadLevel(entity);  
                    var rapidCost = level.Content.GetSeedDefinition(VanillaBlueprintID.heavyWeaponRapid)?.GetCost() ?? 0;  
                    var spreadCost = level.Content.GetSeedDefinition(VanillaBlueprintID.heavyWeaponSpread)?.GetCost() ?? 0;  
                    var rapidRedStones = Mathf.Max(0, rapidCost) / 50;  
                    var spreadRedstones = Mathf.Max(0, spreadCost - 25) / 50;  
                    var totalRedstones = rapidUpgrade * rapidRedStones + spreadUpgrade * spreadRedstones;  
                    if (entity.GetFirstBuff<HeavyWeaponSelfDestructBuff>() != null)  
                    {  
                        totalRedstones *= 0.5f;  
                    }  
  
                    for (int i = 0; i < totalRedstones; i++)  
                    {  
                        entity.Produce(VanillaPickupID.redstone);  
                    }  
  
                    Explosion.Spawn(entity, entity.GetCenter(), 120);  
                    entity.PlaySound(VanillaSoundID.largeExplosion);  
                }  
            }  
        }  
  
        public static EntityID? GetRiderReference(LevelEngine level) => level.GetProperty<EntityID>(PROP_RIDER_REFERENCE);  
        public static void SetRiderReference(LevelEngine level, EntityID? value) => level.SetProperty(PROP_RIDER_REFERENCE, value);  
  
        public const int LIVES = 3;  
  
        private const string PROP_REGION = "heavy_weapon_shoot_balloon_stage";  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_RIDER_REFERENCE =  
            new VanillaLevelPropertyMeta<EntityID>("rider_reference");  
    }  
}