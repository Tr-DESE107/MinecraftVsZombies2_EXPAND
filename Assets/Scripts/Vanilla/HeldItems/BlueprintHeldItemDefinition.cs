using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    public abstract class BlueprintHeldItemDefinition : HeldItemDefinition
    {
        public BlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        #region 实体
        public override bool IsForEntity() => false;
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, IHeldItemData data)
        {
            return HeldFlags.None;
        }
        #endregion

        #region 网格
        public override bool IsForGrid() => true;
        public override bool FilterGridPointerPhase(PointerPhase phase)
        {
            return phase == (Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press);
        }
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data)
        {
            var flags = HeldFlags.None;
            var level = grid.Level;
            var seed = GetSeedPackAt(level, (int)data.ID);
            if (seed != null)
            {
                var seedDef = seed.GetDefinitionID();
                if (grid.CanPlaceBlueprint(seedDef, out _))
                {
                    flags |= HeldFlags.Valid;
                }
            }
            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, IHeldItemData data, NamespaceID targetLayer)
        {
            var level = grid.Level;
            var seed = GetSeedPackAt(level, (int)data.ID);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (!grid.CanPlaceBlueprint(seedDef.GetID(), out var error))
            {
                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, Global.Game.GetGridErrorMessage(error), 0, 150);
                return false;
            }

            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                seed.UseOnGrid(grid, data);
                OnUseBlueprint(grid, data, seed);
                return true;
            }
            return false;
        }
        #endregion
        public override void UseOnLawn(LevelEngine level, LawnArea area, IHeldItemData data)
        {
            base.UseOnLawn(level, area, data);
            if (area != LawnArea.Side)
                return;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        public override NamespaceID GetModelID(LevelEngine level, long id)
        {
            var seed = GetSeedPackAt(level, (int)id);
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                return entityDef.GetModelID();
            }
            return null;
        }
        protected abstract SeedPack GetSeedPackAt(LevelEngine level, int index);
        public override SeedPack GetSeedPack(LevelEngine level, IHeldItemData data)
        {
            return GetSeedPackAt(level, (int)data.ID);
        }
        protected virtual void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed)
        {
        }
    }
}
