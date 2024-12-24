using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Carts;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Grids;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.dream)]
    public class Dream : AreaDefinition
    {
        public Dream(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaAreaProps.DOOR_Z, 240f);
            SetProperty(EngineAreaProps.CART_REFERENCE, VanillaCartID.nyanCat);
            SetProperty(EngineAreaProps.AREA_TAGS, new NamespaceID[] { VanillaAreaTags.day, VanillaAreaTags.water });
            SetProperty(VanillaLevelProps.MUSIC_ID, VanillaMusicID.dreamLevel);
            SetProperty(EngineAreaProps.GRID_BOTTOM_Z, 40);
            SetProperty(EngineAreaProps.MAX_LANE_COUNT, 6);
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (x >= 3 && x <= 6 && y >= 1 && y <= 4)
                    {
                        grids.Add(VanillaGridID.water);
                    }
                    else
                    {
                        grids.Add(VanillaGridID.grass);
                    }
                }
            }
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            if (Global.Game.IsUnlocked(VanillaUnlockID.dreamIsNightmare))
            {
                level.SetAreaModelPreset(VanillaAreaModelPresets.Dream.nightmare);
                level.AddBuff<NightmareLevelBuff>();
            }
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
        }
    }
}