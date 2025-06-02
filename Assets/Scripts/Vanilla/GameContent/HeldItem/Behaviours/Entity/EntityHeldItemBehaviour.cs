﻿using MVZ2.HeldItems;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    public abstract class EntityHeldItemBehaviour : HeldItemBehaviourDefinition, IEntityHeldItemBehaviour
    {
        protected EntityHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public Entity GetEntity(LevelEngine level, IHeldItemData data)
        {
            return level.FindEntityByID(data.ID);
        }
        public Entity GetEntity(IHeldItemTarget target, IHeldItemData data)
        {
            return GetEntity(target.GetLevel(), data);
        }
    }
}
