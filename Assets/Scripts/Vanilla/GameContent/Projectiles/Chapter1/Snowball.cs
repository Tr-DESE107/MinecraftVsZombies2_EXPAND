﻿using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.snowball)]
    public class Snowball : ProjectileBehaviour, IHellfireIgniteBehaviour
    {
        public Snowball(string nsp, string name) : base(nsp, name)
        {
        }
        public void Ignite(Entity entity, Entity hellfire, bool cursed)
        {
            entity.SetModelProperty("Melted", true);
        }
    }
}
