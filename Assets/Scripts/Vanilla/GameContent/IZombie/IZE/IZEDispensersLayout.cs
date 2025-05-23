using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeDispensers)]
    public class IZEDispensersLayout : IZELayout
    {
        public IZEDispensersLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.dispenser, 7, rng);
            RandomFill(map, VanillaContraptionID.drivenser, 2, rng);
            RandomFill(map, VanillaContraptionID.splitenser, 4, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 4, rng);
        }
    }
}
