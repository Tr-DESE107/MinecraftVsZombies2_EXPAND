using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Difficulties;
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
            AddModifier(new FloatModifier(VanillaLevelProps.CONVEY_SPEED, NumberOperator.Multiply, 1.5f));

            AddModifier(new FloatModifier(VanillaDifficultyProps.GUNPOWDER_DAMAGE_MULTIPLIER, NumberOperator.Multiply, 0.66666666666f));

            AddModifier(new IntModifier(VanillaDifficultyProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Add, -22));
            AddModifier(new IntModifier(VanillaDifficultyProps.MOTHER_TERROR_EGG_COUNT, NumberOperator.Add, -1));
            AddModifier(new IntModifier(VanillaDifficultyProps.PARASITIZED_TERROR_COUNT, NumberOperator.Add, -1));
            AddModifier(new FloatModifier(VanillaDifficultyProps.REVERSE_SATELLITE_DAMAGE_MULTIPLIER, NumberOperator.AddMultiple, -1f));
            AddModifier(new IntModifier(VanillaDifficultyProps.SKELETON_HORSE_JUMP_TIMES, NumberOperator.Add, -1));
            AddModifier(new IntModifier(VanillaDifficultyProps.WICKED_HERMIT_ZOMBIE_STUN_TIME, NumberOperator.Add, 75));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.FRANKENSTEIN_NO_STEEL, true));

            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_FATE_CHOICE_COUNT, NumberOperator.Add, 1));
            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_MAX_FATE_TIMES, NumberOperator.Add, -1));

            AddModifier(new FloatModifier(VanillaDifficultyProps.CRUSHING_WALLS_SPEED, NumberOperator.Add, -1f));
            AddModifier(new FloatModifier(VanillaDifficultyProps.NIGHTMAREAPER_SPIN_DAMAGE, NumberOperator.Add, -10));
            AddModifier(new IntModifier(VanillaDifficultyProps.NIGHTMAREAPER_TIMEOUT, NumberOperator.Add, 900));

            AddModifier(new FloatModifier(VanillaDifficultyProps.WITHER_REGENERATION, NumberOperator.Set, 0));

            AddModifier(new FloatModifier(VanillaDifficultyProps.STARSHARD_CARRIER_CHANCE_INCREAMENT, NumberOperator.Multiply, 2f));
            AddModifier(new FloatModifier(VanillaDifficultyProps.REDSTONE_CARRIER_CHANCE_INCREAMENT, NumberOperator.Multiply, 2f));
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
