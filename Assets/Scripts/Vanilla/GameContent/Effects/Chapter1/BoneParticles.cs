using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.boneParticles)]
    public class BoneParticles : EffectBehaviour
    {

        #region 公有方法
        public BoneParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}