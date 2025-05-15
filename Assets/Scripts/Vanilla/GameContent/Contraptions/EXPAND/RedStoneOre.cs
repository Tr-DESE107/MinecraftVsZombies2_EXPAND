using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Damages;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Difficulties;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.RedStoneOre)]
    public class RedStoneOre : ContraptionBehaviour
    {
        public RedStoneOre(string nsp, string name) : base(nsp, name)
        {

        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var Orecount = 12;
            if (entity.Level.Difficulty == VanillaDifficulties.normal)
            {
                 Orecount = 10;
            }
            else if (entity.Level.Difficulty == VanillaDifficulties.hard)
            {
                 Orecount = 8;
            }
            else if (entity.Level.Difficulty == VanillaDifficulties.lunatic)
            {
                 Orecount = 6;
            }
            for (var i = 0; i < Orecount; i++)
            {
                entity.Produce(VanillaPickupID.redstone);
            }

        }
    }
}
