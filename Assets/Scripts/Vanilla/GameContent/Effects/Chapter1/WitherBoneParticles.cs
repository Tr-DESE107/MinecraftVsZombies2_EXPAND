using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.wither_bone_particles)]
    public class WitherBoneParticles : EffectBehaviour
    {

        #region 公有方法
        public WitherBoneParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}