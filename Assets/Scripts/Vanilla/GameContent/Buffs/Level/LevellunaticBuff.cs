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
    [BuffDefinition(VanillaBuffNames.Level.levellunatic)]
    public class LevelLunaticBuff : BuffDefinition
    {
        public LevelLunaticBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaLevelProps.NO_CARTS, true));
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_POWER, NumberOperator.AddMultiple, 0.5f));

            AddModifier(new IntModifier(VanillaDifficultyProps.REDSTONE_ORE_DROP_COUNT, NumberOperator.Add, -4));
            AddModifier(new FloatModifier(VanillaDifficultyProps.GHOST_TAKEN_DAMAGE_MULTIPLIER, NumberOperator.Multiply, 0));
            AddModifier(new IntModifier(VanillaDifficultyProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Set, 120));
            AddModifier(new FloatModifier(VanillaDifficultyProps.GHAST_DAMAGE_MULTIPLIER, NumberOperator.AddMultiple, 1f));
            AddModifier(new IntModifier(VanillaDifficultyProps.MOTHER_TERROR_EGG_COUNT, NumberOperator.Add, 2));
            AddModifier(new IntModifier(VanillaDifficultyProps.PARASITIZED_TERROR_COUNT, NumberOperator.Add, 1));
            AddModifier(new FloatModifier(VanillaDifficultyProps.REVERSE_SATELLITE_DAMAGE_MULTIPLIER, NumberOperator.AddMultiple, 1f));
            AddModifier(new IntModifier(VanillaDifficultyProps.SKELETON_HORSE_JUMP_TIMES, NumberOperator.Add, 1));
            AddModifier(new IntModifier(VanillaDifficultyProps.WICKED_HERMIT_ZOMBIE_STUN_TIME, NumberOperator.Add, -150));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.FRANKENSTEIN_INSTANT_STEEL, true));
            AddModifier(new FloatModifier(VanillaDifficultyProps.FRANKENSTEIN_SPEED, NumberOperator.Multiply, 2));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.SLENDERMAN_MIND_SWAP_ZOMBIES, true));
            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_FATE_CHOICE_COUNT, NumberOperator.Add, -1));
            AddModifier(new IntModifier(VanillaDifficultyProps.SLENDERMAN_MAX_FATE_TIMES, NumberOperator.Add, 1));

            AddModifier(new FloatModifier(VanillaDifficultyProps.CRUSHING_WALLS_SPEED, NumberOperator.Add, 2f));
            AddModifier(new FloatModifier(VanillaDifficultyProps.NIGHTMAREAPER_SPIN_DAMAGE, NumberOperator.Add, 10));
            AddModifier(new IntModifier(VanillaDifficultyProps.NIGHTMAREAPER_TIMEOUT, NumberOperator.Add, -1500));

            AddModifier(new BooleanModifier(VanillaDifficultyProps.WITHER_SKULL_WITHERS_TARGET, true));
            AddModifier(new BooleanModifier(VanillaDifficultyProps.THE_GIANT_IS_MALLEABLE, true));
            AddAura(new EnemyAura());
        }

        public class EnemyAura : AuraEffectDefinition
        {
            public EnemyAura() : base()
            {
                BuffID = VanillaBuffID.lunaticEnemy;
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
