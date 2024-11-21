using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVZ2.HeldItems
{
    [Flags]
    public enum HeldFlags
    {
        None = 0,
        Valid = 1,
        ForceReset = 1 << 1,
        HideGridColor = 1 << 2
    }
}
