using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    public abstract class UndeadFlyingObject : StateEnemy
    {
        public UndeadFlyingObject(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(GetStayTime()));
            var column = entity.GetColumn();
            var lane = entity.GetLane();
            SetTargetGridX(entity, column);
            SetTargetGridY(entity, lane);

            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, FLY_HEIGHT);
            buff.SetProperty(FlyBuff.PROP_FLY_SPEED_FACTOR, FLY_SPEED_FACTOR_ENTER);
            buff.SetProperty(FlyBuff.PROP_FLY_SPEED, FLY_SPEED_ENTER);
            buff.SetProperty(FlyBuff.PROP_MAX_FLY_SPEED, MAX_FLY_SPEED);

            var position = entity.Position;
            position.y = START_HEIGHT + entity.Level.GetGroundY(position.x, position.z);
            entity.Position = position;

            entity.PlaySound(VanillaSoundID.ufo);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.IsOnGround)
            {
                var effects = new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE);
                entity.Die(effects, entity);
            }
            entity.SetAnimationBool("SpotlightOn", entity.State == STATE_ACT);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (!info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH) && !info.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
            {
                float damageMutliplier = entity.Level.GetReverseSatelliteDamageMultiplier();
                float radius = entity.GetRange();
                var damage = entity.GetDamage() * damageMutliplier;
                if (damage >= 0)
                {
                    entity.Explode(entity.GetCenter(), radius, entity.GetFaction(), damage, new DamageEffectList(VanillaDamageEffects.EXPLOSION));
                }
                var param = entity.GetSpawnParams();
                param.SetProperty(EngineEntityProps.SIZE, entity.GetScaledSize());
                var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), param);
                entity.PlaySound(VanillaSoundID.explosion);
            }
            entity.Remove();
        }
        protected override int GetActionState(Entity enemy)
        {
            var baseState = base.GetActionState(enemy);
            if (baseState != STATE_IDLE && baseState != STATE_DEATH)
            {
                return GetUFOState(enemy);
            }
            return baseState;
        }
        protected override void UpdateActionState(Entity enemy, int state)
        {
            base.UpdateActionState(enemy, state);
            switch (state)
            {
                case STATE_STAY:
                    UpdateStateStay(enemy);
                    break;
                case STATE_ACT:
                    UpdateStateAct(enemy);
                    break;
                case STATE_LEAVE:
                    UpdateStateLeave(enemy);
                    break;
            }
        }
        protected virtual void UpdateStateStay(Entity enemy)
        {
            FlyToTargetPosition(enemy);
        }
        protected virtual void UpdateStateAct(Entity enemy)
        {
            FlyToTargetPosition(enemy);
        }
        private void UpdateStateLeave(Entity enemy)
        {
            Leave(enemy);
        }
        private void FlyToTargetPosition(Entity enemy)
        {
            foreach (var buff in enemy.GetBuffs<FlyBuff>())
            {
                buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, FLY_HEIGHT);
                buff.SetProperty(FlyBuff.PROP_FLY_SPEED_FACTOR, FLY_SPEED_FACTOR_ENTER);
                buff.SetProperty(FlyBuff.PROP_FLY_SPEED, FLY_SPEED_ENTER);
            }

            var targetPosition = GetTargetPosition(enemy);
            var targetVelocity = targetPosition - enemy.Position;
            var velocity = enemy.Velocity;
            velocity.x = velocity.x * 0.5f + targetVelocity.x * 0.5f;
            velocity.z = velocity.z * 0.5f + targetVelocity.z * 0.5f;
            enemy.Velocity = velocity;
        }
        private void Leave(Entity enemy)
        {
            foreach (var buff in enemy.GetBuffs<FlyBuff>())
            {
                buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, LEAVE_HEIGHT);
                buff.SetProperty(FlyBuff.PROP_FLY_SPEED_FACTOR, FLY_SPEED_FACTOR_LEAVE);
                buff.SetProperty(FlyBuff.PROP_FLY_SPEED, FLY_SPEED_LEAVE);
            }

            if (enemy.GetRelativeY() >= LEAVE_HEIGHT)
            {
                enemy.Remove();
            }
        }
        private Vector3 GetTargetPosition(Entity enemy)
        {
            var level = enemy.Level;
            var column = GetTargetGridX(enemy);
            var lane = GetTargetGridY(enemy);
            var x = level.GetEntityColumnX(column);
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundY(x, z) + FLY_HEIGHT;
            return new Vector3(x, y, z);
        }
        public abstract int GetStayTime();
        public abstract int GetActTime();
        public static FrameTimer GetStateTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_STATE_TIMER);
        public static void SetStateTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(PROP_STATE_TIMER, value);
        public static int GetUFOState(Entity entity) => entity.GetBehaviourField<int>(PROP_UFO_STATE);
        public static void SetUFOState(Entity entity, int value) => entity.SetBehaviourField(PROP_UFO_STATE, value);
        public static int GetTargetGridX(Entity entity) => entity.GetBehaviourField<int>(PROP_TARGET_GRID_X);
        public static void SetTargetGridX(Entity entity, int value) => entity.SetBehaviourField(PROP_TARGET_GRID_X, value);
        public static int GetTargetGridY(Entity entity) => entity.GetBehaviourField<int>(PROP_TARGET_GRID_Y);
        public static void SetTargetGridY(Entity entity, int value) => entity.SetBehaviourField(PROP_TARGET_GRID_Y, value);

        public const float FLY_HEIGHT = 80;
        public const float FLY_SPEED_ENTER = 0.3f;
        public const float FLY_SPEED_LEAVE = 0.1f;
        public const float FLY_SPEED_FACTOR_ENTER = 0.5f;
        public const float FLY_SPEED_FACTOR_LEAVE = 0.1f;
        public const float MAX_FLY_SPEED = 100f;
        public const float LEAVE_HEIGHT = 600;
        public const float START_HEIGHT = 600;
        public const int STATE_DEATH = VanillaEntityStates.DEAD;
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        public const int STATE_STAY = VanillaEntityStates.WALK;
        public const int STATE_ACT = VanillaEntityStates.ATTACK;
        public const int STATE_LEAVE = VanillaEntityStates.ENEMY_LEAVE;
        private const string PROP_REGION = "ufo";
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<int> PROP_TYPE = new VanillaEntityPropertyMeta<int>("type");
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<int> PROP_TARGET_GRID_X = new VanillaEntityPropertyMeta<int>("target_grid_x");
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<int> PROP_TARGET_GRID_Y = new VanillaEntityPropertyMeta<int>("target_grid_y");
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<int> PROP_UFO_STATE = new VanillaEntityPropertyMeta<int>("ufo_state", STATE_STAY);
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("state_timer");
    }
}
