using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    public abstract class BlueprintHeldItemDefinition : HeldItemDefinition, IBlueprintHeldItemDefinition
    {
        public BlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new PickupHeldItemBehaviour(this));
            AddBehaviour(new TriggerCartHeldItemBehaviour(this));
            AddBehaviour(new SelectBlueprintHeldItemBehaviour(this));
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var seed = GetSeedPack(level, data);
            var seedDef = seed?.Definition;
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
        public abstract SeedPack GetSeedPack(LevelEngine level, IHeldItemData data);
    }
}
