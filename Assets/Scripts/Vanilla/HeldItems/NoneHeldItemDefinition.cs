using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.none)]
    public class NoneHeldItemDefinition : HeldItemDefinition
    {
        public NoneHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        #region 实体
        public override bool IsForEntity() => true;
        public override bool IsForPickup() => true;
        public override bool FilterEntityPointerPhase(Entity entity, PointerPhase phase)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    return phase != PointerPhase.Release;
                    break;
                case EntityTypes.CART:
                    return phase == PointerPhase.Press;
            }
            return false;
        }
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, IHeldItemData data)
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
        public override bool UseOnEntity(Entity entity, IHeldItemData data)
        {
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                        entity.Collect();
                    break;
                case EntityTypes.CART:
                    if (!entity.IsCartTriggered())
                        entity.TriggerCart();
                    break;
            }
            return false;
        }
        #endregion

        public override bool IsForGrid() => false;
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data)
        {
            return HeldFlags.None;
        }
    }
}
