﻿using MVZ2.HeldItems;
using PVZEngine.Entities;

namespace MVZ2Logic.HeldItems
{
    public interface IHeldTwinkleEntityBehaviour
    {
        bool ShouldMakeEntityTwinkle(Entity entity, IHeldItemData data);
    }
}
