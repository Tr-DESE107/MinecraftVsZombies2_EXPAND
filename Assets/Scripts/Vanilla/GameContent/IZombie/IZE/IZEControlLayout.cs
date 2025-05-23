using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeControl)]
    public class IZEControlLayout : IZELayout
    {
        public IZEControlLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.hellfire, 1, rng);
            RandomFill(map, VanillaContraptionID.drivenser, 1, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 3, rng);
            RandomFill(map, VanillaContraptionID.stoneDropper, 2, rng);
            RandomFill(map, VanillaContraptionID.splitenser, 3, rng);
            RandomFill(map, VanillaContraptionID.woodenDropper, 4, rng);
            RandomFill(map, VanillaContraptionID.soulFurnace, 1, rng);
            RandomFill(map, VanillaContraptionID.dreamCrystal, 2, rng);
            RandomFill(map, VanillaContraptionID.gravityPad, 2, rng);
        }
    }
}
