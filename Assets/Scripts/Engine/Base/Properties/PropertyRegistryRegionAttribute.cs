using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PropertyRegistryRegionAttribute : Attribute
    {
        public PropertyRegistryRegionAttribute(string regionName = null)
        {
            RegionName = regionName;
        }
        public string RegionName { get; private set; }
    }
}
