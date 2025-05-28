using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleCanYouPassIt)]
    public class PuzzleCanYouPassItLayout : IZombieLayoutDefinition
    {
        public PuzzleCanYouPassItLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.ironHelmettedZombie,
                VanillaEnemyID.caveSpider
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.drivenser, 3, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 4, rng);
            RandomFill(map, VanillaContraptionID.mineTNT, 3, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
            RandomFill(map, VanillaContraptionID.smallDispenser, 2, rng);
        }
    }
}
