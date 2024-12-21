using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.dreamKey)]
    public class DreamKey : ArtifactDefinition
    {
        public DreamKey(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, PostContraptionEvokeCallback);
            AddAura(new EvokedContraptionInvincibleAura());
        }
        private void PostContraptionEvokeCallback(Entity contraption)
        {
            var level = contraption.Level;
            if (!level.HasArtifact(ID))
                return;
            contraption.HealEffects(contraption.GetMaxHealth(), (Entity)null);
            var artifact = level.GetArtifact(ID);
            artifact.Highlight();
        }
        public static readonly NamespaceID ID = VanillaArtifactID.dreamKey;

        public class EvokedContraptionInvincibleAura : AuraEffectDefinition
        {
            public EvokedContraptionInvincibleAura()
            {
                BuffID = VanillaBuffID.dreamKeyShield;
                UpdateInterval = 7;
            }
            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                return level.GetEntities(EntityTypes.PLANT);
            }
            public override bool CheckCondition(AuraEffect effect, IBuffTarget target)
            {
                var entity = target.GetEntity();
                if (entity == null)
                    return false;
                return entity.IsEvoked();
            }
        }
    }
}
