﻿using MVZ2.Vanilla.Contraptions;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class ContraptionBehaviour : AIEntityBehaviour, IEvokableContraption, ITriggerableContraption
    {
        public ContraptionBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual bool CanEvoke(Entity entity)
        {
            return !entity.IsEvoked() && !entity.IsAIFrozen();
        }
        public virtual void Evoke(Entity entity)
        {
            OnEvoke(entity);
        }
        public virtual bool CanTrigger(Entity entity)
        {
            return entity.IsTriggerActive() && !entity.IsAIFrozen();
        }
        public virtual void Trigger(Entity entity)
        {
            OnTrigger(entity);
        }
        protected virtual void OnTrigger(Entity entity)
        {

        }
        protected virtual void OnEvoke(Entity entity)
        {

        }

    }
}