using MVZ2.GameContent.Effects;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.waterStainBlown)]
    public class WaterStainBlownBehaviour : EntityBehaviourDefinition, IBeBlownBehaviour
    {
        public WaterStainBlownBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        public void BeBlown(Entity entity, Entity source)
        {
            if (WaterStain.IsStainFrozen(entity))
                return;
            WaterStain.Disappear(entity);
        }
    }
}
