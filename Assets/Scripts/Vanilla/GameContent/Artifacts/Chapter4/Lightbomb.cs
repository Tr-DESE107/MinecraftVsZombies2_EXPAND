using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.lightbomb)]
    public class Lightbomb : ArtifactDefinition
    {
        public Lightbomb(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_DETONATE, PostContraptionDetonateCallback);
        }
        public void PostContraptionDetonateCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact?.Definition != this)
                    continue;
                artifact.Highlight();

                foreach (var enemy in level.FindEntities(e => e.IsVulnerableEntity() && e.IsHostile(contraption)))
                {
                    enemy.TakeDamage(LIGHT_DAMAGE, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.MUTE), contraption);
                }
                contraption.Spawn(VanillaEffectID.stunningFlash, contraption.GetCenter());
                contraption.PlaySound(VanillaSoundID.evocation);
            }
        }
        public const float LIGHT_DAMAGE = 170;
    }
}
