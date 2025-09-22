using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.BerserkerHead)]
    public class BerserkerHead : MeleeEnemy
    {
        public BerserkerHead(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_APPLY_STATUS_EFFECT, PostEntityCharmCallback, filter: VanillaBuffID.Entity.charm);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 20f);
        }
        public override void PreTakeDamage(DamageInput input, CallbackResult result)
        {
            base.PreTakeDamage(input, result);
            if (input.Effects.HasEffect(VanillaDamageEffects.GOLD))
            {
                input.Multiply(3);
            }
        }
        private void PostEntityCharmCallback(VanillaLevelCallbacks.PostApplyStatusEffectParams param, CallbackResult result)
        {
            var entity = param.entity;
            var buff = param.buff;
            if (!entity.IsEntityOf(VanillaEnemyID.BerserkerHead))
                return;
            var body = GetBody(entity);
            if (!body.ExistsAndAlive())
                return;
            CharmBuff.CloneCharm(buff, body);
        }
        public static Entity GetBody(Entity entity)
        {
            var entityID = entity.GetBehaviourField<EntityID>(FIELD_BODY);
            return entityID?.GetEntity(entity.Level);
        }
        public static void SetBody(Entity entity, Entity value)
        {
            entity.SetBehaviourField(FIELD_BODY, new EntityID(value));
        }

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            Explode(entity, entity.GetDamage() * 6, entity.GetFaction());
            entity.Remove();
        }
        public static void Explode(Entity entity, float damage, int faction)
        {
            var scale = entity.GetFinalScale();
            var scaleX = Mathf.Abs(scale.x);
            var range = entity.GetRange() * scaleX;
            entity.Explode(entity.GetCenter(), range, faction, damage, new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE));

            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion, scaleX == 0 ? 1000 : 1 / (scaleX));
        }

        public static readonly VanillaEntityPropertyMeta<EntityID> FIELD_BODY = new VanillaEntityPropertyMeta<EntityID>("Body");
    }
}
