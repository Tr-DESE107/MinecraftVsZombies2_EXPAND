﻿using UnityEngine;

namespace MVZ2.UI
{
    public interface ITooltipAnchor
    {
        Vector3 Position { get; }
        bool IsDisabled { get; }
        Vector2 Pivot { get; }
    }
}
