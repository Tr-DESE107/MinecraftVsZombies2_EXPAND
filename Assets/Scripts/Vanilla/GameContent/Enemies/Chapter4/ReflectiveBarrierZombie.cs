﻿using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.reflectiveBarrierZombie)]
    public class ReflectiveBarrierZombie : Zombie
    {
        public ReflectiveBarrierZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
