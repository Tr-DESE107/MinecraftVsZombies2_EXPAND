﻿using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.netherStar)]
    public class NetherStar : ArtifactDefinition
    {
        public NetherStar(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_USE_STARSHARD, PostUseStarshardCallback);
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            var number = artifact.GetNumber();
            if (number < 0)
            {
                number = 0;
                artifact.SetNumber(number);
            }
            artifact.SetGlowing(number == MAX_NUMBER - 1);
        }
        private void PostUseStarshardCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition != this)
                    continue;
                var number = artifact.GetNumber();
                number++;
                if (number >= MAX_NUMBER)
                {
                    artifact.Highlight();
                    number -= MAX_NUMBER;
                    contraption.Spawn(VanillaPickupID.starshard, contraption.Position);
                }
                artifact.SetNumber(number);
            }
        }
        private const int MAX_NUMBER = 4;
    }
}
