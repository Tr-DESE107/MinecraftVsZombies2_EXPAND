using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleAbsoluteDefense)]
    public class PuzzleAbsoluteDefenseLayout : IZombieLayoutDefinition
    {
        public PuzzleAbsoluteDefenseLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.ironHelmettedZombie,
                VanillaEnemyID.skeletonHorse
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            for (int lane = 0; lane < map.Lanes; lane++)
            {
                Insert(map, 3, lane, VanillaContraptionID.obsidian);
            }
            Insert(map, 0, 0, VanillaContraptionID.woodenDropper);
            Insert(map, 1, 0, VanillaContraptionID.furnace);
            Insert(map, 2, 0, VanillaContraptionID.furnace);

            Insert(map, 0, 1, VanillaContraptionID.furnace);
            Insert(map, 1, 1, VanillaContraptionID.woodenDropper);
            Insert(map, 2, 1, VanillaContraptionID.stoneDropper);

            Insert(map, 0, 2, VanillaContraptionID.totenser);
            Insert(map, 1, 2, VanillaContraptionID.totenser);
            Insert(map, 2, 2, VanillaContraptionID.furnace);

            Insert(map, 0, 3, VanillaContraptionID.furnace);
            Insert(map, 1, 3, VanillaContraptionID.woodenDropper);
            Insert(map, 2, 3, VanillaContraptionID.stoneDropper);

            Insert(map, 0, 4, VanillaContraptionID.woodenDropper);
            Insert(map, 1, 4, VanillaContraptionID.furnace);
            Insert(map, 2, 4, VanillaContraptionID.furnace);
        }
    }
}
