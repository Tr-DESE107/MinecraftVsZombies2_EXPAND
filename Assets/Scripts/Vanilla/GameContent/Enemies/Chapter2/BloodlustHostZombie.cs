using MVZ2.GameContent.Buffs.Enemies;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.BloodlustHostZombie)]
    public class BloodlustHostZombie : Zombie
    {
        public BloodlustHostZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var regen = entity.AddBuff<RegenerationBuff>();
            if (regen != null)
            {
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 3f);
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000);
            }
        }

    }
}
