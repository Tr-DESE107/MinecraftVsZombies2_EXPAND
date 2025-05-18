using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Obstacles
{
    [EntityBehaviourDefinition(VanillaObstacleNames.gargoyleStatue)]
    public class GargoyleStatue : ObstacleBehaviour
    {
        public GargoyleStatue(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.AddBuff<TemporaryUpdateBeforeGameBuff>();
            entity.TriggerAnimation("Rise");
            entity.PlaySound(VanillaSoundID.dirtRise);

            KillConflictContraptions(entity);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.Remove();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(5));
        }
    }
}
