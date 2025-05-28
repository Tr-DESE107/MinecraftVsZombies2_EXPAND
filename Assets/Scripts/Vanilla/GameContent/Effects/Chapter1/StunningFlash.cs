using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.stunningFlash)]
    public class StunningFlash : EffectBehaviour
    {
        #region ���з���
        public StunningFlash(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetDisplayScale(Vector3.one * entity.Timeout);
        }
        #endregion
    }
}