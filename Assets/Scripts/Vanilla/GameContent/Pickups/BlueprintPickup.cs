using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.blueprintPickup)]
    public class BlueprintPickup : PickupBehaviour, IHeldEntityBehaviour
    {
        public BlueprintPickup(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            UpdateModel(entity);
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            UpdateModel(pickup);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            var level = pickup.Level;
            level.SetHeldItem(VanillaHeldTypes.entity, pickup.ID, 0);
            pickup.PlaySound(pickup.GetCollectSound());
        }
        private static SeedDefinition GetSeedDefinition(Entity pickup)
        {
            var seedID = GetBlueprintID(pickup);
            var seedDef = pickup.Level.Content.GetSeedDefinition(seedID);
            if (seedDef == null)
                return null;
            return seedDef;
        }
        private static void UpdateModel(Entity pickup)
        {
            pickup.SetModelProperty("BlueprintID", GetBlueprintID(pickup));
            pickup.SetModelProperty("CommandBlock", IsCommandBlock(pickup));
            pickup.SetAnimationBool("HideEnergy", true);
            pickup.SetAnimationBool("Dark", (pickup.Timeout < 100 && pickup.Timeout % 20 < 10) || pickup.Level.IsHoldingEntity(pickup));
        }
        #region 接口实现
        bool IHeldEntityBehaviour.IsValidFor(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetGrid;
        }

        HeldHighlight IHeldEntityBehaviour.GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seedDef = GetSeedDefinition(entity);
            return grid.GetSeedHeldHighlight(seedDef);
        }

        void IHeldEntityBehaviour.Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    {
                        var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
                        if (interaction != targetPhase)
                            return;

                        var seedDef = GetSeedDefinition(entity);
                        if (seedDef == null)
                            return;
                        var grid = gridTarget.Target;
                        var level = grid.Level;
                        if (!grid.CanPlaceBlueprint(seedDef.GetID(), out var error))
                        {
                            var message = Global.Game.GetGridErrorMessage(error);
                            if (!string.IsNullOrEmpty(message))
                            {
                                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, message, 0, 150);
                            }
                            return;
                        }

                        if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                        {
                            var commandBlock = IsCommandBlock(entity);
                            entity.Remove();
                            grid.UseEntityBlueprintDefinition(seedDef, data, commandBlock);
                            level.ResetHeldItem();
                        }
                    }
                    break;
                case HeldItemTargetLawn lawnTarget:
                    {
                        var level = lawnTarget.Level;
                        var area = lawnTarget.Area;

                        if (area == LawnArea.Side)
                        {
                            if (level.CancelHeldItem())
                            {
                                level.PlaySound(VanillaSoundID.tap);
                            }
                        }
                    }
                    break;
            }
        }
        NamespaceID IHeldEntityBehaviour.GetModelID(Entity entity, LevelEngine level, IHeldItemData data)
        {
            var seedDef = GetSeedDefinition(entity);
            if (seedDef == null)
                return null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                return entityDef.GetModelID();
            }
            return null;
        }
        void IHeldEntityBehaviour.PostSetModel(Entity entity, LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            if (entity == null)
                return;
            if (IsCommandBlock(entity))
            {
                model.SetShaderInt("_Grayscale", 1);
            }
        }

        float IHeldEntityBehaviour.GetRadius(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return 0;
        }

        void IHeldEntityBehaviour.Update(Entity entity, LevelEngine level, IHeldItemData data)
        {
            if (entity == null || !entity.Exists())
            {
                level.ResetHeldItem();
            }
        }
        #endregion
        public static NamespaceID GetBlueprintID(Entity pickup) => pickup.GetBehaviourField<NamespaceID>(PROP_BLUEPRINT_ID);
        public static void SetBlueprintID(Entity pickup, NamespaceID value) => pickup.SetBehaviourField(PROP_BLUEPRINT_ID, value);
        public static bool IsCommandBlock(Entity pickup) => pickup.GetBehaviourField<bool>(PROP_COMMAND_BLOCK);
        public static void SetCommandBlock(Entity pickup, bool value) => pickup.SetBehaviourField(PROP_COMMAND_BLOCK, value);
        public static readonly VanillaEntityPropertyMeta PROP_BLUEPRINT_ID = new VanillaEntityPropertyMeta("BlueprintID");
        public static readonly VanillaEntityPropertyMeta PROP_COMMAND_BLOCK = new VanillaEntityPropertyMeta("CommandBlock");
    }
}