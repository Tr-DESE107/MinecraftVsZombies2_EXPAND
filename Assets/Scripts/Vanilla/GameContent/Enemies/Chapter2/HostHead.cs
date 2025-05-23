using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostHead)]
    public class HostHead : MeleeEnemy
    {
        public HostHead(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            var fly = entity.AddBuff<FlyBuff>();
            if (fly != null)
            {
                fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 1);
            }

            
            var regen = entity.AddBuff<RegenerationBuff>(); 
            if (regen != null)
            {
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 1.5f); 
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000);  
            }
        }
    }
}
