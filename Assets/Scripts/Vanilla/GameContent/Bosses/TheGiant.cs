using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Modifiers;
using MVZ2.Vanilla.Properties;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    [EntityBehaviourDefinition(VanillaBossNames.theGiant)]
    public partial class TheGiant : BossBehaviour
    {
        public TheGiant(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.FLIP_X, PROP_FLIP_X, priority: VanillaModifierPriorities.FORCE));
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            stateMachine.Init(boss);
            stateMachine.StartState(boss, STATE_IDLE);

            boss.CollisionMaskHostile |=
                EntityCollisionHelper.MASK_PLANT |
                EntityCollisionHelper.MASK_ENEMY |
                EntityCollisionHelper.MASK_OBSTACLE |
                EntityCollisionHelper.MASK_BOSS |
                EntityCollisionHelper.MASK_CART;
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);

            var cryTimer = GetCryTimer(entity);
            if (cryTimer == null)
            {
                cryTimer = new FrameTimer(CRY_INTERVAL);
                SetCryTimer(entity, cryTimer);
            }
            if (!entity.HasBuff<TheGiantInactiveBuff>())
            {
                cryTimer.Run();
            }
            if (cryTimer.Expired)
            {
                cryTimer.ResetTime(CRY_INTERVAL);
                entity.PlaySound(VanillaSoundID.zombieCry, 0.5f);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var other = collision.Other;
            var self = collision.Entity;
            if (!other.Exists() || !other.IsHostile(self))
                return;
            if (other.Type == EntityTypes.CART)
            {
                other.Die(self);
                other.PlaySound(VanillaSoundID.smash);
                return;
            }
            var otherCollider = collision.OtherCollider;
            var crushDamage = 1000000;
            var substate = stateMachine.GetSubState(self);
            if (!other.IsInvincible())
            {
                var result = otherCollider.TakeDamage(crushDamage, new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), self);
                if (result != null && result.HasAnyFatal())
                {
                    other.PlaySound(VanillaSoundID.smash);
                    if (self.State == STATE_PACMAN)
                    {
                        self.HealEffects(STATE_PACMAN, other);
                    }
                }
            }
        }
        public override void PreTakeDamage(DamageInput damageInfo, CallbackResult result)
        {
            base.PreTakeDamage(damageInfo, result);
            if (damageInfo.Amount > 600)
            {
                damageInfo.SetAmount(600);
            }
        }
        #endregion 事件

        #region 字段
        public static FrameTimer GetCryTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_CRY_TIMER);
        public static void SetCryTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(PROP_CRY_TIMER, value);
        public static int GetPhase(Entity entity) => entity.GetBehaviourField<int>(PROP_PHASE);
        public static void SetPhase(Entity entity, int value) => entity.SetBehaviourField(PROP_PHASE, value);
        public static bool IsFlipX(Entity entity) => entity.GetBehaviourField<bool>(PROP_FLIP_X);
        public static void SetFlipX(Entity entity, bool value) => entity.SetBehaviourField(PROP_FLIP_X, value);

        #region 僵尸块
        public static List<EntityID> GetZombieBlocks(Entity entity) => entity.GetBehaviourField<List<EntityID>>(PROP_ZOMBIE_BLOCKS);
        public static void SetZombieBlocks(Entity entity, List<EntityID> value) => entity.SetBehaviourField(PROP_ZOMBIE_BLOCKS, value);
        public static void AddZombieBlock(Entity entity, EntityID value) 
        {
            var blocks = GetZombieBlocks(entity);
            if (blocks == null)
            {
                blocks = new List<EntityID>();
                SetZombieBlocks(entity, blocks);
            }
            blocks.Add(value);
        }
        public static bool RemoveZombieBlock(Entity entity, EntityID value)
        {
            var blocks = GetZombieBlocks(entity);
            if (blocks == null)
            {
                return false;
            }
            return blocks.Remove(value);
        }
        public static bool HasZombieBlock(Entity entity, EntityID value)
        {
            var blocks = GetZombieBlocks(entity);
            if (blocks == null)
            {
                return false;
            }
            return blocks.Contains(value);
        }
        public static void ClearZombieBlocks(Entity entity)
        {
            var blocks = GetZombieBlocks(entity);
            if (blocks == null)
            {
                return;
            }
            blocks.Clear();
        }
        #endregion

        #endregion

        public static void SetInactive(Entity entity, bool value)
        {
            entity.SetAnimationBool("Active", !value);
            if (value)
            {
                if (!entity.HasBuff<TheGiantInactiveBuff>())
                    entity.AddBuff<TheGiantInactiveBuff>();
            }
            else
            {
                entity.RemoveBuffs<TheGiantInactiveBuff>();
            }
        }
        public static bool AtLeft(Entity entity)
        {
            return entity.Position.x < VanillaLevelExt.LAWN_CENTER_X;
        }
        public static void Stun(Entity entity, int duration)
        {
            if (entity.IsDead)
                return;
            entity.PlaySound(VanillaSoundID.zombieHurt, 0.5f);
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.StartState(entity, STATE_STUNNED);
            var stateTimer = stateMachine.GetStateTimer(entity);
            stateTimer.ResetTime(duration);
        }
        public static int GetZombieBlockLeftStartColumn(Entity entity)
        {
            return ZOMBIE_BLOCK_LEFT_COLUMN_START + ZOMBIE_BLOCK_COLUMNS - 1;
        }
        public static int GetZombieBlockRightStartColumn(Entity entity)
        {
            return entity.Level.GetMaxColumnCount() - ZOMBIE_BLOCK_COLUMNS;
        }
        public static float GetCombineX(Entity entity, bool atRight)
        {
            var level = entity.Level;
            int startColumn;
            int endColumn;
            if (atRight)
            {
                startColumn = GetZombieBlockRightStartColumn(entity);
                endColumn = startColumn + ZOMBIE_BLOCK_COLUMNS;
            }
            else
            {
                startColumn = GetZombieBlockLeftStartColumn(entity);
                endColumn = startColumn - ZOMBIE_BLOCK_COLUMNS;
            }
            var startX = level.GetEntityColumnX(startColumn);
            var endX = level.GetEntityColumnX(endColumn);
            return (startX + endX) * 0.5f;
        }
        public static float GetCombineZ(Entity entity)
        {
            var level = entity.Level;
            return (level.GetGridTopZ() + level.GetGridBottomZ()) *0.5f;
        }
        public static Vector3 GetCombinePosition(Entity entity, bool atRight)
        {
            var level = entity.Level;
            var x = GetCombineX(entity, atRight);
            var z = GetCombineZ(entity);
            var y = level.GetGroundY(x, z);
            return new Vector3(x, y, z);
        }
        #region 常量
        private static readonly VanillaEntityPropertyMeta PROP_CRY_TIMER = new VanillaEntityPropertyMeta("CryTimer");
        private static readonly VanillaEntityPropertyMeta PROP_PHASE = new VanillaEntityPropertyMeta("Phase");
        private static readonly VanillaEntityPropertyMeta PROP_ZOMBIE_BLOCKS = new VanillaEntityPropertyMeta("ZombieBlocks");
        private static readonly VanillaEntityPropertyMeta PROP_FLIP_X = new VanillaEntityPropertyMeta("FlipX");

        private const int STATE_IDLE = VanillaEntityStates.THE_GIANT_IDLE;
        private const int STATE_DISASSEMBLY = VanillaEntityStates.THE_GIANT_DISASSEMBLY;
        private const int STATE_EYES = VanillaEntityStates.THE_GIANT_EYES;
        private const int STATE_ROAR = VanillaEntityStates.THE_GIANT_ROAR;
        private const int STATE_ARMS = VanillaEntityStates.THE_GIANT_ARMS;
        private const int STATE_BREATH = VanillaEntityStates.THE_GIANT_BREATH;
        private const int STATE_STUNNED = VanillaEntityStates.THE_GIANT_STUNNED;
        private const int STATE_PACMAN = VanillaEntityStates.THE_GIANT_PACMAN;
        private const int STATE_SNAKE = VanillaEntityStates.THE_GIANT_SNAKE;
        private const int STATE_CHASE = VanillaEntityStates.THE_GIANT_CHASE;
        private const int STATE_DEATH = VanillaEntityStates.THE_GIANT_DEATH;

        public const int PHASE_1 = 0;
        public const int PHASE_2 = 1;

        public const int ZOMBIE_BLOCK_COLUMNS = 2;
        public const int ZOMBIE_BLOCK_LEFT_COLUMN_START = -1;
        public const int ZOMBIE_BLOCK_MOVE_INTERVAL = 10;
        public const float DARK_HOLE_EFFECT_SCALE = 2.5f;

        public const int CRY_INTERVAL = 300;
        #endregion 常量

        private static TheGiantStateMachine stateMachine = new TheGiantStateMachine();
    }
}
