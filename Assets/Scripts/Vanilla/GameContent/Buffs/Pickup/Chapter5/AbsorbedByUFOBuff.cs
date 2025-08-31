﻿using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Pickup.absorbedByUFO)]
    public class AbsorbedByUFOBuff : BuffDefinition
    {
        public AbsorbedByUFOBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Multiply, 0));
            AddModifier(new BooleanModifier(VanillaPickupProps.NO_COLLECT, true));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity != null)
            {
                var ufoID = GetUFOID(buff);
                var ufo = ufoID.GetEntity(buff.Level);
                if (!ufo.ExistsAndAlive())
                {
                    buff.Remove();
                    return;
                }

                var distance = ufo.Position - entity.Position;
                var velocity = entity.Velocity;
                velocity = velocity * (1 - MOVE_FACTOR) + (distance).normalized * ABSORB_SPEED * MOVE_FACTOR;
                entity.Velocity = velocity;

                if (distance.sqrMagnitude <= SQR_ABSORB_DISTANCE)
                {
                    entity.Remove();
                    buff.Remove();
                    UFOBehaviourBlue.AddAbsorbedEntityID(ufo, entity.GetDefinitionID());
                }
            }
        }
        public static EntityID GetUFOID(Buff buff) => buff.GetProperty<EntityID>(PROP_UFO);
        public static void SetUFOID(Buff buff, EntityID value) => buff.SetProperty(PROP_UFO, value);
        public const float ABSORB_SPEED = 10f;
        public const float ABSORB_DISTANCE = 20;
        public const float SQR_ABSORB_DISTANCE = ABSORB_DISTANCE * ABSORB_DISTANCE;
        public const float MOVE_FACTOR = 0.2f;
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_UFO = new VanillaBuffPropertyMeta<EntityID>("ufo");
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
    }
}
