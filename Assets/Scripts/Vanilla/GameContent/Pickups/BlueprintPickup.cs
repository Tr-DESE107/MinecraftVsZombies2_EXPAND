using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.blueprintPickup)]
    public class BlueprintPickup : PickupBehaviour
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
            level.SetHeldItem(VanillaHeldTypes.blueprintPickup, pickup.ID, 0);
            pickup.PlaySound(pickup.GetCollectSound());
        }
        public static SeedDefinition GetSeedDefinition(Entity pickup)
        {
            if (!pickup.IsBlueprintPickup())
                return null;
            var seedID = GetBlueprintID(pickup);
            var seedDef = pickup.Level.Content.GetSeedDefinition(seedID);
            if (seedDef == null)
                return null;
            return seedDef;
        }
        public static NamespaceID GetSeedEntityID(Entity pickup)
        {
            var seedDef = GetSeedDefinition(pickup);
            if (seedDef == null)
                return null;
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return null;
            return seedDef.GetSeedEntityID();
        }
        private static void UpdateModel(Entity pickup)
        {
            var level = pickup.Level;
            bool isHolding = level.GetHeldItemType() == VanillaHeldTypes.blueprintPickup && level.GetHeldItemID() == pickup.ID;
            pickup.SetModelProperty("BlueprintID", GetBlueprintID(pickup));
            pickup.SetModelProperty("CommandBlock", IsCommandBlock(pickup));
            pickup.SetAnimationBool("HideEnergy", true);
            pickup.SetAnimationBool("Dark", pickup.Timeout < 100 && pickup.Timeout % 20 < 10 && !isHolding);
            pickup.SetAnimationBool("Selected", isHolding);
        }

        public static NamespaceID GetBlueprintID(Entity pickup) => pickup.GetBehaviourField<NamespaceID>(PROP_BLUEPRINT_ID);
        public static void SetBlueprintID(Entity pickup, NamespaceID value) => pickup.SetBehaviourField(PROP_BLUEPRINT_ID, value);
        public static bool IsCommandBlock(Entity pickup) => pickup.GetBehaviourField<bool>(PROP_COMMAND_BLOCK);
        public static void SetCommandBlock(Entity pickup, bool value) => pickup.SetBehaviourField(PROP_COMMAND_BLOCK, value);
        public static bool IgnoresTouchRaycast(Entity pickup) => pickup.GetBehaviourField<bool>(PROP_IGNORE_TOUCH_RAYCAST);
        public static void SetIgnoreTouchRaycast(Entity pickup, bool value) => pickup.SetBehaviourField(PROP_IGNORE_TOUCH_RAYCAST, value);
        public const int PICK_THRESOLD = 5;
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_BLUEPRINT_ID = new VanillaEntityPropertyMeta<NamespaceID>("BlueprintID");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_COMMAND_BLOCK = new VanillaEntityPropertyMeta<bool>("CommandBlock");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_IGNORE_TOUCH_RAYCAST = new VanillaEntityPropertyMeta<bool>("IgnoreRaycast");
    }
}