using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Vanilla.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla
{
    [Definition(VanillaObstacleNames.gargoyleStatue)]
    public class GargoyleStatue : VanillaObstacle
    {
        public GargoyleStatue(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.RightFaction);
            entity.AddBuff<TemporaryUpdateBeforeGameBuff>();
            entity.TriggerAnimation("Rise");
            entity.PlaySound(SoundID.dirtRise);

            entity.InitFragment();
            foreach (var contraption in entity.Level.FindEntities(e => e.Type == EntityTypes.PLANT && e.GetGrid() == entity.GetGrid()))
            {
                contraption.Die();
            }
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.PostFragmentDeath(damageInfo);
            entity.Remove();
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(5));

            UpdateTakenGrids(entity);

            entity.UpdateFragment();
        }
    }
}
