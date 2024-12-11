using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
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
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
        }
        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);
            level.Spawn(VanillaEffectID.rain, new Vector3(VanillaLevelExt.LEVEL_WIDTH * 0.5f, 0, 0), null);
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
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
        }
        public override void OnPostEnemySpawned(Entity entity)
        {
            base.OnPostEnemySpawned(entity);
            var advanceDistance = entity.RNG.Next(0, entity.Level.GetGridWidth() * 3f);
            entity.Position += Vector3.left * advanceDistance;

            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, Mathf.Lerp(3, 5, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount()));
        }
        private void SetThunderTimer(LevelEngine level, FrameTimer timer)
        {
            level.SetProperty("ThunderTimer", timer);
        }
        private FrameTimer GetThunderTimer(LevelEngine level)
        {
            return level.GetProperty<FrameTimer>("ThunderTimer");
        }
    }
}
