using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleAllYourObservesAreBelongToUs)]
    public class PuzzleAYOABTULayout : IZombieLayoutDefinition
    {
        public PuzzleAYOABTULayout(string nsp, string name) : base(nsp, name, 6)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaBlueprintID.FromEntity(VanillaEnemyID.imp),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.leatherCappedZombie),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.ghost),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.reflectiveBarrierZombie),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.ironHelmettedZombie),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.skeletonWarrior),
                VanillaBlueprintID.FromEntity(VanillaEnemyID.wickedHermitZombie),
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFillAtLane(map, 0, VanillaContraptionID.mineTNT, 4, rng);

            Insert(map, 0, 1, VanillaContraptionID.noteBlock);
            Insert(map, 5, 1, VanillaContraptionID.dreamCrystal);
            RandomFillAtLane(map, 1, VanillaContraptionID.stoneDropper, 1, rng);
            RandomFillAtLane(map, 1, VanillaContraptionID.woodenDropper, 1, rng);

            RandomFillAtLane(map, 2, VanillaContraptionID.punchton, 1, rng);
            RandomFillAtLane(map, 2, VanillaContraptionID.magichest, 3, rng);

            Insert(map, 5, 3, VanillaContraptionID.hellfire);
            RandomFillAtLane(map, 3, VanillaContraptionID.dispenser, 3, rng);

            RandomFillAtLane(map, 4, VanillaContraptionID.splitenser, 1, rng);
            RandomFillAtLane(map, 4, VanillaContraptionID.totenser, 1, rng);
            RandomFillAtLane(map, 4, VanillaContraptionID.smallDispenser, 1, rng);
            RandomFillAtLane(map, 4, VanillaContraptionID.soulFurnace, 1, rng);
            RandomFillAtLane(map, 4, VanillaContraptionID.silvenser, 1, rng);


            RandomFill(map, VanillaContraptionID.furnace, 9, rng);
        }
    }
}
