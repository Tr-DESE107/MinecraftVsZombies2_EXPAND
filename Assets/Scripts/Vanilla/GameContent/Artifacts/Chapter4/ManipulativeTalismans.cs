using MVZ2.Vanilla.Artifacts;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.manipulativeTalismans)]
    public class ManipulativeTalismans : ArtifactDefinition
    {
        public ManipulativeTalismans(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_ENEMY_FAINT, PreEnemyFaintCallback, priority: -100);
        }
        public override void PostAdd(Artifact artifact)
        {
            base.PostAdd(artifact);
            artifact.SetRNG(artifact.Level.CreateRNG());
        }
        private void PreEnemyFaintCallback(EntityCallbackParams param, CallbackResult result)
        {
            var enemy = param.entity;
            if (!enemy.IsHostileEntity())
                return;
            var level = enemy.Level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact?.Definition != this)
                    continue;
                var rng = artifact.GetRNG();
                if (rng.Next(100) < REVIVE_CHANCE)
                {
                    enemy.Revive();
                    enemy.Health = enemy.GetMaxHealth();
                    enemy.Charm(level.Option.LeftFaction);
                    result.SetFinalValue(false);
                    artifact.Highlight();
                    enemy.PlaySound(VanillaSoundID.revived);
                    return;
                }
            }
        }
        public const int REVIVE_CHANCE = 10;
    }
}
