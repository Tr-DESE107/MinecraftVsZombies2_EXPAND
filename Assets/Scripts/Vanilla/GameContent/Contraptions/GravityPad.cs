using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
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
            foreach (var projectile in level.FindEntities(e => ValidatePullDown(entity, e)))
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
            entity.Level.Spawn(VanillaContraptionID.anvil, pos, entity);
        }
        public static bool ValidatePullDown(Entity pad, Entity entity)
        {
            if (entity.Type != EntityTypes.PROJECTILE)
                return false;
            if (!pad.IsFriendly(entity))
                return false;
            return IsOver(pad, entity) && entity.Position.y > pad.Position.y + MIN_HEIGHT;
        }

        public static bool ValidateEnemy(Entity pad, Entity entity)
        {
            if (entity.Type != EntityTypes.ENEMY)
                return false;
            if (!pad.IsHostile(entity))
                return false;
            return IsOver(pad, entity);
        }
        private static bool IsOver(Entity pad, Entity entity)
        {
            var padSize = pad.GetScaledSize();
            var entitySize = entity.GetScaledSize();
            return Detection.IsXCoincide(pad.Position.x, padSize.x, entity.Position.x, entitySize.x) &&
                Detection.IsYCoincide(pad.Position.y, AFFECT_HEIGHT, entity.Position.y, entitySize.y) &&
                Detection.IsZCoincide(pad.Position.z, padSize.z, entity.Position.z, entitySize.z);
        }
        public const float AFFECT_HEIGHT = 64;
        public const float MIN_HEIGHT = 5;
        public const float PULL_DOWN_SPEED = -3.333f;

        public class GravityAura : AuraEffectDefinition
        {
            public GravityAura()
            {
                BuffID = VanillaBuffID.gravityPadGravity;
                UpdateInterval = 7;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source;
                var entity = source.GetEntity();
                if (entity == null)
                    return;
                results.AddRange(entity.Level.FindEntities(e => ValidateEnemy(entity, e)));
            }
        }
    }
}
