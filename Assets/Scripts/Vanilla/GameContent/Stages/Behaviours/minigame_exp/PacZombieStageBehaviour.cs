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
using MVZ2.Vanilla.Pickups;
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

namespace MVZ2.GameContent.Stages
{
    public class PacZombieStageBehaviour : StageBehaviour
    {
        public PacZombieStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            // 借用星之碎片 UI 当命数显示：禁用星之碎片本身功能 + 换成命数图标  
            AddModifier(new NamespaceIDModifier(LogicLevelProps.STARSHARD_DISABLE_ID, SetOperator.Set, VanillaBlueprintErrors.locked));
            AddModifier(new SpriteReferenceModifier(LogicAreaProps.STARSHARD_ICON, SetOperator.Set, VanillaSprites.snipenserLife));
            stageDef.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostContraptionDeathCallback, filter: EntityTypes.PLANT);
        }

        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetPickaxeActive(false);
            level.SetTriggerActive(false);

            // 命数：初始 LIVES 条命  
            level.SetStarshardCount(LIVES);
            level.SetStarshardSlotCount(LIVES);

            RespawnDevourer(level);
        }

        public override void Update(LevelEngine level)
        {
            base.Update(level);
            if (level.CurrentWave >= 1)
            {
                var reference = GetDevourerReference(level);
                var devourer = reference?.GetEntity(level);
                if (!devourer.ExistsAndAlive())
                {
                    if (level.GetStarshardCount() > 0)
                    {
                        RespawnDevourer(level);
                        level.AddStarshardCount(-1);
                    }
                    else
                    {
                        level.GameOver(GameOverTypes.NO_ENEMY, null, VanillaStrings.DEATH_MESSAGE_SNIPENSER_LOST);
                    }
                }
            }
        }

        private void RespawnDevourer(LevelEngine level)
        {
            var pos = new Vector3(level.GetEntityColumnX(2), 0, level.GetLaneZ(2));

            // 隐形矿车：已有就复用，没有再新建（复活时避免重复生成矿车）  
            var cart = SpawnOrFindMinecart(level, pos);
            if (cart == null)
                return;

            // 吃怪模式吞噬者，骑上矿车  
            var devParams = new SpawnParams();
            devParams.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());
            devParams.SetProperty(Devourer.PROP_ENDLESS_GHOST, true);
            var devourer = level.Spawn(VanillaContraptionID.devourer, cart.Position, cart, devParams);
            if (devourer == null)
                return;
            devourer.RideOn(cart);
            SetDevourerReference(level, new EntityID(devourer));
        }

        private Entity? SpawnOrFindMinecart(LevelEngine level, Vector3 pos)
        {
            // 需求1+3：优先复用已有矿车，并清除多余/残留矿车，保证唯一  
            var cart = MinecartRideable.FindSingleCart(level);
            if (cart != null)
                return cart;
            cart = level.Spawn(VanillaEffectID.minecartRideable, pos, null);
            if (cart != null)
                MinecartRideable.SetInvisible(cart);   // tint alpha=0 隐形  
            return cart;
        }

        private void PostContraptionDeathCallback(LevelCallbacks.EntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (level.HasBehaviour(this))
            {
                var snipenserReference = GetDevourerReference(level);
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

        public static EntityID? GetDevourerReference(LevelEngine level) => level.GetProperty<EntityID>(PROP_DEVOURER_REFERENCE);
        public static void SetDevourerReference(LevelEngine level, EntityID? value) => level.SetProperty(PROP_DEVOURER_REFERENCE, value);

        public const int LIVES = 3;
        private const string PROP_REGION = "pac_zombie_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_DEVOURER_REFERENCE = new VanillaLevelPropertyMeta<EntityID>("devourer_reference");
    }
}
