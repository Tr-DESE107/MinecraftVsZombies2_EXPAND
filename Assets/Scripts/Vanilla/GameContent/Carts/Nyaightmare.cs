using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Carts
{
    [EntityBehaviourDefinition(VanillaCartNames.nyaightmare)]
    public class Nyaightmare : CartBehaviour
    {
        public Nyaightmare(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationBool("Running", entity.IsCartTriggered());
        }
    }
}