using System.Collections.Generic;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.beingRiden)]
    public class BeingRidenBuff : BuffDefinition
    {
        public BeingRidenBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var horse = buff.GetEntity();
            if (horse == null)
                return;
            var passenger = GetPassenger(buff);
            if (!passenger.ExistsAndAlive() || passenger.IsHostile(horse))
            {
                horse.GetOffHorse();
            }
        }
        public static Entity GetPassenger(Buff buff)
        {
            var entityID = buff.GetProperty<EntityID>(PROP_TARGET);
            return entityID?.GetEntity(buff.Level);
        }
        public static void SetPassenger(Buff buff, Entity value)
        {
            buff.SetProperty(PROP_TARGET, new EntityID(value));
        }
        public const string PROP_TARGET = "Target";
    }
}
