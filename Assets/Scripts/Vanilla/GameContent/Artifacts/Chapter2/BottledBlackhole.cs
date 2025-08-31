using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.bottledBlackhole)]
    public class BottledBlackhole : ArtifactDefinition
    {
        public BottledBlackhole(string nsp, string name) : base(nsp, name)
        {
            AddAura(new DamageAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            var level = artifact.Level;
            artifact.SetGlowing(!level.IsConveyorMode() && level.GetSeedSlotCount() > level.GetSeedPackCount());
        }
        public class DamageAura : AuraEffectDefinition
        {
            public DamageAura() : base(VanillaBuffID.Contraption.bottledBlackholeDamage)
            {
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.PLANT));
            }
            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                var level = effect.Level;
                if (level.IsConveyorMode())
                    return;
                var count = level.GetSeedSlotCount() - level.GetSeedPackCount();
                BottledBlackholeDamageBuff.SetDamageMultiplier(buff, count * DAMAGE_MULTIPLIER);
            }
        }
        public const float DAMAGE_MULTIPLIER = 0.15f;
    }
}
