using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2Logic;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    [HeldItemDefinition(BuiltinHeldItemNames.blueprint)]
    public class ClassicBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ClassicBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new ClassicBlueprintHeldItemBehaviour(this));
        }
        public override SeedPack GetSeedPack(LevelEngine level, IHeldItemData data)
        {
            return level.GetSeedPackAt((int)data.ID);
        }
    }
}
