using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.levelEasy)]
    public class LevelEasyBuff : BuffDefinition
    {
        public LevelEasyBuff(string nsp, string name) : base(nsp, name)
        {
            AddAura(new BlueprintAura());
            AddAura(new ContraptionAura());
            AddAura(new ArmorAura());
            AddModifier(new FloatModifier(VanillaLevelProps.CONVEY_SPEED, NumberOperator.Multiply, 1.5f));
        }

        public class BlueprintAura : AuraEffectDefinition
        {
            public BlueprintAura() : base()
            {
                BuffID = VanillaBuffID.SeedPack.easyBlueprint;
                UpdateInterval = 30;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetAllSeedPacks());
            }
        }
        public class ContraptionAura : AuraEffectDefinition
        {
            public ContraptionAura() : base()
            {
                BuffID = VanillaBuffID.easyContraption;
                UpdateInterval = 4;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.PLANT));
            }
        }
        public class ArmorAura : AuraEffectDefinition
        {
            public ArmorAura() : base()
            {
                BuffID = VanillaBuffID.easyArmor;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.ENEMY).Select(e => e.EquipedArmor).Where(e => e != null));
            }
        }
    }
}
