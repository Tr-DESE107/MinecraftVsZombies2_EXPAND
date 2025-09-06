using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.NetherVanguard)]
    public class NetherVanguard : MeleeEnemy
    {
        public NetherVanguard(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (!entity.HasBuff<VanguardResistanceBuff>())
            {
                entity.AddBuff<VanguardResistanceBuff>();
            }
            if (entity.Level.IsIZombie())
            {
                //entity.AddBuff<IZombieSkeletonWarriorBuff>();
                var helmet = entity.GetMainArmor();
                var shield = entity.GetArmorAtSlot(VanillaArmorSlots.shield);
                //helmet?.AddBuff<IZombieSkeletonWarriorArmorBuff>();
                //shield?.AddBuff<IZombieSkeletonWarriorArmorBuff>();
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();

            if (entity.State == VanillaEntityStates.ATTACK)
            {
                WitherAOE(entity, 2f, entity.GetFaction());
            }
        }

        public static void WitherAOE(Entity entity, float damage, int faction)
        {

            var range = 80;

            entity.Explode(
                entity.GetCenter(),
                range,
                faction,
                damage,
                new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE, VanillaDamageEffects.WITHER, VanillaDamageEffects.WITHER)
            );


        }
    }
}
