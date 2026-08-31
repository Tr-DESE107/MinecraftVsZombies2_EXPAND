#nullable enable  
  
using MVZ2.Vanilla.Entities;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic.Entities;  
using PVZEngine.Buffs;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Modifiers;  
using UnityEngine;  
  
namespace MVZ2.GameContent.Buffs.Enemies  
{  
    [AutoBuffDefinition(VanillaBuffNames.Enemy.AncientGhostBuff)]  
    public class AncientGhostBuff : BuffDefinition  
    {  
        public AncientGhostBuff(string nsp, string name) : base(nsp, name)  
        {  
            // 只保留透明度效果：将实体的 TINT 乘上一个 alpha 可变的颜色  
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULTIPLIER));  
            // 隐身时隐藏血条；buff 被移除时该修饰器会自动撤销，血条恢复正常  
            var hpbarModifier = new IntModifier(LogicEntityProps.HP_BAR_VISIBILITY, IntegerOperator.Set, HPBarVisibility.HIDDEN);  
            AddModifier(hpbarModifier);  
        }  
  
        public override void OnCreate(Buff buff)  
        {  
            base.OnCreate(buff);  
            // 初始完全透明（alpha = 0），没有任何减伤 / ETHEREAL 效果  
            buff.SetProperty(PROP_TINT_MULTIPLIER, new Color(1, 1, 1, TINT_ALPHA_MIN));  
        }  
  
        public override void PostAdd(Buff buff)  
        {  
            base.PostAdd(buff);  
            UpdateIllumination(buff);  
        }  
  
        public override void PostUpdate(Buff buff)  
        {  
            base.PostUpdate(buff);  
            UpdateIllumination(buff);  
        }  
  
        private void UpdateIllumination(Buff buff)  
        {  
            var entity = buff.GetEntity();  
            if (entity == null)  
                return;  
  
            // 只判断是否被光源照亮（不含白天）  
            bool illuminated = entity.IsIlluminated();  
  
            float tintSpeed = illuminated ? TINT_SPEED : -TINT_SPEED;  
            var tint = buff.GetProperty<Color>(PROP_TINT_MULTIPLIER);  
            tint.a = Mathf.Clamp(tint.a + tintSpeed, TINT_ALPHA_MIN, TINT_ALPHA_MAX);  
            buff.SetProperty(PROP_TINT_MULTIPLIER, tint);  
        }  
  
        public static readonly VanillaBuffPropertyMeta<Color> PROP_TINT_MULTIPLIER = new VanillaBuffPropertyMeta<Color>("TintMultiplier");  
  
        public const float TINT_ALPHA_MIN = 0f;   // 未照亮：完全透明  
        public const float TINT_ALPHA_MAX = 1f;   // 照亮后：完全可见  
        public const float TINT_SPEED = 0.02f;    // 渐变速度  
    }  
}
