using System.Linq;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.starshard)]
    public class StarshardHeldItemDefinition : HeldItemDefinition
    {
        public StarshardHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        #region 实体
        public override bool IsForEntity() => !Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            HeldFlags flags = HeldFlags.ForceReset;
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    if (entity.CanEvoke())
                    {
                        flags |= HeldFlags.Valid;
                    }
                    break;
            }
            return flags;
        }
        public override bool UseOnEntity(Entity entity, long id, PointerPhase phase)
        {
            base.UseOnEntity(entity, id, phase);
            if (phase != PointerPhase.Press)
                return false;
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Evoke();
                    entity.Level.AddStarshardCount(-1);
                    return true;
            }
            return false;
        }
        #endregion

        #region 图格
        public override bool IsForGrid() => Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            var flags = HeldFlags.ForceReset;
            var entities = grid.GetTakenEntities();
            if (entities.Any(e => e.Type == EntityTypes.PLANT && e.CanEvoke()))
            {
                flags |= HeldFlags.Valid;
            }
            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, long id, PointerPhase phase)
        {
            base.UseOnGrid(grid, id, phase);
            if (phase != PointerPhase.Release)
                return false;
            var entities = grid.GetTakenEntities();
            var entity = entities.FirstOrDefault(e => e.Type == EntityTypes.PLANT && e.CanEvoke());
            if (entity != null)
            {
                entity.Evoke();
                entity.Level.AddStarshardCount(-1);
            }
            return true;
        }
        #endregion

        public override void UseOnLawn(LevelEngine level, LawnArea area, long id, PointerPhase phase)
        {
            base.UseOnLawn(level, area, id, phase);
            var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
            if (phase != targetPhase)
                return;
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }

        public override NamespaceID GetModelID(LevelEngine level, long id)
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
