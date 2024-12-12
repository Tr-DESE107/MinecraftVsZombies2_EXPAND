using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
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
using UnityEngine;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.blueprint)]
    public class BlueprintHeldItemDefinition : HeldItemDefinition
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
            if (!IsValidOnGrid(grid, data, out var message))
            {
                return message;
            }
            return null;
        }
        public override bool UseOnGrid(LawnGrid grid, IHeldItemData data, PointerPhase phase)
        {
            var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
            if (phase != targetPhase)
                return false;
            var level = grid.Level;
            var seed = level.GetSeedPackAt((int)data.ID);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var x = level.GetEntityColumnX(grid.Column);
                var z = level.GetEntityLaneZ(grid.Lane);
                var y = level.GetGroundY(x, z);

                var position = new Vector3(x, y, z);
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                var entity = level.Spawn(entityID, position, null);
                level.AddEnergy(-seedDef.GetCost());
                level.SetRechargeTimeToUsed(seed);
                seed.ResetRecharge();
                if (data.InstantTrigger && entity.CanTrigger())
                {
                    entity.Trigger();
                }
                level.PlaySound(entityDef.GetPlaceSound(), position);
                if (entity.Type == EntityTypes.PLANT)
                {
                    level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_PLACE, entityID, entity);
                }
                return true;
            }
            return false;
        }
        private bool IsValidOnGrid(LawnGrid grid, IHeldItemData data, out string errorMessage)
        {
            errorMessage = null;
            var level = grid.Level;
            var seed = level.GetSeedPackAt((int)data.ID);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                if (entityDef.Type == EntityTypes.PLANT)
                {
                    if (!grid.CanPlace(entityDef))
                    {
                        if (grid.GetTakenEntities().Any(e => e.IsEntityOf(VanillaObstacleID.gargoyleStatue)))
                            errorMessage = VanillaStrings.ADVICE_CANNOT_PLACE_ON_STATUES;
                        return false;
                    }
                }
            }
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
            var seed = level.GetSeedPackAt((int)id);
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
    }
}
