using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.GameContent.Implements
{
    public class BlueprintImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, PostUseEntityBlueprintCallback);
        }
        private void PostUseEntityBlueprintCallback(Entity entity, SeedDefinition seedDef, SeedPack seed, IHeldItemData heldData)
        {
            if (entity == null)
                return;
            if (heldData.InstantTrigger && entity.CanTrigger())
            {
                entity.Trigger();
            }
            if (heldData.InstantEvoke && entity.CanEvoke() && entity.Level.GetStarshardCount() > 0 && !entity.Level.IsStarshardDisabled())
            {
                entity.Level.AddStarshardCount(-1);
                entity.Evoke();
            }
            if (seed != null)
            {
                var drawnFromPool = seed.GetDrawnConveyorSeed();
                if (NamespaceID.IsValid(drawnFromPool))
                {
                    entity.AddTakenConveyorSeed(drawnFromPool);
                }
            }
        }
    }
}
