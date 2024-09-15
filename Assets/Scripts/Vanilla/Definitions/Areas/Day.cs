using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(AreaNames.day)]
    public class Day : AreaDefinition
    {
        public Day(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinAreaProps.DOOR_Z, 240f);
            SetProperty(AreaProperties.GRID_WIDTH, 80);
            SetProperty(AreaProperties.GRID_HEIGHT, 80);
            SetProperty(AreaProperties.GRID_LEFT_X, 260);
            SetProperty(AreaProperties.GRID_BOTTOM_Z, 80);
            SetProperty(AreaProperties.MAX_LANE_COUNT, 5);
            SetProperty(AreaProperties.MAX_COLUMN_COUNT, 9);
            SetProperty(AreaProperties.CART_REFERENCE, CartID.minecart);
            SetProperty(BuiltinLevelProps.MUSIC_ID, MusicID.day);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(GridID.grass);
            }
        }
        public override void Init(LevelEngine level)
        {
            level.Spawn<Miner>(new Vector3(600, 0, 60), null);
        }
    }
}