using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaContraption : VanillaEntity, IEvokableContraption
    {
        public VanillaContraption()
        {
            SetProperty(EntityProperties.ATTACK_SPEED, 1f);
            SetProperty(EntityProperties.DAMAGE, 100f);
            SetProperty(EntityProperties.MAX_HEALTH, 300f);
            SetProperty(EntityProperties.FRICTION, 0.2f);
            SetProperty(EntityProperties.SHELL, ShellID.stone);
            SetProperty(EntityProperties.FALL_DAMAGE, 22.5f);
            SetProperty(EntityProps.DEATH_SOUND, SoundID.stone);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Game.Option.LeftFaction);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            UpdateTakenGrids(entity);
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.Game.PlaySound(entity.GetDeathSound(), entity.Pos);
            entity.Remove();
        }
        public virtual void Evoke(Contraption contraption)
        {
        }
        public override int Type => EntityTypes.PLANT;

    }
    public interface IEvokableContraption
    {
        void Evoke(Contraption contraption);
    }
}