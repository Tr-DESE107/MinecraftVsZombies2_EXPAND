using MVZ2.GameContent.Enemies;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.MutantVillager)]
    public class MutantVillager : MutantZombieBase
    {
        public MutantVillager(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaEnemyID.ImpMannequin);
        }
    }
}