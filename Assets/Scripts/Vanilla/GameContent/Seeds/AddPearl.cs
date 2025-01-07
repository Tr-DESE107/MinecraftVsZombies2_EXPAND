using MVZ2.Vanilla;
using MVZ2Logic.SeedPacks;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [Definition(VanillaBlueprintNames.addPearl)]
    public class AddPearl : SeedOptionDefinition
    {
        public AddPearl(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Use(SeedPack seedPack)
        {
            base.Use(seedPack);
        }
    }
}
