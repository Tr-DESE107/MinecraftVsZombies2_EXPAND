using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.note)]
    public class Note : ProjectileBehaviour
    {
        public Note(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_DISPLAY_SCALE_MULTIPLIER));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_SCALE, NumberOperator.Multiply, PROP_DISPLAY_SCALE_MULTIPLIER));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskFriendly |= EntityCollisionHelper.MASK_PLANT;
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            var dmg = projectile.GetDamage();
            projectile.SetProperty(PROP_DISPLAY_SCALE_MULTIPLIER, Vector3.one * Mathf.Min(5, Mathf.Pow(dmg / 10f, 0.5f)));
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var note = hitResult.Projectile;
            Reflect(note, hitResult.Other);
            var dmg = note.GetDamage(true);
            dmg--;
            note.SetDamage(dmg);
            if (dmg <= 0)
            {
                note.Remove();
            }
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            var note = collision.Entity;
            var noteBlock = collision.Other;
            if (!noteBlock.IsEntityOf(VanillaContraptionID.noteBlock))
                return;
            if (!collision.OtherCollider.IsMainCollider())
                return;
            if (note.IsProjectileColliderIgnored(collision.OtherCollider))
                return;
            if (note.Parent != noteBlock || !note.IsFriendly(noteBlock))
                return;
            note.SetDamage(noteBlock.GetDamage());
            note.Velocity = noteBlock.GetFacingDirection() * noteBlock.GetShotVelocity().magnitude;
            note.AddIgnoredProjectileCollider(collision.OtherCollider);
            noteBlock.TriggerAnimation("Shoot");
            NoteBlock.PlayHarpSound(noteBlock);
        }
        public static void Reflect(Entity note, Entity other)
        {
            var vel = note.Velocity;
            var magnitude = vel.magnitude;
            if (other.Position.x < note.PreviousPosition.x)
            {
                vel = Vector3.right * magnitude;
            }
            else
            {
                vel = Vector3.left * magnitude;
            }
            note.Velocity = vel;
        }
        private static readonly VanillaEntityPropertyMeta PROP_DISPLAY_SCALE_MULTIPLIER = new VanillaEntityPropertyMeta("DisplayScaleMultiplier");
    }
}
