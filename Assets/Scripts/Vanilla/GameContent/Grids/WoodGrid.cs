﻿using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.Grids
{
    [GridDefinition(VanillaGridNames.wood)]
    public class WoodGrid : GridDefinition
    {
        public WoodGrid(string nsp, string name) : base(nsp, name)
        {
        }
        public override NamespaceID GetPlaceSound(Entity entity)
        {
            var entitySound = entity.GetPlaceSound();
            if (NamespaceID.IsValid(entitySound))
            {
                return entitySound;
            }
            return VanillaSoundID.wood;
        }
    }
}
