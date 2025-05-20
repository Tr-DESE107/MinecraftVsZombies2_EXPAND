using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Bosses
{
    public partial class TheGiant
    {
        #region 状态机
        private class TheGiantStateMachine : EntityStateMachine
        {
            public TheGiantStateMachine()
            {
                AddState(new IdleState());
                AddState(new DisassemblyState());
                AddState(new EyeState());
                AddState(new ArmsState());
                AddState(new RoarState());
                AddState(new BreathState());
                AddState(new StunState());
                AddState(new FaintState());
                AddState(new ChaseState());
                AddState(new DeathState());
            }
        }
        #endregion

        private static void SpawnDarkHole(Entity entity)
        {
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * DARK_HOLE_EFFECT_SCALE);
            entity.Spawn(VanillaEffectID.darkHole, entity.Position, param);
            entity.PlaySound(VanillaSoundID.odd);
        }
        public static void Stun(Entity entity, int duration)
        {
            if (entity.IsDead)
                return;
            if (!CanBeStunned(entity))
                return;
            entity.PlaySound(VanillaSoundID.zombieHurt, 0.5f);
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.StartState(entity, STATE_STUNNED);
            var substateTimer = stateMachine.GetSubStateTimer(entity);
            substateTimer.ResetTime(duration);
        }
        private static void RoarLoop(Entity entity)
        {
            entity.Level.ShakeScreen(15, 0, 5);
            foreach (var ent in entity.Level.FindEntities(e => CanRoarStun(entity, e)))
            {
                if (ent.IsEntityOf(VanillaContraptionID.lightningOrb))
                    continue;
                if (ent.IsEntityOf(VanillaContraptionID.noteBlock))
                {
                    if (!ent.HasBuff<NoteBlockChargedBuff>())
                    {
                        ent.AddBuff<NoteBlockChargedBuff>();
                        ent.PlaySound(VanillaSoundID.growBig);
                    }
                    continue;
                }
                ent.Stun(ROAR_STUN_TIME);
            }
        }
        private static bool CanCrawl(Entity entity)
        {
            return entity.GetBounds().min.x > VanillaLevelExt.GetAttackBorderX(false);
        }
        private static void CheckDeath(Entity entity)
        {
            if (!entity.IsDead)
                return;
            if (GetPhase(entity) != PHASE_3)
            {
                stateMachine.StartState(entity, STATE_FAINT);
            }
            else
            {
                stateMachine.StartState(entity, STATE_DEATH);
            }
        }

        #region 状态
        private class IdleState : EntityStateMachineState
        {
            public IdleState() : base(STATE_IDLE, ANIMATION_STATE_IDLE) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(60);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                if (GetPhase(entity) == PHASE_3)
                {
                    if (CanCrawl(entity))
                    {
                        stateMachine.StartState(entity, STATE_CHASE);
                    }
                    return;
                }
                else
                {
                    if (entity.IsDead)
                    {
                        stateMachine.StartState(entity, STATE_FAINT);
                        return;
                    }
                }
                if (GetPhase(entity) == PHASE_1 && entity.Health <= entity.GetMaxHealth() * 0.5f)
                {
                    Stun(entity, 30);
                    return;
                }
                UpdateStateSwitch(stateMachine, entity);
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void UpdateStateSwitch(EntityStateMachine stateMachine, Entity entity)
            {
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));
                if (stateTimer.Expired)
                {
                    var substate = stateMachine.GetSubState(entity);
                    if (substate != SUBSTATE_REFORMED)
                    {
                        stateMachine.StartState(entity, STATE_DISASSEMBLY);
                    }
                    else
                    {
                        var nextState = GetNextState(stateMachine, entity);
                        stateMachine.StartState(entity, nextState);
                    }
                }
            }
            private int GetNextState(EntityStateMachine stateMachine, Entity entity)
            {
                var lastState = stateMachine.GetPreviousState(entity);

                var atLeft = AtLeft(entity);
                bool attack1Used = lastState == STATE_EYES || lastState == STATE_ARMS;
                bool phase2 = GetPhase(entity) == PHASE_2;
                if (atLeft)
                {
                    if (attack1Used)
                    {
                        if (!phase2)
                        {
                            lastState = STATE_BREATH;
                            return lastState;
                        }
                        else
                        {
                            lastState = STATE_SNAKE;
                        }
                    }
                    lastState = STATE_EYES;
                    var innerTarget = FindEyeBulletTarget(entity, false);
                    var outerTarget = FindEyeBulletTarget(entity, true);
                    if (innerTarget.ExistsAndAlive() || outerTarget.ExistsAndAlive())
                    {
                        stateMachine.SetPreviousState(entity, lastState);
                        return lastState;
                    }
                }
                else
                {
                    if (attack1Used)
                    {
                        lastState = STATE_ARMS;
                        if (CanArmsAttack(entity))
                        {
                            stateMachine.SetPreviousState(entity, lastState);
                            return lastState;
                        }
                    }
                    if (!phase2)
                    {
                        lastState = STATE_ROAR;
                        if (entity.Level.EntityExists(e => CanRoarStun(entity, e)))
                        {
                            stateMachine.SetPreviousState(entity, lastState);
                            return lastState;
                        }
                    }
                    else
                    {
                        lastState = STATE_PACMAN;
                    }
                }
                return STATE_IDLE;
            }
            public const int SUBSTATE_REFORMED = 1;
        }
        private class DisassemblyState : EntityStateMachineState
        {
            public DisassemblyState() : base(STATE_DISASSEMBLY, ANIMATION_STATE_DISASSEMBLY) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CURL:
                        if (substateTimer.Expired)
                        {
                            SetInactive(entity, true);
                            // 分离。
                            bool atLeft = AtLeft(entity);
                            var targetState = atLeft ? SUBSTATE_TO_RIGHT : SUBSTATE_TO_LEFT;
                            stateMachine.SetSubState(entity, targetState);
                            substateTimer.ResetTime(30);

                            var level = entity.Level;
                            var rng = entity.RNG;
                            var lanes = level.GetMaxLaneCount();
                            var validLanes = Enumerable.Range(0, lanes);
                            for (int x = 0; x < ZOMBIE_BLOCK_COLUMNS; x++)
                            {
                                int column;
                                int columnEnd;
                                if (atLeft)
                                {
                                    column = GetZombieBlockLeftStartColumn(entity) - x;
                                    columnEnd = GetZombieBlockRightStartColumn(entity) - x + (ZOMBIE_BLOCK_COLUMNS - 1);
                                }
                                else
                                {
                                    column = GetZombieBlockRightStartColumn(entity) + x;
                                    columnEnd = GetZombieBlockLeftStartColumn(entity) + x - (ZOMBIE_BLOCK_COLUMNS - 1);
                                }
                                var lanesPool = validLanes.Shuffle(rng);
                                for (int y = 0; y < lanes; y++)
                                {
                                    var i = y + x * lanes;
                                    var lane = lanesPool.ElementAt(y);
                                    var block = entity.Spawn(VanillaEffectID.zombieBlock, entity.Position, entity.GetSpawnParams());
                                    block.SetParent(entity);
                                    ZombieBlock.SetStartGrid(block, column, lane);
                                    ZombieBlock.SetTargetGrid(block, columnEnd, lane);
                                    ZombieBlock.SetMoveCooldown(block, 30 + i * ZOMBIE_BLOCK_MOVE_INTERVAL);
                                    AddZombieBlock(entity, new EntityID(block));
                                }
                            }
                            SpawnDarkHole(entity);
                            entity.Position = GetCombinePosition(entity, atLeft);
                        }
                        break;
                    case SUBSTATE_TO_LEFT:
                    case SUBSTATE_TO_RIGHT:
                        if (substateTimer.Expired)
                        {
                            var blocks = GetZombieBlocks(entity);
                            bool allReached = true;
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (!block.ExistsAndAlive())
                                        continue;
                                    if (!ZombieBlock.IsReached(block))
                                    {
                                        substateTimer.Reset();
                                        allReached = false;
                                        break;
                                    }
                                }
                            }
                            if (allReached)
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_COMBINE);
                                substateTimer.ResetTime(15);
                                if (blocks != null)
                                {
                                    foreach (var blockID in blocks)
                                    {
                                        var block = blockID.GetEntity(entity.Level);
                                        if (block == null || !block.Exists())
                                            continue;
                                        ZombieBlock.SetTargetPosition(block, entity.Position);
                                    }
                                }
                            }
                        }
                        break;
                    case SUBSTATE_COMBINE:
                        if (substateTimer.Expired)
                        {
                            var blocks = GetZombieBlocks(entity);
                            if (blocks != null)
                            {
                                foreach (var blockID in blocks)
                                {
                                    var block = blockID.GetEntity(entity.Level);
                                    if (block == null || !block.Exists())
                                        continue;
                                    block.Remove();
                                }
                            }
                            SpawnDarkHole(entity);
                            ClearZombieBlocks(entity);
                            SetInactive(entity, false);
                            SetFlipX(entity, AtLeft(entity));
                            stateMachine.SetSubState(entity, SUBSTATE_RESTORE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_RESTORE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_RESTORE:
                        if (substateTimer.Expired)
                        {
                            if (GetPhase(entity) == PHASE_3)
                            {
                                stateMachine.StartState(entity, STATE_CHASE);
                            }
                            else
                            {
                                stateMachine.StartState(entity, STATE_IDLE);
                                stateMachine.SetSubState(entity, IdleState.SUBSTATE_REFORMED);
                            }
                        }
                        break;
                }
            }
            public const int SUBSTATE_CURL = 0;
            public const int SUBSTATE_TO_LEFT = 1;
            public const int SUBSTATE_TO_RIGHT = 2;
            public const int SUBSTATE_COMBINE = 3;
            public const int SUBSTATE_RESTORE = 4;
            public const int ANIMATION_SUBSTATE_RESTORE = 1;
        }
        private class EyeState : EntityStateMachineState
        {
            public EyeState() : base(STATE_EYES, ANIMATION_STATE_EYES) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_OPEN:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_FIRE);
                            substateTimer.ResetTime(EYE_BULLET_INTERVAL * EYE_BULLET_COUNT);
                        }
                        break;
                    case SUBSTATE_FIRE:
                        for (int i = 0; i < substateTimer.PassedIntervalCount(EYE_BULLET_INTERVAL); i++)
                        {
                            var outerEye = (substateTimer.Frame / EYE_BULLET_INTERVAL) % 2 == 0;
                            var target = FindEyeBulletTarget(entity, outerEye);
                            if (target.ExistsAndAlive())
                            {
                                ShootBullet(entity, target, outerEye);
                                continue;
                            }
                            outerEye = !outerEye;
                            target = FindEyeBulletTarget(entity, outerEye);
                            if (target.ExistsAndAlive())
                            {
                                ShootBullet(entity, target, outerEye);
                                continue;
                            }
                            substateTimer.Frame = 0;
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CLOSE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_CLOSE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_CLOSE:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void ShootBullet(Entity entity, Entity target, bool outerEye)
            {
                entity.Level.ShakeScreen(5, 0, 5);
                var param = entity.GetShootParams();
                var offset = outerEye ? OUTER_EYE_BULLET_OFFSET : INNER_EYE_BULLET_OFFSET;
                offset = entity.ModifyShotOffset(offset);
                param.position = entity.Position + offset;
                param.soundID = null;
                param.damage = entity.GetDamage() * EYE_BULLET_DAMAGE_MULTIPLIER;
                param.projectileID = VanillaProjectileID.reflectionBullet;
                param.velocity = (target.GetCenter() - param.position).normalized * EYE_BULLET_SPEED;
                var spawnParam = entity.GetSpawnParams();
                spawnParam.SetProperty(EngineEntityProps.SCALE, Vector3.one * 2);
                spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * 2);
                param.spawnParam = spawnParam;
                var bullet = entity.ShootProjectile(param);
                bullet.PlaySound(VanillaSoundID.reflection, 0.5f);
                bullet.PlaySound(VanillaSoundID.mineExplode, 0.5f);
            }
            public const int SUBSTATE_OPEN = 0;
            public const int SUBSTATE_FIRE = 1;
            public const int SUBSTATE_CLOSE = 2;
            public const int ANIMATION_SUBSTATE_CLOSE = 1;
        }
        private class ArmsState : EntityStateMachineState
        {
            public ArmsState() : base(STATE_ARMS, ANIMATION_STATE_ARMS) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_OUTER_LIFT:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_OUTER_SMASH);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_OUTER_SMASH);
                            substateTimer.ResetTime(5);
                        }
                        break;
                    case SUBSTATE_OUTER_SMASH:
                        if (substateTimer.Expired)
                        {
                            Smash(entity, true);
                            stateMachine.SetSubState(entity, SUBSTATE_OUTER_SMASHED);
                            substateTimer.ResetTime(25);
                        }
                        break;
                    case SUBSTATE_OUTER_SMASHED:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_LIFT);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_INNER_LIFT);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_INNER_LIFT:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_SMASH);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_INNER_SMASH);
                            substateTimer.ResetTime(5);
                        }
                        break;
                    case SUBSTATE_INNER_SMASH:
                        if (substateTimer.Expired)
                        {
                            Smash(entity, false);
                            stateMachine.SetSubState(entity, SUBSTATE_INNER_SMASHED);
                            substateTimer.ResetTime(25);
                        }
                        break;
                    case SUBSTATE_INNER_SMASHED:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            private void ShootBullet(Entity entity, Entity target, bool outerEye)
            {
                entity.Level.ShakeScreen(5, 0, 5);
                var param = entity.GetShootParams();
                var offset = outerEye ? OUTER_EYE_BULLET_OFFSET : INNER_EYE_BULLET_OFFSET;
                offset = entity.ModifyShotOffset(offset);
                param.position = entity.Position + offset;
                param.soundID = null;
                param.damage = entity.GetDamage() * EYE_BULLET_DAMAGE_MULTIPLIER;
                param.projectileID = VanillaProjectileID.reflectionBullet;
                param.velocity = (target.GetCenter() - param.position).normalized * EYE_BULLET_SPEED;
                var spawnParam = entity.GetSpawnParams();
                spawnParam.SetProperty(EngineEntityProps.SCALE, Vector3.one * 2);
                spawnParam.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * 2);
                param.spawnParam = spawnParam;
                var bullet = entity.ShootProjectile(param);
                bullet.PlaySound(VanillaSoundID.reflection, 0.5f);
                bullet.PlaySound(VanillaSoundID.mineExplode, 0.5f);
            }
            public const int SUBSTATE_OUTER_LIFT = 0;
            public const int SUBSTATE_OUTER_SMASH = 1;
            public const int SUBSTATE_OUTER_SMASHED = 2;
            public const int SUBSTATE_INNER_LIFT = 3;
            public const int SUBSTATE_INNER_SMASH = 4;
            public const int SUBSTATE_INNER_SMASHED = 5;
            public const int ANIMATION_SUBSTATE_OUTER_LIFT = 0;
            public const int ANIMATION_SUBSTATE_OUTER_SMASH = 1;
            public const int ANIMATION_SUBSTATE_INNER_LIFT = 2;
            public const int ANIMATION_SUBSTATE_INNER_SMASH = 3;
        }
        private class RoarState : EntityStateMachineState
        {
            public RoarState() : base(STATE_ROAR, ANIMATION_STATE_ROAR) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(15);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_LOOP);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_LOOP);
                            substateTimer.ResetTime(90);
                            entity.PlaySound(VanillaSoundID.giantRoar);
                        }
                        break;
                    case SUBSTATE_LOOP:
                        RoarLoop(entity);
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_END);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_END);
                            substateTimer.ResetTime(15);
                        }
                        break;
                    case SUBSTATE_END:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_START = 0;
            public const int SUBSTATE_LOOP = 1;
            public const int SUBSTATE_END = 2;
            public const int ANIMATION_SUBSTATE_START = 0;
            public const int ANIMATION_SUBSTATE_LOOP = 1;
            public const int ANIMATION_SUBSTATE_END = 2;
        }
        private class BreathState : EntityStateMachineState
        {
            public BreathState() : base(STATE_BREATH, ANIMATION_STATE_BREATH) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = machine.GetSubStateTimer(entity);
                substateTimer.ResetTime(30);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_BREATH);
                            substateTimer.ResetTime(45);
                            entity.PlaySound(VanillaSoundID.poisonCast);
                            for (int lane = 0; lane < entity.Level.GetMaxLaneCount(); lane++)
                            {
                                var x = entity.Position.x + 80 * entity.GetFacingX();
                                var z = entity.Level.GetEntityLaneZ(lane);
                                var y = entity.Level.GetGroundY(x, z);
                                var param = entity.GetSpawnParams();
                                var gas = entity.Spawn(VanillaEffectID.mummyGas, new Vector3(x, y, z), param);
                                gas.Velocity = entity.GetFacingDirection() * 2;
                            }
                        }
                        break;
                    case SUBSTATE_BREATH:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_END);
                            substateTimer.ResetTime(15);
                        }
                        break;
                    case SUBSTATE_END:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_START = 0;
            public const int SUBSTATE_BREATH = 1;
            public const int SUBSTATE_END = 2;
        }
        private class StunState : EntityStateMachineState
        {
            public StunState() : base(STATE_STUNNED, ANIMATION_STATE_STUNNED) { }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_STUNNED:
                        if (substateTimer.PassedFrame(substateTimer.MaxFrame - 18))
                        {
                            entity.Level.ShakeScreen(5, 0, 10);
                            entity.PlaySound(VanillaSoundID.thump);
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_RESTORE);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_RESTORE);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_RESTORE:
                        if (substateTimer.Expired)
                        {
                            stateMachine.StartState(entity, STATE_IDLE);
                            if (GetPhase(entity) == PHASE_1 && entity.Health <= entity.GetMaxHealth() * 0.5f)
                            {
                                SetPhase(entity, PHASE_2);
                            }
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_STUNNED = 0;
            public const int SUBSTATE_RESTORE = 1;
            public const int ANIMATION_SUBSTATE_RESTORE = 1;
        }
        private class FaintState : EntityStateMachineState
        {
            public FaintState() : base(STATE_FAINT, ANIMATION_STATE_FAINT) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(90);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_FAINT:
                        if (substateTimer.PassedFrame(substateTimer.MaxFrame - 18))
                        {
                            entity.Level.ShakeScreen(5, 0, 10);
                            entity.PlaySound(VanillaSoundID.thump);
                        }
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_HEAL);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_HEAL:
                        if (entity.IsDead)
                        {
                            entity.Revive();
                        }
                        SetPhase(entity, PHASE_3);
                        entity.Health = entity.GetMaxHealth() * substateTimer.GetPassedPercentage();
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_ROAR);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_ROAR);
                            substateTimer.ResetTime(90);
                            entity.PlaySound(VanillaSoundID.giantRoar);
                        }
                        break;
                    case SUBSTATE_ROAR:
                        RoarLoop(entity);
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_ROAR_END);
                            stateMachine.SetAnimationSubstate(entity, ANIMATION_SUBSTATE_ROAR_END);
                            substateTimer.ResetTime(30);
                        }
                        break;
                    case SUBSTATE_ROAR_END:
                        if (substateTimer.Expired)
                        {
                            if (AtLeft(entity))
                            {
                                stateMachine.StartState(entity, STATE_DISASSEMBLY);
                            }
                            else
                            {
                                stateMachine.StartState(entity, STATE_CHASE);
                            }
                        }
                        break;
                }
            }
            public const int SUBSTATE_FAINT = 0;
            public const int SUBSTATE_HEAL = 1;
            public const int SUBSTATE_ROAR = 2;
            public const int SUBSTATE_ROAR_END = 3;
            public const int ANIMATION_SUBSTATE_ROAR = 1;
            public const int ANIMATION_SUBSTATE_ROAR_END = 2;
        }
        private class ChaseState : EntityStateMachineState
        {
            public ChaseState() : base(STATE_CHASE, ANIMATION_STATE_CHASE) { }
            public override void OnEnter(EntityStateMachine machine, Entity entity)
            {
                base.OnEnter(machine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.ResetTime(10);
            }
            public override void OnUpdateAI(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateAI(stateMachine, entity);
                var substateTimer = stateMachine.GetSubStateTimer(entity);
                substateTimer.Run(stateMachine.GetSpeed(entity));

                var substate = stateMachine.GetSubState(entity);
                switch (substate)
                {
                    case SUBSTATE_CRAWL_START:
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CRAWL);
                            substateTimer.ResetTime(10);
                        }
                        break;
                    case SUBSTATE_CRAWL:
                        entity.Velocity = entity.GetFacingDirection() * 1;
                        if (substateTimer.Expired)
                        {
                            stateMachine.SetSubState(entity, SUBSTATE_CRAWL_END);
                            substateTimer.ResetTime(10);
                        }
                        break;
                    case SUBSTATE_CRAWL_END:
                        if (substateTimer.Expired)
                        {
                            if (!CanCrawl(entity))
                            {
                                stateMachine.StartState(entity, STATE_IDLE);
                            }
                            else
                            {
                                stateMachine.SetSubState(entity, SUBSTATE_CRAWL_START);
                                substateTimer.ResetTime(10);
                            }
                        }
                        break;
                }
            }
            public override void OnUpdateLogic(EntityStateMachine machine, Entity entity)
            {
                base.OnUpdateLogic(machine, entity);
                CheckDeath(entity);
            }
            public const int SUBSTATE_CRAWL_START = 0;
            public const int SUBSTATE_CRAWL = 1;
            public const int SUBSTATE_CRAWL_END = 2;
        }
        private class DeathState : EntityStateMachineState
        {
            public DeathState() : base(STATE_DEATH, ANIMATION_STATE_DEATH) { }
            public override void OnEnter(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnEnter(stateMachine, entity);
                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.ResetTime(150);
            }
            public override void OnUpdateLogic(EntityStateMachine stateMachine, Entity entity)
            {
                base.OnUpdateLogic(stateMachine, entity);

                var stateTimer = stateMachine.GetStateTimer(entity);
                stateTimer.Run(stateMachine.GetSpeed(entity));

                if (stateTimer.Expired)
                {
                    entity.PlaySound(VanillaSoundID.witherDeath);
                    entity.PlaySound(VanillaSoundID.explosion);
                    entity.Level.Explode(entity.GetCenter(), 120, entity.GetFaction(), entity.GetDamage() * 18, new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), entity);

                    var param = entity.GetSpawnParams();
                    param.SetProperty(EngineEntityProps.SIZE, Vector3.one * 240);
                    var exp = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), param);
                    entity.Level.ShakeScreen(20, 0, 30);
                    entity.Remove();
                }
            }
        }
        #endregion

    }
}
