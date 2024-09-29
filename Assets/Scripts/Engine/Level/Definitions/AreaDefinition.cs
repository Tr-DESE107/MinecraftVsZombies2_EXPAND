using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public class AreaDefinition : Definition
    {
        public AreaDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(AreaProperties.ENEMY_SPAWN_X, 1080);
            SetProperty(AreaProperties.GRID_WIDTH, 80);
            SetProperty(AreaProperties.GRID_HEIGHT, 80);
            SetProperty(AreaProperties.GRID_LEFT_X, 260);
            SetProperty(AreaProperties.GRID_BOTTOM_Z, 80);
            SetProperty(AreaProperties.MAX_LANE_COUNT, 5);
            SetProperty(AreaProperties.MAX_COLUMN_COUNT, 9);
        }
        public virtual void Setup(LevelEngine level) { }
        public NamespaceID[] GetGridDefintionsID()
        {
            return grids.ToArray();
        }
        public virtual void PrepareForBattle(LevelEngine level) { }
        public virtual float GetGroundY(float x, float z) { return 0; }
        protected List<NamespaceID> grids = new List<NamespaceID>();
    }
}
