using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.ridingPassenger)]
    public class RidingPassengerBuff : BuffDefinition
    {
        public RidingPassengerBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEnemyProps.HARMLESS, true));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var passenger = buff.GetEntity();
            if (passenger == null)
                return;
            var horse = GetRidingEntity(buff);
            if (!horse.ExistsAndAlive() || horse.IsHostile(passenger))
            {
                passenger.GetOffPassenger();
            }
            else
            {
                passenger.Position = horse.Position + horse.GetPassengerOffset();
                passenger.Velocity = Vector3.zero;
            }
        }
        public static Entity GetRidingEntity(Buff buff)
        {
            var entityID = buff.GetProperty<EntityID>(PROP_TARGET);
            return entityID?.GetEntity(buff.Level);
        }
        public static void SetRidingEntity(Buff buff, Entity value)
        {
            buff.SetProperty(PROP_TARGET, new EntityID(value));
        }
        public static readonly VanillaBuffPropertyMeta PROP_TARGET = new VanillaBuffPropertyMeta("Target");
    }
}
