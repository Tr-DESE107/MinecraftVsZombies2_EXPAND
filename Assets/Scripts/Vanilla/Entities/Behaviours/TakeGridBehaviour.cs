﻿using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.takeGrid)]
    public class TakeGridBehaviour : EntityBehaviourDefinition
    {
        public TakeGridBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.UpdateTakenGrids();
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            entity.UpdateTakenGrids();
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
    }
}