using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.obsidian)]
    public class Obsidian : ContraptionBehaviour
    {
        public Obsidian(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);
            var state = 0;
            var maxHP = contraption.GetMaxHealth();
            if (contraption.HasBuff<ObsidianArmorBuff>())
            {
                state = GetArmoredHealthState(contraption, maxHP);
                if (contraption.Health <= maxHP * 0.4f)
                {
                    contraption.RemoveBuffs<ObsidianArmorBuff>();
                }
            }
            else
            {
                state = GetHealthState(contraption, maxHP);
            }
            contraption.SetModelHealthState(state);
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
            contraption.Level.PlaySound(VanillaSoundID.armorUp);
        }
        private int GetArmoredHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= 0.4f * maxHP)
            {
                return GetHealthState(contraption, maxHP * 0.4f);
            }
            else if (contraption.Health <= 0.6f * maxHP)
            {
                return 5;
            }
            else if (contraption.Health <= 0.8f * maxHP)
            {
                return 4;
            }
            else
            {
                return 3;
            }
        }
        private int GetHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= maxHP / 3)
            {
                return 2;
            }
            else if (contraption.Health <= maxHP * 2 / 3)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
