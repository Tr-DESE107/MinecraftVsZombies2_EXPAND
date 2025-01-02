using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.dreamCrystalEvocation)]
    public class DreamCrystalEvocationBuff : BuffDefinition
    {
        public DreamCrystalEvocationBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var contraption = buff.GetEntity();
            if (contraption == null)
                return;
            foreach (Entity target in contraption.Level.FindEntities(e => CanHeal(contraption, e)))
            {
                target.Heal(HEAL_PER_FRAME, contraption);
            }
            var time = buff.GetProperty<int>(PROP_TIMEOUT);
            time--;
            if (time <= 0)
            {
                buff.Remove();
            }
            buff.SetProperty(PROP_TIMEOUT, time);
        }
        private static bool CanHeal(Entity self, Entity target)
        {
            return Detection.IsInSphere(target.MainHitbox, self.GetCenter(), 100) && self.IsFriendly(target);
        }
        public const string PROP_TIMEOUT = "Timeout";
        public const int MAX_TIMEOUT = 150;
        public const float HEAL_PER_FRAME = 20;
    }
}
