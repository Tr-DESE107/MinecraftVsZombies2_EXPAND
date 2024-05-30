using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class AreaDefinition : Definition
    {
        public AreaDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(AreaProperties.ENEMY_SPAWN_X, 1080);
        }
        public NamespaceID[] GetGridDefintionsID()
        {
            return grids.ToArray();
        }
        public virtual void PrepareForBattle(Level level) { }
        protected List<NamespaceID> grids = new List<NamespaceID>();
    }
}
