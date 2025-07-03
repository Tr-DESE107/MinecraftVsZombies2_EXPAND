using System.Collections.Generic;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.levelHard)]
    public class LevelHardBuff : BuffDefinition
    {
        public LevelHardBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaLevelProps.NO_CARTS, true));

            AddModifier(new FloatModifier(VanillaDifficultyProps.GUNPOWDER_DAMAGE_MULTIPLIER, NumberOperator.Multiply, 20));

            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_POWER, NumberOperator.AddMultiple, 0.2f));
            AddModifier(new IntModifier(VanillaDifficultyProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Multiply, 2));
            AddModifier(new FloatModifier(VanillaDifficultyProps.GHAST_DAMAGE_MULTIPLIER, NumberOperator.Add, 1f));
            AddModifier(new IntModifier(VanillaDifficultyProps.MOTHER_TERROR_EGG_COUNT, NumberOperator.Add, 1));
            AddModifier(new IntModifier(VanillaDifficultyProps.PARASITIZED_TERROR_COUNT, NumberOperator.Add, 1));
            AddModifier(new FloatModifier(VanillaDifficultyProps.REVERSE_SATELLITE_DAMAGE_MULTIPLIER, NumberOperator.AddMultiple, 1f));
            AddModifier(new IntModifier(VanillaDifficultyProps.SKELETON_HORSE_JUMP_TIMES, NumberOperator.Add, 1));
            AddModifier(new IntModifier(VanillaDifficultyProps.WICKED_HERMIT_ZOMBIE_STUN_TIME, NumberOperator.Add, -75));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.FRANKENSTEIN_INSTANT_STEEL, true));
            AddModifier(new FloatModifier(VanillaDifficultyProps.FRANKENSTEIN_SPEED, NumberOperator.Multiply, 2));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.SLENDERMAN_MIND_SWAP_ZOMBIES, true));
            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_FATE_CHOICE_COUNT, NumberOperator.Add, -1));
            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_MAX_FATE_TIMES, NumberOperator.Add, 1));

            AddModifier(new FloatModifier(VanillaDifficultyProps.CRUSHING_WALLS_SPEED, NumberOperator.Add, 1f));
            AddModifier(new FloatModifier(VanillaDifficultyProps.NIGHTMAREAPER_SPIN_DAMAGE, NumberOperator.Add, 10));
            AddModifier(new IntModifier(VanillaDifficultyProps.NIGHTMAREAPER_TIMEOUT, NumberOperator.Add, -900));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.WITHER_SKULL_WITHERS_TARGET, true));
            AddModifier(new BooleanModifier(VanillaDifficultyProps.THE_GIANT_IS_MALLEABLE, true));
            AddAura(new EnemyAura());
        }

        public class EnemyAura : AuraEffectDefinition
        {
            public EnemyAura() : base()
            {
                BuffID = VanillaBuffID.hardEnemy;
                UpdateInterval = 30;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.ENEMY));
            }
        }
    }
}
