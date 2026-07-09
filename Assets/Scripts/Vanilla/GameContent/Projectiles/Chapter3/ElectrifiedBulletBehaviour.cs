using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;

using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [AutoEntityBehaviourDefinition(VanillaEntityBehaviourNames.ElectrifiedBullet)]
    public class ElectrifiedBulletBehaviour : EntityBehaviourDefinition, IElectrifyBehaviour
    {
        public ElectrifiedBulletBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public void Electrify(Entity entity, Entity teslaCoil)
        {
            var electrifiedBuff = entity.GetFirstBuff<ElectrifiedBuff>();
            if (electrifiedBuff == null)
            {
                entity.AddBuff<ElectrifiedBuff>();
            }
        }
    }
}