using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleFireInTheHole)]
    public class PuzzleFireInTheHoleLayout : IZombieLayoutDefinition
    {
        public PuzzleFireInTheHoleLayout(string nsp, string name) : base(nsp, name, 7)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.imp,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.reflectiveBarrierZombie,
                VanillaEnemyID.wickedHermitZombie,
                VanillaEnemyID.MannequinTNT,
                VanillaEnemyID.NetherArcher,
                VanillaEnemyID.WintherMage,
                VanillaEnemyID.AngryReverser,
                VanillaEnemyID.WitherSkeletonHorse,
                VanillaEnemyID.SixQiZombie,
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFillAtColumn(map, 4, VanillaContraptionID.hellfire, 3, rng);

            RandomFill(map, VanillaContraptionID.magichest, 3, rng);
            RandomFill(map, VanillaContraptionID.splitenser, 3, rng);
            RandomFill(map, VanillaContraptionID.drivenser, 5, rng);
            RandomFill(map, VanillaContraptionID.dispenser, 3, rng);
            RandomFill(map, VanillaContraptionID.furnace, 10, rng);
            RandomFill(map, VanillaContraptionID.GlowingObsidian, 3, rng);
            RandomFill(map, VanillaContraptionID.hellfire, 2, rng);
            RandomFill(map, VanillaContraptionID.lightningOrb, 3, rng);
        }
    }
}
