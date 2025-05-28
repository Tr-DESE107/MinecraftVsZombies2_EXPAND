using System.Collections.Generic;
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
            AddModifier(new IntModifier(VanillaLevelProps.ENEMY_AI_LEVEL, NumberOperator.Add, 2));
            AddModifier(new IntModifier(VanillaLevelProps.BOSS_AI_LEVEL, NumberOperator.Add, 2));
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_POWER, NumberOperator.AddMultiplie, 0.5f));
            AddModifier(new FloatModifier(VanillaLevelProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Multiply, 2.666667f));
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
