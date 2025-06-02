using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2Logic.SeedPacks
{
    public abstract class SeedOptionDefinition : Definition
    {
        public SeedOptionDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void Use(SeedPack seedPack)
        {

        }
        public virtual void Use(LevelEngine level, SeedDefinition seedPack)
        {

        }
        public virtual void Update(SeedPack seedPack, float rechargeSpeed) { }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.SEED_OPTION;
    }
}
