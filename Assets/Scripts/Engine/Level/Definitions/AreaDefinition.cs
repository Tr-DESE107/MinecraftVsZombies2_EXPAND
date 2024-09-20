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
        }
        public virtual void Setup(LevelEngine level) { }
        public NamespaceID[] GetGridDefintionsID()
        {
            return grids.ToArray();
        }
        public virtual void PrepareForBattle(LevelEngine level) { }
        protected List<NamespaceID> grids = new List<NamespaceID>();
    }
}
