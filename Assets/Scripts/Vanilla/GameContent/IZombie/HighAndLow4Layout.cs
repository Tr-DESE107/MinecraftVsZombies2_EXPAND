using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.highAndLow4)]
    public class HighAndLow4Layout : IZombieLayoutDefinition
    {
        public HighAndLow4Layout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.ghast,
                VanillaEnemyID.caveSpider
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
            RandomFill(map, VanillaContraptionID.pistenser, 3, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 2, rng);
            RandomFill(map, VanillaContraptionID.dispenser, 7, rng);
            RandomFill(map, VanillaContraptionID.gravityPad, 4, rng);
        }
    }
}
