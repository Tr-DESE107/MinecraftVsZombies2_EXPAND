using MVZ2.Extensions;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.obsidian)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, VanillaRechargeNames.longTime)]
    public class Obsidian : VanillaContraption
    {
        public Obsidian(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity contraption)
        {
            base.Update(contraption);
            var state = 0;
            var maxHP = contraption.GetMaxHealth();
            if (contraption.HasBuff<ObsidianArmorBuff>())
            {
                state = GetArmoredHealthState(contraption, maxHP);
                if (contraption.Health <= maxHP * 0.4f)
                {
                    contraption.RemoveBuffs(contraption.GetBuffs<ObsidianArmorBuff>());
                }
            }
            else
            {
                state = GetHealthState(contraption, maxHP);
            }
            contraption.SetAnimationInt("HealthState", state);
        }

        public override bool CanEvoke(Entity entity)
        {
            if (entity.HasBuff<ObsidianArmorBuff>())
                return false;
            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
            contraption.AddBuff<ObsidianArmorBuff>();
            contraption.Health = contraption.GetMaxHealth();
            contraption.Level.PlaySound(SoundID.armorUp);
        }
        private int GetArmoredHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= 0.4f * maxHP)
            {
                return GetHealthState(contraption, maxHP * 0.4f);
            }
            else if (contraption.Health <= 0.6f * maxHP)
            {
                return 3;
            }
            else if (contraption.Health <= 0.8f * maxHP)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }
        private int GetHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= maxHP / 3)
            {
                return 0;
            }
            else if (contraption.Health <= maxHP * 2 / 3)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
