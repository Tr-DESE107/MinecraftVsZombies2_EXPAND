using System;

namespace MVZ2Logic.HeldItems
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
