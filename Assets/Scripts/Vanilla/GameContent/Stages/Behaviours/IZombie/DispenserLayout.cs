using MVZ2.GameContent.Contraptions;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class DispenserLayout : IZombieLayout
    {
        public DispenserLayout(int columns) : base(columns)
        {
        }
        public override void Fill(IZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.dispenser, 5, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 4, rng);
            RandomFill(map, VanillaContraptionID.punchton, 3, rng);
        }
    }
}
