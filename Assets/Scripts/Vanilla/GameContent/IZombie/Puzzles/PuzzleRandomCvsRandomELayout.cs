using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleRandomCvsRandomE)]
    public class PuzzleRandomCvsRandomELayout : IZombieLayoutDefinition
    {
        public PuzzleRandomCvsRandomELayout(string nsp, string name) : base(nsp, name, 8)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.RandomZombie,
                VanillaEnemyID.RandomMutant,
                VanillaEnemyID.RandomImp,
                
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.randomChina, 40, rng);


        }
    }
}
