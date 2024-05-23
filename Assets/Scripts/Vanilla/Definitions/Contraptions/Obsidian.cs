using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.obsidian)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.longTime)]
    public class Obsidian : VanillaContraption
    {
        public Obsidian() : base()
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.stone);
            SetProperty(EntityProperties.SIZE, new Vector3(48, 48, 48));
            SetProperty(EntityProperties.MAX_HEALTH, maxHP);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var contraption = entity.ToContraption();
            var state = 0;
            switch (entity.Health)
            {
                case > armoredHP * 2 / 3f + maxHP:
                    state = 3;
                    break;
                case > armoredHP / 3f + maxHP:
                    state = 4;
                    break;
                case > maxHP:
                    state = 5;
                    break;
                case > maxHP * 2 / 3f:
                    state = 0;
                    break;
                case > maxHP / 3f:
                    state = 1;
                    break;
                default:
                    state = 2;
                    break;
            }
            contraption.SetAnimationInt("State", state);
        }

        public override void Evoke(Contraption contraption)
        {
            base.Evoke(contraption);
        }
        private const float maxHP = 4000;
        private const float armoredHP = 6000;
    }
}
