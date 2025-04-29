using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Armors;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostZombie)]
    public class HostZombie : Zombie
    {
        public HostZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var regen = entity.AddBuff<RegenerationBuff>();
            if (regen != null)
            {
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 2f); 
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000); 
            }
        }
        
    }
}
