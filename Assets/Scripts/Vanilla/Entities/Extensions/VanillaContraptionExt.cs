using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaContraptionExt
    {
        public static bool CanEvoke(this Entity contraption)
        {
            var evokable = contraption.Definition.GetBehaviour<IEvokableContraption>();
            if (evokable == null)
                return false;
            return evokable.CanEvoke(contraption);
        }
        public static void Evoke(this Entity contraption)
        {
            var evokable = contraption.Definition.GetBehaviour<IEvokableContraption>();
            if (evokable == null)
                return;
            evokable.Evoke(contraption);
            contraption.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, contraption.GetDefinitionID(), c => c(contraption));
        }
        public static bool HasPassenger(this Entity contraption)
        {
            var grid = contraption?.GetGrid();
            if (grid != null && grid.GetCarrierEntity() == contraption)
            {
                var main = grid.GetMainEntity();
                if (main != null && main.Exists() && main != contraption)
                {
                    return true;
                }
                var protector = grid.GetProtectorEntity();
                if (protector != null && protector.Exists() && protector != contraption)
                {
                    return true;
                }
            }
            return false;
        }
        public static Entity GetProtectingTarget(this Entity contraption)
        {
            var grid = contraption?.GetGrid();
            if (grid == null || grid.GetProtectorEntity() != contraption)
                return null;
            var main = grid.GetMainEntity();
            if (main != null && main.Exists() && main != contraption)
                return main;
            var carrier = grid.GetCarrierEntity();
            if (carrier != null && carrier.Exists() && carrier != contraption)
                return carrier;
            return null;
        }
        public static Entity GetProtector(this Entity contraption)
        {
            var grid = contraption?.GetGrid();
            if (grid == null)
                return null;
            var protector = grid.GetProtectorEntity();
            if (protector != null && protector.Exists() && protector != contraption)
                return protector;
            return null;
        }
        public static bool CanTrigger(this Entity contraption)
        {
            var triggerable = contraption.Definition.GetBehaviour<ITriggerableContraption>();
            if (triggerable == null)
                return false;
            return triggerable.CanTrigger(contraption);
        }
        public static void Trigger(this Entity contraption)
        {
            var triggerable = contraption.Definition.GetBehaviour<ITriggerableContraption>();
            if (triggerable == null)
                return;
            triggerable.Trigger(contraption);
            contraption.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_CONTRAPTION_TRIGGER, c => c(contraption));
        }
        public static Entity UpgradeToContraption(this Entity contraption, NamespaceID target)
        {
            var grid = contraption.GetGrid();
            if (grid == null)
                return null;
            var awake = !contraption.HasBuff<NocturnalBuff>();
            contraption.Remove();
            var upgraded = contraption.GetGrid().PlaceEntity(target);
            if (upgraded == null)
                return null;
            if (awake)
            {
                upgraded.RemoveBuffs<NocturnalBuff>();
            }
            return upgraded;
        }
        public static bool IsEvoked(this Entity contraption)
        {
            return contraption.GetProperty<bool>(PROP_EVOKED);
        }
        public static void SetEvoked(this Entity contraption, bool value)
        {
            contraption.SetProperty(PROP_EVOKED, value);
        }

        public static void ShortCircuit(this Entity contraption, int time)
        {
            var buff = contraption.AddBuff<FrankensteinShockedBuff>();
            buff.SetProperty(FrankensteinShockedBuff.PROP_TIMEOUT, time);
        }
        public static void InflictWither(this Entity enemy, int time)
        {
            Buff buff = enemy.GetFirstBuff<WitheredBuff>();
            if (buff == null)
            {
                buff = enemy.AddBuff<WitheredBuff>();
            }
            buff.SetProperty(WitheredBuff.PROP_TIMEOUT, time);
        }
        private const string PROP_REGION = "contraptions";
        [PropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta PROP_EVOKED = new VanillaEntityPropertyMeta("Evoked");
    }
}
