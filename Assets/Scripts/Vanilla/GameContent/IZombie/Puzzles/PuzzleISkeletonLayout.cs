using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleISkeleton)]
    public class PuzzleISkeletonLayout : IZombieLayoutDefinition
    {
        public PuzzleISkeletonLayout(string nsp, string name) : base(nsp, name, 6)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.skeleton,
                VanillaEnemyID.ghost,
                VanillaEnemyID.necromancer,
                VanillaEnemyID.boneWall,
                VanillaEnemyID.WitherBoneWall,
                VanillaEnemyID.MeleeSkeleton,
                VanillaEnemyID.WitherSkeleton,
                VanillaEnemyID.IronWitherSkeleton,
                VanillaEnemyID.BerserkerHead,
                VanillaEnemyID.skeletonMage,
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            Insert(map, 2, 2, VanillaContraptionID.smallDispenser);
            RandomFill(map, VanillaContraptionID.punchton, 4, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 3, rng);
            RandomFill(map, VanillaContraptionID.dispenser, 3, rng);
            RandomFill(map, VanillaContraptionID.furnace, 9, rng);
            RandomFill(map, VanillaContraptionID.hellfire, 3, rng);
            RandomFill(map, VanillaContraptionID.totenser, 4, rng);
            RandomFill(map, VanillaContraptionID.DispenShield, 2, rng);
            RandomFill(map, VanillaContraptionID.smallDispenser, 2, rng);
        }
    }
}
