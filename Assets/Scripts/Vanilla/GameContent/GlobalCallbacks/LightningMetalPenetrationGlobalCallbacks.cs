#nullable enable

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Shells;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;

using MVZ2Logic.Modding;

using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.GlobalCallbacks
{
    [ModGlobalCallbacks]
    public class LightningMetalPenetrationGlobalCallbacks : VanillaGlobalCallbacks
    {
        public override void Apply(Mod mod)
        {
            mod.AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            // 检查是否为电属性伤害  
            if (!damageInfo.Effects.HasEffect(VanillaDamageEffects.LIGHTNING))
                return;

            // 获取主盔甲  
            var armor = entity.GetMainArmor();
            if (!Armor.Exists(armor))
                return;

            // 获取盔甲材质  
            var shellDefinition = armor.GetShellDefinition();
            if (shellDefinition == null)
                return;

            // 检查是否为金属材质  
            if (shellDefinition.GetID() != VanillaShellID.metal)
                return;

            // 将伤害的一半设定为穿甲伤害  
            // 这里需要修改伤害效果，添加 IGNORE_ARMOR  
            // 由于 DamageEffectList 是不可变的，需要创建新的效果列表  
            var originalAmount = damageInfo.Amount;
            var OriDamage = originalAmount * 0.75f;
            var TGNDamage = originalAmount * 0.25f;

            // 设置原始伤害为一半  
            damageInfo.SetAmount(OriDamage);

            // 创建额外的穿甲伤害  
            var ignoreArmorEffects = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR);

            // 对实体造成额外的穿甲伤害  
            entity.TakeDamageSourced(TGNDamage, ignoreArmorEffects, damageInfo.Source, damageInfo.ShieldTarget);
        }
    }
}