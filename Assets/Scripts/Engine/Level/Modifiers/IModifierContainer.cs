using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine.Modifiers
{
    public interface IModifierContainer
    {
        public object GetProperty(PropertyKey name);
    }
}
