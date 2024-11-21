using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.none)]
    public class NoneHeldItemDefinition : HeldItemDefinition
    {
        public NoneHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                    {
                        return HeldFlags.Valid;
                    }
                    break;
                case EntityTypes.CART:
                    if (!entity.IsCartTriggered())
                    {
                        return HeldFlags.Valid;
                    }
                    break;
            }
            return HeldFlags.None;
        }
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            return HeldFlags.None;
        }
        public override bool UseOnEntity(Entity entity, long id)
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
        public override void HoverOnEntity(Entity entity, long id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                        entity.Collect();
                    break;
            }
        }
        public override bool IsForGrid() => false;
        public override bool IsForEntity() => true;
        public override bool IsForPickup() => true;
    }
}
