#nullable enable  

using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Buffs.Contraptions  
{  
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponSelfDestruct)]  // 需在 VanillaBuffNames.Contraption 里加此常量  
    public class HeavyWeaponSelfDestructBuff : BuffDefinition  
    {  
        public HeavyWeaponSelfDestructBuff(string nsp, string name) : base(nsp, name)  
        {  
        }  
        public override void PostAdd(Buff buff) { base.PostAdd(buff); buff.SetProperty(PROP_TIMEOUT, COUNTDOWN); }  
        public override void PostUpdate(Buff buff)  
        {  
            base.PostUpdate(buff);  
            var t = buff.GetProperty<int>(PROP_TIMEOUT) - 1;  
            buff.SetProperty(PROP_TIMEOUT, t);
            if (t == 30) {
                var e = buff.GetEntity();
                e?.PlaySound(VanillaSoundID.parabotTick);
            }
            if (t <= 0)  
            {  
                var e = buff.GetEntity();
                buff.Remove();  
                if (e != null && !e.IsDead)  
                {  
                    Nuke.Explode(e, 280, e.GetMaxHealth() * 5);  
                    Nuke.ExplodeEffects(e);  
                    e.Die();  
                }  
            }  
        }  
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");  
        public const int COUNTDOWN = 150;   // 5 秒  
    }  
}
