using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Callbacks;

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
            entity.Level.Spawn(VanillaEffectID.evocationStar, entity.GetCenter(), entity);
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