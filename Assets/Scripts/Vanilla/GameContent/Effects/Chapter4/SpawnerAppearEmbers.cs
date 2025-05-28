using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.spawnerAppearEmbers)]
    public class SpawnerAppearEmbers : EffectBehaviour
    {

        #region 公有方法
        public SpawnerAppearEmbers(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}