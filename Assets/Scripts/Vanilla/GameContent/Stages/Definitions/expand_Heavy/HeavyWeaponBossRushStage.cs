#nullable enable

using MVZ2.GameContent.Seeds;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [AutoStageDefinition(VanillaStageNames.HeavyWeaponBossRush)]
    public class HeavyWeaponBossRushStage : StageDefinition
    {
        public HeavyWeaponBossRushStage(string nsp, string name) : base(nsp, name)
        {
            // 复用重装兵器操控与波次系统，但【不加】 FinalWaveClearBehaviour，  
            // 通关完全由 BossRushBehaviour 控制。  
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new RedstoneDropStageBehaviour(this));
            AddBehaviour(new HeavyWeaponStageBehaviour(this));   // 生成/复活 MegaSnipenser  
            AddBehaviour(new SpeedUpStageBehaviour(this, 1.5f, 2f));
            AddBehaviour(new BossRushBehaviour(this));           // 必须在 WaveStageBehaviour 之后，确保其 Start 后执行  
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            var blueprints = new NamespaceID[]
            {
                VanillaBlueprintID.heavyWeaponFlashbang,
                VanillaBlueprintID.heavyWeaponRapid,
                VanillaBlueprintID.heavyWeaponSpread,
            };
            level.SetSeedSlotCount(blueprints.Length);
            level.FillSeedPacks(blueprints);
        }
    }
}
