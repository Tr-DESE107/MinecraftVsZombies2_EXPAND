using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Artifacts;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.Models;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.gravityPad)]
    public class GravityPad : ContraptionBehaviour
    {
        public GravityPad(string nsp, string name) : base(nsp, name)
        {
            AddAura(new GravityAura());
            projectileDetector = new GravityPadDetector(false, AFFECT_HEIGHT);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.carriers);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var level = entity.Level;
            var minY = entity.Position.y + MIN_HEIGHT;
            detectBuffer.Clear();
            projectileDetector.DetectEntities(entity, detectBuffer);
            foreach (var projectile in detectBuffer)
            {
                Vector3 pos = projectile.Position;
                pos.y = Mathf.Max(pos.y + PULL_DOWN_SPEED, minY);
                projectile.Position = pos;
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationBool("IsOn", !entity.IsAIFrozen());
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var pos = entity.Position + Vector3.up * 600;
            var level = entity.Level;
            if (level.AreaID == VanillaAreaID.castle && !Global.Game.IsUnlocked(VanillaUnlockID.brokenLantern))
            {
                if (!level.EntityExists(e => e.IsEntityOf(VanillaPickupID.artifactPickup) && ArtifactPickup.GetArtifactID(e) == VanillaArtifactID.brokenLantern))
                {
                    var lantern = level.Spawn(VanillaPickupID.artifactPickup, pos + Vector3.up * 100, entity);
                    ArtifactPickup.SetArtifactID(lantern, VanillaArtifactID.brokenLantern);
                }
            }
            var anvil = level.Spawn(VanillaContraptionID.anvil, pos, entity);
            anvil.SetFactionAndDirection(entity.GetFaction());
        }
        public const float AFFECT_HEIGHT = 64;
        public const float MIN_HEIGHT = 5;
        public const float PULL_DOWN_SPEED = -3.333f;
        private Detector projectileDetector;
        private List<Entity> detectBuffer = new List<Entity>();

        public class GravityAura : AuraEffectDefinition
        {
            public GravityAura()
            {
                BuffID = VanillaBuffID.gravityPadGravity;
                UpdateInterval = 7;
                enemyDetector = new GravityPadDetector(true, AFFECT_HEIGHT);
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source;
                var entity = source.GetEntity();
                if (entity == null)
                    return;
                detectBuffer.Clear();
                enemyDetector.DetectEntities(entity, detectBuffer);
                results.AddRange(detectBuffer);
            }
            private Detector enemyDetector;
            private List<Entity> detectBuffer = new List<Entity>();
        }
    }
}
