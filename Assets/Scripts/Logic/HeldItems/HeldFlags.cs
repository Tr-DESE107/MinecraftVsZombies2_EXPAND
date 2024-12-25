using System;

namespace MVZ2Logic.HeldItems
{
    [Flags]
    public enum HeldFlags
    {
        None = 0,
        Valid = 1,
        ForceReset = 1 << 1,
        NoHighlight = 1 << 2,
        ValidOnProtector = 1 << 3,
        ValidOnCarrier = 1 << 4,
    }
}
