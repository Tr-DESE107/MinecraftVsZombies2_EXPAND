using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public partial class WhackAGhostStage : StageDefinition
    {
        public WhackAGhostStage(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaAreaProps.DARKNESS_VALUE, 0.5f);
            SetProperty(VanillaStageProps.AUTO_COLLECT, true);
            var waveStageBehaviour = new WaveStageBehaviour(this);
            waveStageBehaviour.SpawnFlagZombie = false;
            AddBehaviour(waveStageBehaviour);
            AddBehaviour(new GemStageBehaviour(this));
        }
        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);
            level.StartRain();
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            level.SetEnergyActive(false);
            level.SetBlueprintsActive(false);
            level.SetPickaxeActive(false);
            level.SetStarshardActive(false);
            level.SetTriggerActive(false);
            SetThunderTimer(level, new FrameTimer(150));
        }
        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                level.SetHeldItem(VanillaHeldTypes.sword, 0, 255, true);
            }
            var timer = GetThunderTimer(level);
            timer.Run();
            if (timer.Expired)
            {
                level.Thunder();
                timer.Reset();
            }
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            var timer = GetThunderTimer(level);
            timer.Frame = Mathf.Min(timer.Frame, 30);

            var napstablookPoints = (wave - 5) / 3f;
            if (level.IsHugeWave(wave))
            {
                napstablookPoints *= 2.5f;
            }
            var napstablookCount = Mathf.CeilToInt(napstablookPoints);
            for (int i = 0; i < napstablookCount; i++)
            {
                var lane = level.GetRandomEnemySpawnLane();
                var column = level.GetSpawnRNG().Next(0, 5);
                var x = level.GetColumnX(column);
                var z = level.GetEntityLaneZ(lane);
                var y = level.GetGroundY(x, z);
                var napstablook = level.Spawn(VanillaEnemyID.napstablook, new Vector3(x, y, z), null);
                napstablook.SetScale(new Vector3(-1, 1, 1));
                napstablook.SetDisplayScale(new Vector3(-1, 1, 1));
                AddSpeedBuff(napstablook);
            }
        }
        public override void OnPostEnemySpawned(Entity entity)
        {
            base.OnPostEnemySpawned(entity);
            var advanceDistance = entity.RNG.Next(0, entity.Level.GetGridWidth() * 3f);
            entity.Position += Vector3.left * advanceDistance;
            AddSpeedBuff(entity);
        }
        private void SetThunderTimer(LevelEngine level, FrameTimer timer)
        {
            level.SetBehaviourField(ID, "ThunderTimer", timer);
        }
        private FrameTimer GetThunderTimer(LevelEngine level)
        {
            return level.GetBehaviourField<FrameTimer>(ID, "ThunderTimer");
        }
        private void AddSpeedBuff(Entity entity)
        {
            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, Mathf.Lerp(3, 5, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount()));
        }
        private static readonly NamespaceID ID = new NamespaceID("mvz2", "whack_a_ghost");
    }
}
