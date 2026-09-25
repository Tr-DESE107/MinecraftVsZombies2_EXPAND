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
using PVZEngine.Damages;
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
  
        // 生成/复活：隐形矿车（无铁轨、null 父节点 -> 自由移动）+ 骑乘当前记录的骑手类型（默认超级狙击发射器）
        private void RespawnRider(LevelEngine level)  
        {  
            var cart = SpawnOrFindCart(level);  
            if (cart == null)  
                return;  
  
            var param = new SpawnParams();  
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());  
            var riderID = GetRiderExpanded(level) ? VanillaContraptionID.EXPANDispenser_Pistenser : VanillaContraptionID.MegaSnipenser;
            var rider = level.Spawn(riderID, cart.Position, cart, param);  
            rider?.AddBuff<DreamButterflyShieldBuff>();  
            if (rider != null)  
            {  
                rider.RideOn(cart);  
                SetRiderReference(level, new EntityID(rider));  
                // 融合体骑手重生时从关卡备份恢复通用射速等级（升级时由 HeavyWeaponUpgradeUtils 同步）
                if (rider.IsEntityOf(VanillaContraptionID.EXPANDispenser_Pistenser))
                {
                    HeavyWeaponUpgradeUtils.SetRapidLevel(rider, HeavyWeaponUpgradeUtils.GetLevelRapidBackup(level));
                }
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
  
        // “更换器械”蓝图入口（本关卡）：在超级狙击发射器与融合体 EXPANDispenser_Pistenser 之间自由切换，继承射速等级。
        // 先生成新骑手并更新引用，最后旧骑手退场，避免行为 Update 检测到“骑手死亡”而扣除命数；
        // 死亡回调按 PROP_RIDER_REFERENCE 判定，旧骑手退场不会触发红石返还与爆炸。
        public static bool SwapRider(LevelEngine level)
        {
            var reference = GetRiderReference(level);
            var oldRider = reference?.GetEntity(level);
            if (oldRider == null || !oldRider.ExistsAndAlive())
                return false;

            var cart = MinecartRideable.FindSingleCart(level);
            if (cart == null)
                return false;

            var oldIsExpand = oldRider.IsEntityOf(VanillaContraptionID.EXPANDispenser_Pistenser);
            if (!oldIsExpand && !oldRider.IsEntityOf(VanillaContraptionID.MegaSnipenser))
                return false;
            var newRiderID = oldIsExpand ? VanillaContraptionID.MegaSnipenser : VanillaContraptionID.EXPANDispenser_Pistenser;

            // 射速等级双向继承：MegaSnipenser 用自身等级存储，融合体用通用等级存储（同步关卡备份）
            var rapidLevel = oldIsExpand ? HeavyWeaponUpgradeUtils.GetLevelRapidBackup(level) : MegaSnipenser.GetRapidLevel(oldRider);

            var param = new SpawnParams();
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());
            var newRider = level.Spawn(newRiderID, cart.Position, cart, param);
            if (newRider == null || !newRider.ExistsAndAlive())
                return false;
            newRider.RideOn(cart);
            SetRiderReference(level, new EntityID(newRider));
            SetRiderExpanded(level, newRider.IsEntityOf(VanillaContraptionID.EXPANDispenser_Pistenser));
            newRider.AddBuff<DreamButterflyShieldBuff>();

            var newIsExpand = newRider.IsEntityOf(VanillaContraptionID.EXPANDispenser_Pistenser);
            if (newIsExpand)
            {
                HeavyWeaponUpgradeUtils.SetRapidLevel(newRider, rapidLevel);
            }
            else
            {
                MegaSnipenser.SetRapidLevel(newRider, rapidLevel);
            }
            HeavyWeaponUpgradeUtils.SetLevelRapidBackup(level, rapidLevel);
            newRider.PlaySound(VanillaSoundID.gunReload);
            newRider.PlaySound(VanillaSoundID.powerUp);

            // 旧骑手退场（引用已指向新骑手，不影响命数）
            oldRider.Die(new DamageEffectList());
            return true;
        }

        // 当前骑手是否为融合体（重生时按此类型生成）
        public static bool GetRiderExpanded(LevelEngine level) => level.GetProperty<bool>(PROP_RIDER_EXPANDED);
        public static void SetRiderExpanded(LevelEngine level, bool value) => level.SetProperty(PROP_RIDER_EXPANDED, value);

        public const int LIVES = 3;  
  
        private const string PROP_REGION = "heavy_weapon_shoot_balloon_stage";  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_RIDER_REFERENCE =  
            new VanillaLevelPropertyMeta<EntityID>("rider_reference");  
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> PROP_RIDER_EXPANDED =
            new VanillaLevelPropertyMeta<bool>("rider_expanded");
    }  
}