using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.witherSkull)]
    public class WitherSkull : ProjectileBehaviour
    {
        public WitherSkull(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEntityTakeDamageCallback);
        }
        public override void Init(Entity projectile)
        {
            base.Init(projectile);
            projectile.SetModelProperty("Source", projectile.Position);
            projectile.SetModelProperty("Dest", projectile.Position + projectile.Velocity);
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.SetModelProperty("Source", projectile.Position);
            projectile.SetModelProperty("Dest", projectile.Position + projectile.Velocity);
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
