using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.ContraptionDevourer_2)]
    public partial class ContraptionDevourer_2 : StageDefinition
    {
        public ContraptionDevourer_2(string nsp, string name) : base(nsp, name)
        {

            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
        }
        
        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);

            var allContraptions = new NamespaceID[]
            {

        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        VanillaContraptionID.lightningOrb,
        
        
        VanillaContraptionID.teslaCoil,
        VanillaContraptionID.stoneShield,

            };

            int Num = 10;

            var rand = new System.Random();

            // 随机选 Num 个不重复装置
            var selectedContraptions = allContraptions.OrderBy(x => rand.Next()).Take(Num).ToArray();

            var usedPositions = new System.Collections.Generic.HashSet<(int, int)>();

            for (int i = 0; i < Num; i++)
            {
                int col, row;
                do
                {
                    col = rand.Next(0, 8);  // 0~7列
                    row = rand.Next(0, 5);  // 0~4行
                } while (!usedPositions.Add((col, row)));

                float x = level.GetEntityColumnX(col);
                float z = level.GetEntityLaneZ(row);
                float y = level.GetGroundY(x, z);
                Vector3 pos = new Vector3(x, y, z);

                level.Spawn(selectedContraptions[i], pos, null);
            }
        }

        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);

            level.SetSeedSlotCount(10);
            level.FillSeedPacks(new NamespaceID[]
            {
                VanillaBlueprintID.FromEntity(VanillaContraptionID.devourer),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.devourer),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.devourer),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.lightningOrb),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.tnt),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.punchton),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.dreamSilk),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.magichest),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.blackHoleBomb),
                VanillaBlueprintID.FromEntity(VanillaContraptionID.mineTNT),
            });
            //level.SetPickaxeActive(false);
            level.SetEnergy(8000);
        }
    }
}
