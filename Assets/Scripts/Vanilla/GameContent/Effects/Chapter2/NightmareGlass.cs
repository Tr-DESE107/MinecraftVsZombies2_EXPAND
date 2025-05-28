using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareGlass)]
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
            SetBreakTimeout(entity, 60);
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
                    entity.Level.ShakeScreen(10, 0, 15);
                    var buff = entity.AddBuff<WhiteFlashBuff>();
                    buff.SetProperty(WhiteFlashBuff.PROP_TIMEOUT, 2);
                    buff.SetProperty(WhiteFlashBuff.PROP_MAX_TIMEOUT, 2);
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
        public static readonly VanillaEntityPropertyMeta<int> PROP_BREAK_TIMEOUT = new VanillaEntityPropertyMeta<int>("BreakTimeout");
        public static readonly NamespaceID ID = VanillaEffectID.nightmareGlass;
    }
}