using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.diamond)]
    public class Diamond : Gem
    {
        public Diamond(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.PlaySound(VanillaSoundID.chime);
        }
        protected override bool GetRandomCollectPitch()
        {
            return false;
        }
    }
}