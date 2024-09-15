using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.none)]
    public class NoneHeldItemDefinition : HeldItemDefinition
    {
        public NoneHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    return !entity.IsCollected();
                case EntityTypes.CART:
                    return !entity.IsCartTriggered();
            }
            return false;
        }
        public override bool IsValidOnGrid(LawnGrid grid, int id)
        {
            return false;
        }
        public override bool UseOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    entity.Collect();
                    break;
                case EntityTypes.CART:
                    entity.TriggerCart();
                    break;
            }
            return false;
        }
        public override void HoverOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                        entity.Collect();
                    break;
            }
        }
    }
}
