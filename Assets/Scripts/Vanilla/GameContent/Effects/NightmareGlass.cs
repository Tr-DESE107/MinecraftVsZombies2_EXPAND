using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.nightmareGlass)]
    public class NightmareGlass : EffectBehaviour
    {

        #region 公有方法
        public NightmareGlass(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.screenCover);
            SetBreakTimeout(entity, 30);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var breakTimeout = GetBreakTimeout(entity);
            if (breakTimeout > 0)
            {
                breakTimeout--;
                SetBreakTimeout(entity, breakTimeout);
                if (breakTimeout <= 0)
                {
                    entity.TriggerModel("Break");
                    entity.PlaySound(VanillaSoundID.glassBreakBig);
                }
            }

            if (entity.Timeout <= 30)
            {
                var tint = entity.GetTint();
                tint.a = entity.Timeout / 30f;
                entity.SetTint(tint);
            }
        }
        public static int GetBreakTimeout(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_BREAK_TIMEOUT);
        public static void SetBreakTimeout(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_BREAK_TIMEOUT, value);
        public const string PROP_BREAK_TIMEOUT = "BreakTimeout";
        public static readonly NamespaceID ID = VanillaEffectID.nightmareGlass;
    }
}