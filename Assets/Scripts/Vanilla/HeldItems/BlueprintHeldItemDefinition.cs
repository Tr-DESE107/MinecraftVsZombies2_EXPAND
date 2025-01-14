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
        public override bool CheckRaycast(HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetGrid;
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seed = GetSeedPackAt(level, (int)data.ID);
            if (seed != null)
            {
                var seedDef = seed.GetDefinitionID();
                if (grid.CanPlaceBlueprint(seedDef, out _))
                {
                    return HeldHighlight.Green;
                }
                else
                {
                    return HeldHighlight.Red;
                }
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    {
                        var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
                        if (interaction != targetPhase)
                            return;

                        var grid = gridTarget.Target;

                        var level = grid.Level;
                        var seed = GetSeedPackAt(level, (int)data.ID);
                        if (seed == null)
                            return;
                        var seedDef = seed.Definition;
                        if (!grid.CanPlaceBlueprint(seedDef.GetID(), out var error))
                        {
                            var message = Global.Game.GetGridErrorMessage(error);
                            if (!string.IsNullOrEmpty(message))
                            {
                                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, message, 0, 150);
                            }
                            return;
                        }

                        if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                        {
                            seed.UseOnGrid(grid, data);
                            OnUseBlueprint(grid, data, seed);
                            level.ResetHeldItem();
                        }
                    }
                    break;
                case HeldItemTargetLawn lawnTarget:
                    {
                        var level = lawnTarget.Level;
                        var area = lawnTarget.Area;

                        if (area == LawnArea.Side)
                        {
                            if (level.CancelHeldItem())
                            {
                                level.PlaySound(VanillaSoundID.tap);
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 网格
        #endregion
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var seed = GetSeedPackAt(level, (int)data.ID);
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
