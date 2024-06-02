using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(AreaNames.day)]
    public class Day : AreaDefinition
    {
        public Day(string nsp, string name) : base(nsp, name)
        {
            SetProperty(AreaProperties.GRID_WIDTH, 80);
            SetProperty(AreaProperties.GRID_HEIGHT, 80);
            SetProperty(AreaProperties.GRID_LEFT_X, 260);
            SetProperty(AreaProperties.GRID_BOTTOM_Z, 80);
            SetProperty(AreaProperties.MAX_LANE_COUNT, 5);
            SetProperty(AreaProperties.MAX_COLUMN_COUNT, 9);
            SetProperty(AreaProperties.CART_REFERENCE, CartID.minecart);
            SetProperty(LevelProps.MUSIC_ID, MusicID.day);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(GridID.grass);
            }
        }
    }
}