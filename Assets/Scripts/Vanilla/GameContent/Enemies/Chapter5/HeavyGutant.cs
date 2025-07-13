using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Contraptions;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HeavyGutant)]
    public class HeavyGutant : MutantZombieBase
    {
        public HeavyGutant(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaContraptionID.tnt);
        }
    }

}