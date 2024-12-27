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
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data)
        {
            var flags = HeldFlags.None;
            if (IsValidOnGrid(grid, data, out _))
            {
                flags |= HeldFlags.Valid;
            }
            return flags;
        }
        public override string GetHeldErrorMessageOnGrid(LawnGrid grid, IHeldItemData data)
        {
            if (!IsValidOnGrid(grid, data, out var error))
            {
                return Global.Game.GetGridErrorMessage(error);
            }
            return null;
        }
        public override bool UseOnGrid(LawnGrid grid, IHeldItemData data, PointerPhase phase, NamespaceID targetLayer)
        {
            var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
            if (phase != targetPhase)
                return false;
            var level = grid.Level;
            var seed = GetSeedPackAt(level, (int)data.ID);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                seed.UseOnGrid(grid, data);
                OnUseBlueprint(grid, data, seed);
                return true;
            }
            return false;
        }
        private bool IsValidOnGrid(LawnGrid grid, IHeldItemData data, out NamespaceID error)
        {
            error = null;
            var level = grid.Level;
            var seed = GetSeedPackAt(level, (int)data.ID);
            if (seed == null)
                return false;
            var seedDef = seed.GetDefinitionID();
            if (!grid.CanPlaceBlueprint(seedDef, out error))
                return false;
            return true;
        }
        #endregion
        public override void UseOnLawn(LevelEngine level, LawnArea area, IHeldItemData data, PointerPhase phase)
        {
            base.UseOnLawn(level, area, data, phase);
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
