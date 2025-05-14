using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public class IZombieBehaviour : StageBehaviour
    {
        public IZombieBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.SetProperty(VanillaLevelProps.FURNACE_DROP_REDSTONE_COUNT, 8);
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            GenerateMap(level);
        }
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetEnergy(level.GetStartEnergy());
            level.SetSeedSlotCount(10);
            level.SetStarshardActive(false);
            level.SetPickaxeActive(false);
            level.SetTriggerActive(false);
            level.FillSeedPacks(new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.gargoyle,
                VanillaEnemyID.ironHelmettedZombie,
            });
        }
        private void GenerateMap(LevelEngine level)
        {
            int columns = 4;
            var map = new IZombieMap(level, columns, level.GetMaxLaneCount());
            var layout = new DispenserLayout(columns);
            layout.Fill(map, level.GetSpawnRNG());

            map.Apply();
        }
        private void FillLayout(LevelEngine level)
        {

        }

        #region 属性字段
        private const string PROP_REGION = "i_zombie_stage";
        #endregion
    }
}
