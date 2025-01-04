using MVZ2.GameContent.Damages;
using MVZ2.GameContent.HeldItem;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.pickaxe)]
    public class PickaxeHeldItemDefinition : ToEntityHeldItemDefinition
    {
        public PickaxeHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected override bool CanUseOnEntity(Entity entity)
        {
            if (entity == null)
                return false;
            if (entity.Type != EntityTypes.PLANT)
                return false;
            if (entity.HasPassenger())
                return false;
            return entity.GetFaction() == entity.Level.Option.LeftFaction && !entity.CannotDig();
        }
        protected override void UseOnEntity(Entity entity)
        {
            var effects = new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE);
            entity.Die(effects);
        }

        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            return VanillaModelID.pickaxeHeldItem;
        }
    }
}
