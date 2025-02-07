using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyRegistryAttribute : Attribute
    {
        public PropertyRegistryAttribute(string regionName = null)
        {
            RegionName = regionName;
        }
        public string RegionName { get; private set; }
    }
}
