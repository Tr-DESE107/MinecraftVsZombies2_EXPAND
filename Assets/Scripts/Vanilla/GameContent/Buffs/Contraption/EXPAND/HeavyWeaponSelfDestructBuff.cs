#nullable enable  

using MVZ2.GameContent.Contraptions;  
using MVZ2.Vanilla.Entities;  
using MVZ2.Vanilla.Properties;  
using PVZEngine.Buffs;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Modifiers;  
  
namespace MVZ2.GameContent.Buffs.Contraptions  
{  
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponSelfDestruct)]  // 需在 VanillaBuffNames.Contraption 里加此常量  
    public class HeavyWeaponSelfDestructBuff : BuffDefinition  
    {  
        public HeavyWeaponSelfDestructBuff(string nsp, string name) : base(nsp, name)  
        {  
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));  // 倒计时期间无敌  
        }  
        public override void PostAdd(Buff buff) { base.PostAdd(buff); buff.SetProperty(PROP_TIMEOUT, COUNTDOWN); }  
        public override void PostUpdate(Buff buff)  
        {  
            base.PostUpdate(buff);  
            var t = buff.GetProperty<int>(PROP_TIMEOUT) - 1;  
            buff.SetProperty(PROP_TIMEOUT, t);  
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
        public const int COUNTDOWN = 180;   // 3 秒  
    }  
}
