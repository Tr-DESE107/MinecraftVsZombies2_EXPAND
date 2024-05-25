using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEffect : VanillaEntity
    {
        protected VanillaEffect(string nsp, string name) : base(nsp, name)
        {
        }

        public override int Type => EntityTypes.EFFECT;
    }
}
