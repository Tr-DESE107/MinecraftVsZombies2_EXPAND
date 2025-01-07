using MVZ2.HeldItems;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Grids;
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
    }
}
