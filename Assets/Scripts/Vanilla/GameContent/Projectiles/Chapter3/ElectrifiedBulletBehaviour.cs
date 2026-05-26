using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.ElectrifiedBullet)]
    public class ElectrifiedBulletBehaviour : ProjectileBehaviour, IElectrifyBehaviour
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