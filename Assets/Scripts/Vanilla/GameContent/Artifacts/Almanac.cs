using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.almanac)]
    public class Almanac : ArtifactDefinition
    {
        public Almanac(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
        }
        private void PostLevelStartCallback(LevelEngine level)
        {
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact?.Definition?.GetID() != ID)
                    continue;
                artifact.Highlight();
                level.AddStarshardCount(1);
                level.PlaySound(VanillaSoundID.starshardUse);
            }
        }
        public static readonly NamespaceID ID = VanillaArtifactID.almanac;
    }
}
