using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleDeadBalloon)]
    public class PuzzleDeadBalloonLayout : IZombieLayoutDefinition
    {
        public PuzzleDeadBalloonLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.gargoyle,
                VanillaEnemyID.ghast
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.pistenser, 2, rng);
            RandomFill(map, VanillaContraptionID.punchton, 5, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 3, rng);
            RandomFill(map, VanillaContraptionID.teslaCoil, 2, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
        }
    }
}
