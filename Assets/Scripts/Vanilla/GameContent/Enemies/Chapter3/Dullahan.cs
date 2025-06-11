using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.dullahan)]
    public class Dullahan : MeleeEnemy
    {
        public Dullahan(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_CHARM, PostEntityCharmCallback);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.ChangeModel(VanillaModelID.dullahanMain);
            var param = entity.GetSpawnParams();
            if (entity.IsPreviewEnemy())
            {
                param.SetProperty(VanillaEnemyProps.PREVIEW_ENEMY, true);
            }
            var horse = entity.Spawn(VanillaEnemyID.skeletonHorse, entity.Position, param);
            entity.RideOn(horse);
            entity.SetAnimationBool("Sitting", true);
            entity.SetAnimationBool("HoldingHead", !IsHeadDropped(entity));
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
            if (!entity.IsEntityOf(VanillaEnemyID.dullahan))
                return;
            var head = GetHead(entity);
            if (!head.ExistsAndAlive())
                return;
            CharmBuff.CloneCharm(buff, head);
        }
        protected override int GetActionState(Entity enemy)
        {
            var baseState = base.GetActionState(enemy);
            if (baseState == VanillaEntityStates.WALK)
            {
                var horse = enemy.GetRidingEntity();
                var hasHorse = horse.ExistsAndAlive();
                if (hasHorse)
                {
                    return STATE_IDLE;
                }
            }
            return baseState;
        }
        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);
            var horse = enemy.GetRidingEntity();
            if (horse == null)
            {
                DropHead(enemy);
            }
            else if (horse.IsEntityOf(VanillaEnemyID.skeletonHorse))
            {
                if (horse.State == SkeletonHorse.STATE_JUMP)
                {
                    DropHead(enemy);
                }
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            var horse = entity.GetRidingEntity();
            var hasHorse = horse.ExistsAndAlive();
            entity.SetAnimationBool("Sitting", hasHorse);
            entity.SetAnimationBool("HoldingHead", !IsHeadDropped(entity));
            entity.SetModelDamagePercent();
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                return;
            }
            DropHead(entity);
        }
        public static Entity DropHead(Entity entity)
        {
            if (IsHeadDropped(entity))
                return null;
            var head = entity.SpawnWithParams(VanillaEnemyID.dullahanHead, entity.GetCenter());
            SetHead(entity, head);
            DullahanHead.SetBody(head, entity);
            SetHeadDropped(entity, true);
            return head;
        }

        public static bool IsHeadDropped(Entity entity) => entity.GetBehaviourField<bool>(ID, FIELD_HEAD_DROPPED);
        public static void SetHeadDropped(Entity entity, bool value) => entity.SetBehaviourField(ID, FIELD_HEAD_DROPPED, value);
        public static Entity GetHead(Entity entity)
        {
            var entityID = entity.GetBehaviourField<EntityID>(ID, FIELD_HEAD);
            return entityID?.GetEntity(entity.Level);
        }
        public static void SetHead(Entity entity, Entity value)
        {
            entity.SetBehaviourField(ID, FIELD_HEAD, new EntityID(value));
        }

        public static readonly VanillaEntityPropertyMeta<EntityID> FIELD_HEAD = new VanillaEntityPropertyMeta<EntityID>("Head");
        public static readonly VanillaEntityPropertyMeta<bool> FIELD_HEAD_DROPPED = new VanillaEntityPropertyMeta<bool>("HeadDropped");
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        private static readonly NamespaceID ID = VanillaEnemyID.dullahan;
    }
}
