using MVZ2.GameContent.HeldItem;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.trigger)]
    public class TriggerHeldItemDefinition : ToEntityHeldItemDefinition
    {
        public TriggerHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            return VanillaModelID.triggerHeldItem;
        }
        protected override bool CanUseOnEntity(Entity entity)
        {
            if (entity == null)
                return false;
            if (entity.Type != EntityTypes.PLANT)
                return false;
            if (entity.NoHeldTarget())
                return false;
            return entity.GetFaction() == entity.Level.Option.LeftFaction && entity.CanTrigger();
        }

        protected override void UseOnEntity(Entity entity)
        {
            entity.Trigger();
        }
    }
}
