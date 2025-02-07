using MVZ2.GameContent.HeldItem;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.starshard)]
    public class StarshardHeldItemDefinition : ToEntityHeldItemDefinition
    {
        public StarshardHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        protected override bool CanUseOnEntity(Entity entity)
        {
            if (entity == null)
                return false;
            if (entity.Type != EntityTypes.PLANT)
                return false;
            if (entity.NoHeldTarget())
                return false;
            return entity.GetFaction() == entity.Level.Option.LeftFaction && entity.CanEvoke();
        }
        protected override void UseOnEntity(Entity entity)
        {
            entity.Level.AddStarshardCount(-1);
            entity.Evoke();
        }

        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var modelID = VanillaModelID.GetStarshardHeldItem(level.AreaDefinition.GetID());
            if (Global.Game.GetModelMeta(modelID) == null)
            {
                return VanillaModelID.defaultStartShardHeldItem;
            }
            return modelID;
        }
    }
}
