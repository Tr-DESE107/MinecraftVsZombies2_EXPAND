﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.iceBolt)]
    public class IceBolt : ProjectileBehaviour, IHellfireIgniteBehaviour
    {
        public IceBolt(string nsp, string name) : base(nsp, name)
        {
        }
        public void Ignite(Entity entity, Entity hellfire, bool cursed)
        {
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.SIZE, entity.GetScaledSize());
            entity.Spawn(VanillaEffectID.smoke, entity.Position, param);
            entity.Die();
            entity.PlaySound(VanillaSoundID.fizz);
        }
    }
}
