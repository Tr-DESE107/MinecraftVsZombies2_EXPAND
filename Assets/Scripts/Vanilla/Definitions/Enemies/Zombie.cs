using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Enemies
{
    public class Zombie : VanillaEnemy
    {
        protected override bool ValidateAttackTarget(Enemy enemy, Entity other)
        {
            return false;
        }
    }
}
