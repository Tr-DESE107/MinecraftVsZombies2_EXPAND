using MVZ2.GameContent.Enemies;
using PVZEngine.Level;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostMutant)]
    public class HostMutant : MutantZombieBase
    {
        public HostMutant(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            var regen = entity.AddBuff<RegenerationBuff>();
            if (regen != null)
            {
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 1f);
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000);
            }
        }
    }

}