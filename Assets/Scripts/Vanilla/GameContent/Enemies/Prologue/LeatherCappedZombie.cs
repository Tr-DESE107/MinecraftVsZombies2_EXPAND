﻿using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.leatherCappedZombie)]
    public class LeatherCappedZombie : Zombie
    {
        public LeatherCappedZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
