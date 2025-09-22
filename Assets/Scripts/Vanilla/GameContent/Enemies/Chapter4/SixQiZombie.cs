using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.SixQiZombie)]
    public class SixQiZombie : Zombie
    {
        public SixQiZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (!entity.HasBuff<SixQiResistanceBuff>())
            {
                entity.AddBuff<SixQiResistanceBuff>();
            }

        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();

            if (entity.State == VanillaEntityStates.ATTACK)
            {
                SixQiAOE(entity, 3.34f, entity.GetFaction());
            }
        }

        public static void SixQiAOE(Entity entity, float damage, int faction)
        {

            var range = 20;

            entity.Explode(
                entity.GetCenter(),
                range,
                faction,
                damage,
                new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE, VanillaDamageEffects.VOID)
            );


        }

    }
}
