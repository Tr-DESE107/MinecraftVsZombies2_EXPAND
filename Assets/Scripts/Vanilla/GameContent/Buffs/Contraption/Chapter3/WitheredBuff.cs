using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Level;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.withered)]
    public class WitheredBuff : BuffDefinition
    {
        public WitheredBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.witherParticles, VanillaModelID.witherParticles);
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEntityTakeDamageCallback);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null)
            {
                entity.TakeDamage(WITHER_DAMAGE, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.MUTE), entity);
            }

            if (entity.Health <= 1)
            {
                Explode(entity);
                entity.Remove();
            }
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public const float WITHER_DAMAGE = 1 / 3f;

        public static void Explode(Entity entity)
        {
            var range = entity.GetRange();
            entity.PlaySound(VanillaSoundID.explosion);
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            var damageEffects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
            entity.Explode(entity.Position, 120, VanillaFactions.NEUTRAL, 30, damageEffects);


        }

        private void PostEntityTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            if (output == null)
                return;
            var entity = output.Entity;
            if (entity == null)
                return;
            if (!entity.Level.WitherSkullWithersTarget())
                return;
            if (entity.IsUndead())
                return;
            if (output.BodyResult == null)
                return;
            if (output.BodyResult.Amount <= 0)
                return;
            var source = output.BodyResult.Source;
            if (source != null && source.DefinitionID == GetID())
            {
                entity.InflictWither(WITHER_TIME);
            }
        }
        public const int WITHER_TIME = 900;

    }

}
