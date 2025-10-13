using MVZ2.GameContent.Enemies;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.MegaMutantVillager)]
    public class MegaMutantVillager : MutantZombieBase
    {
        public MegaMutantVillager(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaEnemyID.ImpMannequin);
        }
    }
}