using MVZ2.GameContent.Areas;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.largeSnowball)]
    public class LargeSnowball : ProjectileBehaviour
    {
        public LargeSnowball(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEnemyTakeDamageCallback);
            SetProperty(VanillaEntityProps.WATER_INTERACTION, WaterInteraction.DROWN);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetSnowballScale(entity, 1f);
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);

            projectile.Velocity += projectile.Velocity.normalized / 3f;
            var scale = GetSnowballScale(projectile);
            scale = Mathf.Clamp(scale + SCALE_SPEED, MIN_SCALE, MAX_SCALE);
            SetSnowballScale(projectile, scale);

            float angleSpeed = -projectile.Velocity.x * 2.5f;
            projectile.RenderRotation += Vector3.forward * angleSpeed;

            Vector3 scaleVector = new Vector3(scale, scale, 1);
            projectile.SetScale(scaleVector);
            projectile.SetDisplayScale(scaleVector);
            projectile.SetShadowScale(scaleVector * 0.5f);
            projectile.SetDamage(Mathf.Max(0, (scale - 1) * 300));
        }
        private void PostEnemyTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            var bodyResult = output.BodyResult;
            if (bodyResult == null)
                return;
            var entity = bodyResult.Entity;
            var source = bodyResult.Source?.GetEntity(entity.Level);
            if (source == null)
                return;
            if (bodyResult.Fatal && source.IsEntityOf(VanillaProjectileID.largeSnowball))
            {
                source.PlaySound(VanillaSoundID.grind);
            }
        }
        public static float GetSnowballScale(Entity entity) => entity.GetBehaviourField<float>(ID, PROP_SNOWBALL_SCALE);
        public static void SetSnowballScale(Entity entity, float scale) => entity.SetBehaviourField(ID, PROP_SNOWBALL_SCALE, scale);

        private static readonly NamespaceID ID = VanillaAreaID.halloween;
        public static readonly VanillaEntityPropertyMeta<float> PROP_SNOWBALL_SCALE = new VanillaEntityPropertyMeta<float>("SnowballScale");
        public const float MIN_SCALE = 1;
        public const float SCALE_SPEED = 0.1f;
        public const float MAX_SCALE = 10;
    }
}
