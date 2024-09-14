using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.LevelManagement;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.obsidian)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.longTime)]
    public class Obsidian : VanillaContraption
    {
        public Obsidian(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.stone);
            SetProperty(EntityProperties.SIZE, new Vector3(48, 48, 48));
            SetProperty(EntityProperties.MAX_HEALTH, maxHP);
        }
        public override void Update(Entity contraption)
        {
            base.Update(contraption);
            var state = 0;
            if (contraption.HasBuff<ObsidianArmorBuff>())
            {
                state = GetArmoredHealthState(contraption);
                if (contraption.Health <= maxHP)
                {
                    contraption.RemoveBuffs(contraption.GetBuffs<ObsidianArmorBuff>());
                }
            }
            else
            {
                state = GetHealthState(contraption);
            }
            contraption.SetAnimationInt("HealthState", state);
        }

        public override bool CanEvoke(Entity entity)
        {
            if (entity.HasBuff<ObsidianArmorBuff>())
                return false;
            return base.CanEvoke(entity);
        }

        public override void Evoke(Entity contraption)
        {
            base.Evoke(contraption);
            contraption.AddBuff<ObsidianArmorBuff>();
            contraption.Health = contraption.GetMaxHealth();
            contraption.Level.PlaySound(SoundID.armorUp);
        }
        private int GetArmoredHealthState(Entity contraption)
        {
            switch (contraption.Health)
            {
                case > armoredHP * 2 / 3f + maxHP:
                    return 5;
                case > armoredHP / 3f + maxHP:
                    return 4;
                case > maxHP:
                    return 3;
                default:
                    return GetHealthState(contraption);
            }
        }
        private int GetHealthState(Entity contraption)
        {
            switch (contraption.Health)
            {
                case > maxHP * 2 / 3f:
                    return 2;
                case > maxHP / 3f:
                    return 1;
                default:
                    return 0;
            }
        }
        private const float maxHP = 4000;
        private const float armoredHP = 6000;
    }
}
