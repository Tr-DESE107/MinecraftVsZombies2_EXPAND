using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HeavyGutant)]
    public class HeavyGutant : MutantZombieBase
    {
        public HeavyGutant(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaProjectileID.missile);
        }
    }

}