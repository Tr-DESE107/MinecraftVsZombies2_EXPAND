﻿using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.Grids
{
    [GridDefinition(VanillaGridNames.stoneSlab)]
    public class StoneSlabGrid : GridDefinition
    {
        public StoneSlabGrid(string nsp, string name) : base(nsp, name)
        {
            SetProperty<bool>(VanillaGridProps.IS_SLAB, true);
        }
        public override NamespaceID GetPlaceSound(Entity entity)
        {
            var entitySound = entity.GetPlaceSound();
            if (NamespaceID.IsValid(entitySound))
            {
                return entitySound;
            }
            return VanillaSoundID.stone;
        }
    }
}
