using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.dullahanHead)]
    public class DullahanHead : MeleeEnemy
    {
        public DullahanHead(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_CHARM, PostEntityCharmCallback);
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
        private void PostEntityCharmCallback(VanillaLevelCallbacks.PostEntityCharmParams param, CallbackResult result)
        {
            var entity = param.entity;
            var buff = param.buff;
            if (!entity.IsEntityOf(VanillaEnemyID.dullahanHead))
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
        public static readonly VanillaEntityPropertyMeta<EntityID> FIELD_BODY = new VanillaEntityPropertyMeta<EntityID>("Body");
    }
}
