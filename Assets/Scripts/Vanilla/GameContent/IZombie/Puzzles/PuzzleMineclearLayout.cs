using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleMineclear)]
    public class PuzzleMineclearLayout : IZombieLayoutDefinition
    {
        public PuzzleMineclearLayout(string nsp, string name) : base(nsp, name, 7)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.imp,
                VanillaEnemyID.Mannequin,
                VanillaEnemyID.necromancer,
                VanillaEnemyID.KingSkeleton,
                VanillaEnemyID.EvilMage,
                VanillaEnemyID.KingofReverser,
                VanillaEnemyID.SixQiZombie,
                VanillaEnemyID.emperorZombie,
                VanillaEnemyID.shikaisenZombie,
                VanillaEnemyID.HeavyGutant,
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.mineTNT, 10, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 5, rng);
            RandomFill(map, VanillaContraptionID.punchton, 6, rng);
            RandomFill(map, VanillaContraptionID.furnace, 10, rng);
            RandomFill(map, VanillaContraptionID.hellfire, 4, rng);
        }
    }
}
