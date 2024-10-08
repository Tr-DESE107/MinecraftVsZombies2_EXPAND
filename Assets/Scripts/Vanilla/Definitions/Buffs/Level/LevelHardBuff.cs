using System.Collections.Generic;
using MVZ2.GameContent;
using PVZEngine.Auras;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(VanillaBuffNames.Level.levelHard)]
    public class LevelHardBuff : BuffDefinition
    {
        public LevelHardBuff(string nsp, string name) : base(nsp, name)
        {
            AddAura(new EnemyAura());
        }

        public class EnemyAura : AuraEffectDefinition
        {
            public EnemyAura() : base()
            {
                BuffID = VanillaBuffID.hardEnemy;
                UpdateInterval = 30;
            }

            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                return level.GetEntities(EntityTypes.ENEMY);
            }

            public override bool CheckCondition(AuraEffect effect, IBuffTarget entity)
            {
                return true;
            }
        }
    }
}
