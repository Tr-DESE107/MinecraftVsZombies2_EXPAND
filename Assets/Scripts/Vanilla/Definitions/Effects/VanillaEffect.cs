using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEffect : VanillaEntity
    {
        protected VanillaEffect(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinEntityProps.SHADOW_HIDDEN, true);
        }
        public override int Type => EntityTypes.EFFECT;
    }
}
