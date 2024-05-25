using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(AreaNames.day)]
    public class Day : AreaDefinition
    {
        public Day(string nsp, string name) : base(nsp, name)
        {
            SetProperty(AreaProperties.GRID_SIZE, 80);
            SetProperty(AreaProperties.GRID_LEFT_X, 260);
            SetProperty(AreaProperties.GRID_BOTTOM_Z, 80);
            SetProperty(AreaProperties.MAX_LANE_COUNT, 5);
            SetProperty(AreaProperties.MAX_COLUMN_COUNT, 9);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(GridID.grass);
            }
        }
    }
}