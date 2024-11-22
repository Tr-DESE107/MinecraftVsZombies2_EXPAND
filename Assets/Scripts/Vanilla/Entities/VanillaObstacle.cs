using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaObstacle : VanillaEntity
    {
        protected VanillaObstacle(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaContraptionProps.FRAGMENT_ID, GetID());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.RightFaction);
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.PlaySound(entity.GetDeathSound());
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
        public override int Type => EntityTypes.OBSTACLE;
    }
}
