using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    public class EntitySeed : SeedDefinition
    {
        public EntitySeed(string nsp, string name, int cost, NamespaceID rechargeID, EntitySeedInfo info) : base(nsp, name)
        {
            SetProperty(VanillaSeedProps.SEED_TYPE, SeedTypes.ENTITY);
            SetProperty(VanillaSeedProps.SEED_ENTITY_ID, new NamespaceID(nsp, name));
            SetProperty(EngineSeedProps.COST, cost);
            SetProperty(EngineSeedProps.RECHARGE_ID, rechargeID);
            SetProperty(VanillaSeedProps.TRIGGER_ACTIVE, info.triggerActive);
            SetProperty(VanillaSeedProps.CAN_IMBUE, info.canImbue);
            SetProperty(VanillaSeedProps.CAN_INSTANT_TRIGGER, info.canInstantTrigger);
            SetProperty(VanillaSeedProps.UPGRADE_BLUEPRINT, info.upgrade);
            if (info.upgrade)
            {
                AddAura(new UpgradeEndlessCostAura());
            }
        }

        public class UpgradeEndlessCostAura : AuraEffectDefinition
        {
            public UpgradeEndlessCostAura()
            {
                BuffID = VanillaBuffID.SeedPack.upgradeEndlessCost;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                if (!auraEffect.Level.IsEndless())
                    return;
                var source = auraEffect.Source;
                if (source is not SeedPack seedPack)
                    return;
                if (!seedPack.IsUpgradeBlueprint())
                    return;
                results.Add(seedPack);
            }
            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                if (target is not SeedPack seed)
                    return;
                var seedDef = seed?.Definition;
                if (seedDef == null || seedDef.GetSeedType() != SeedTypes.ENTITY)
                    return;
                var entityID = seed?.Definition?.GetSeedEntityID();
                if (!NamespaceID.IsValid(entityID))
                    return;
                buff.SetProperty(UpgradeEndlessCostBuff.PROP_ADDITION, seed.Level.GetEntityCount(entityID) * ADDITION);
            }
            public const float ADDITION = 50;
        }
    }
    public struct EntitySeedInfo
    {
        public bool triggerActive;
        public bool canInstantTrigger;
        public bool upgrade;
        public bool canImbue;
    }
}
