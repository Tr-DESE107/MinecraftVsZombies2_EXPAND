﻿using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.almanac)]
    public class Almanac : ArtifactDefinition
    {
        public Almanac(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
        }
        private void PostLevelStartCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact?.Definition?.GetID() != ID)
                    continue;
                var enemies = level.GetEnemyPool();
                if (enemies == null)
                    continue;
                var energy = enemies.Length / 2 * 25f;
                if (energy <= 0)
                    continue;
                artifact.Highlight();
                level.AddEnergy(energy);
                level.PlaySound(VanillaSoundID.points);
            }
        }
        public static readonly NamespaceID ID = VanillaArtifactID.almanac;
    }
}
