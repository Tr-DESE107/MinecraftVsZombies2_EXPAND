using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent;
using PVZEngine.Auras;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(VanillaBuffNames.Level.levelEasy)]
    public class LevelEasyBuff : BuffDefinition
    {
        public LevelEasyBuff(string nsp, string name) : base(nsp, name)
        {
            AddAura(new BlueprintAura());
            AddAura(new ContraptionAura());
            AddAura(new ArmorAura());
        }

        public class BlueprintAura : AuraEffectDefinition
        {
            public BlueprintAura() : base()
            {
                BuffID = VanillaBuffID.SeedPack.easyBlueprint;
                UpdateInterval = 30;
            }

            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                return level.GetAllSeedPacks();
            }

            public override bool CheckCondition(AuraEffect effect, IBuffTarget entity)
            {
                return true;
            }
        }
        public class ContraptionAura : AuraEffectDefinition
        {
            public ContraptionAura() : base()
            {
                BuffID = VanillaBuffID.easyContraption;
                UpdateInterval = 4;
            }

            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                return level.GetEntities(EntityTypes.PLANT);
            }

            public override bool CheckCondition(AuraEffect effect, IBuffTarget entity)
            {
                return true;
            }
        }
        public class ArmorAura : AuraEffectDefinition
        {
            public ArmorAura() : base()
            {
                BuffID = VanillaBuffID.easyArmor;
            }

            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                return level.GetEntities(EntityTypes.ENEMY).Select(e => e.EquipedArmor).Where(e => e != null);
            }

            public override bool CheckCondition(AuraEffect effect, IBuffTarget entity)
            {
                return true;
            }
        }
    }
}
