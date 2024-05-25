using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class AreaDefinition : Definition
    {

        public AreaDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public NamespaceID[] GetGridDefintionsID()
        {
            return grids.ToArray();
        }
        protected List<NamespaceID> grids = new List<NamespaceID>();
    }
}
