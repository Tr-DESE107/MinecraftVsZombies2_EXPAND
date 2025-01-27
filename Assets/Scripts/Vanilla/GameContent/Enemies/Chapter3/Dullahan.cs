using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.dullahan)]
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
            var horse = entity.Spawn(VanillaEnemyID.skeletonHorse, entity.Position);
            entity.RideOn(horse);
        }
        public override void PreTakeDamage(DamageInput input)
        {
            base.PreTakeDamage(input);
            if (input.Effects.HasEffect(VanillaDamageEffects.GOLD))
            {
                input.Multiply(3);
            }
        }
        private void PostEntityCharmCallback(Entity entity, Buff buff)
        {
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
            var head = entity.Spawn(VanillaEnemyID.dullahanHead, entity.GetCenter());
            head.SetFactionAndDirection(entity.GetFaction());
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

        public const string FIELD_HEAD = "Head";
        public const string FIELD_HEAD_DROPPED = "HeadDropped";
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        private static readonly NamespaceID ID = VanillaEnemyID.dullahan;
    }
}
