using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class AreaDefinition : Definition
    {
        public GridDefinition[] GetGrids()
        {
            return grids.Select(i => gridDefinitions[i]).ToArray();
        }
        private List<int> grids = new List<int>();
        private List<GridDefinition> gridDefinitions = new List<GridDefinition>();
    }
}
