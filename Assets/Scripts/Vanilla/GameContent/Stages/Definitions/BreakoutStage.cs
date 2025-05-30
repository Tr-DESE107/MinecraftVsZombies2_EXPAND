using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public partial class BreakoutStage : StageDefinition
    {
        public BreakoutStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new RedstoneDropStageBehaviour(this));
        }
        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);
            SpawnBoard(level);
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            level.SetSeedSlotCount(3);
            level.FillSeedPacks(new NamespaceID[3]
            {
                VanillaBlueprintID.returnPearl,
                VanillaBlueprintID.lengthenBoard,
                VanillaBlueprintID.addPearl
            });
            level.SetPickaxeActive(false);
            level.SetStarshardActive(false);
            level.SetTriggerActive(false);
        }
        public override void OnPostHugeWave(LevelEngine level)
        {
            base.OnPostHugeWave(level);
        }
        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            var board = level.FindFirstEntity(VanillaEffectID.breakoutBoard);
            if (board == null || !board.Exists())
            {
                board = SpawnBoard(level);
            }
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                level.SetHeldItem(VanillaHeldTypes.breakoutBoard, board.ID, 100, true);
            }
        }
        public override void OnPostEnemySpawned(Entity entity)
        {
            base.OnPostEnemySpawned(entity);
            AddSpeedBuff(entity);
        }
        private Entity SpawnBoard(LevelEngine level)
        {
            var x = level.GetEntityColumnX(1);
            var z = level.GetEntityLaneZ(2);
            var y = 32;
            var pos = new Vector3(x, y, z);
            var board = level.Spawn(VanillaEffectID.breakoutBoard, pos, null);
            BreakoutBoard.SpawnPearl(board);
            return board;
        }
        private void AddSpeedBuff(Entity entity)
        {
            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, Mathf.Lerp(1, 2, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount()));
        }
    }
}
