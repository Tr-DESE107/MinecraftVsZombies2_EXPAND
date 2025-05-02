using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla.Entities;
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
            AddModifier(new IntModifier(VanillaLevelProps.ENEMY_AI_LEVEL, NumberOperator.Add, -1));
            AddModifier(new IntModifier(VanillaLevelProps.BOSS_AI_LEVEL, NumberOperator.Add, -1));
            AddModifier(new FloatModifier(VanillaLevelProps.CONVEY_SPEED, NumberOperator.Multiply, 1.5f));
            AddModifier(new FloatModifier(VanillaLevelProps.STARSHARD_CARRIER_CHANCE_INCREAMENT, NumberOperator.Multiply, 2));
            AddModifier(new FloatModifier(VanillaLevelProps.REDSTONE_CARRIER_CHANCE_INCREAMENT, NumberOperator.Multiply, 2));
            AddModifier(new FloatModifier(VanillaLevelProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Multiply, 0.5f));
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
                results.AddRange(level.GetEntities(EntityTypes.ENEMY).Select(e => e.GetMainArmor()).Where(e => e != null));
            }
        }
    }
}
