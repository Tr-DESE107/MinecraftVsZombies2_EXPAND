using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    [Definition(VanillaPickupNames.diamond)]
    public class Diamond : Gem
    {
        public Diamond(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.PlaySound(SoundID.chime);
        }
        protected override bool GetRandomCollectPitch()
        {
            return false;
        }
    }
}