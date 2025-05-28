using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.errorBlock)]
    public class ErrorBlock : ContraptionBehaviour
    {
        public ErrorBlock(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool CanEvoke(Entity entity)
        {
            return false;
        }
    }
}
