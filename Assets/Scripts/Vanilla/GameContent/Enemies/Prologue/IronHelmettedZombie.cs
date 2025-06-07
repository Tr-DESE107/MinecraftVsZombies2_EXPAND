﻿using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.ironHelmettedZombie)]
    public class IronHelmettedZombie : Zombie
    {
        public IronHelmettedZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
