using System.Collections.Generic;
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
            AddModifier(new IntModifier(VanillaLevelProps.ENEMY_AI_LEVEL, NumberOperator.Add, 1));
            AddModifier(new IntModifier(VanillaLevelProps.BOSS_AI_LEVEL, NumberOperator.Add, 1));
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_POWER, NumberOperator.AddMultiplie, 0.2f));
            AddModifier(new FloatModifier(VanillaLevelProps.NAPSTABLOOK_PARALYSIS_TIME, NumberOperator.Multiply, 2));
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
