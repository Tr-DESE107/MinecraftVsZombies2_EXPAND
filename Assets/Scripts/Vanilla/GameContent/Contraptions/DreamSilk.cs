using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.dreamSilk)]
    public class DreamSilk : ContraptionBehaviour
    {
        public DreamSilk(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity silk)
        {
            base.Init(silk);
            silk.Timeout = silk.GetMaxTimeout();
            silk.PlaySound(VanillaSoundID.sparkle);
            var grid = silk.GetGrid();
            if (grid != null)
            {
                foreach (Entity entity in grid.GetEntities())
                {
                    if (!CanSleep(entity))
                        continue;
                    entity.AddBuff<DreamSilkBuff>();
                    break;
                }
            }
        }
        protected override void UpdateLogic(Entity silk)
        {
            base.UpdateLogic(silk);
            silk.Timeout--;
            var tint = silk.GetTint();
            tint.a = silk.Timeout / (float)silk.GetMaxTimeout();
            silk.SetTint(tint);
            if (silk.Timeout <= 0)
            {
                silk.Remove();
            }
        }
        public static bool CanSleep(Entity entity)
        {
            if (entity.Type != EntityTypes.PLANT)
                return false;
            if (entity.IsAIFrozen())
                return false;
            if (!entity.CanDeactive())
                return false;
            if (!entity.IsFriendlyEntity())
                return false;
            return true;
        }
        public override bool CanEvoke(Entity entity)
        {
            return false;
        }
    }
}
