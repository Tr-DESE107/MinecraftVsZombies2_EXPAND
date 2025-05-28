﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.entityPhysics)]
    public class EntityPhysicsBuff : BuffDefinition
    {
        public EntityPhysicsBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.FRICTION, NumberOperator.Multiply, PROP_FRICTION));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_FRICTION, 1f);
            UpdateMultipliers(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateMultipliers(buff);
        }
        private void UpdateMultipliers(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            float friction = 1;
            if (!entity.IsOnGround && !entity.KeepGroundFriction())
            {
                friction = 0.1f;
            }
            buff.SetProperty(PROP_FRICTION, friction);
        }
        public static readonly VanillaBuffPropertyMeta<float> PROP_FRICTION = new VanillaBuffPropertyMeta<float>("Friction");
    }
}
