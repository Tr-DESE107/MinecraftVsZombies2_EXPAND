using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.Entity.ExplosionProtection)]
    public class ExplosionProtection : BuffDefinition
    {
        public ExplosionProtection(string nsp, string name) : base(nsp, name)
        {
            // ע��ص�����ʵ�弴���ܵ��˺�ǰ����
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback, priority: -100);
        }

        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            // ��ʵ���ȡ ExplosionProtection buffʵ��  
            var buff = entity.GetFirstBuff<ExplosionProtection>();
            if (buff == null)
                return;

            // ����˺�����"��ը"Ч����������˺�  
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.EXPLOSION))
            {
                var level = buff.GetProperty<float>(PROP_Protection_Level); // ��ȷָ�����Ͳ���  
                damageInfo.Multiply(1f - level); // ����level��float����  
            }
        }

        // ����һ�� Buff ����Ԫ���ݣ����ڴ洢�����ȼ�
        public static readonly VanillaBuffPropertyMeta<float> PROP_Protection_Level =
            new VanillaBuffPropertyMeta<float>("Protection_Level");
    }
}
