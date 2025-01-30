using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.invertedMirror)]
    public class InvertedMirror : ArtifactDefinition
    {
        public InvertedMirror(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_SHOT, PostProjectileShotCallback);
        }
        private void PostProjectileShotCallback(Entity projectile)
        {
            if (!projectile.IsHostileEntity())
                return;
            var level = projectile.Level;
            var artifacts = level.GetArtifacts();
            bool valid = false;
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition != this)
                    continue;
                artifact.Highlight();
                valid = true;
            }
            if (valid)
            {
                projectile.AddBuff<InvertedMirrorBuff>();
            }
        }
    }
}
