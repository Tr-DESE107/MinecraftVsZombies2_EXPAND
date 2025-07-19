using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    // ����һ��Ч���� NightmareaperTimer���󶨵���Ϸ����Ϊ "nightmareaperTimer" ��Ч��ʵ��
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareaperTimer)]
    public class NightmareaperTimer : EffectBehaviour
    {
        // ���캯�������������ռ�����֣����û��๹����
        public NightmareaperTimer(string nsp, string name) : base(nsp, name)
        {
        }

        // ��ʼ��ʱ���ã�����Ч����һ����Чʱ����
        public override void Init(Entity entity)
        {
            base.Init(entity);
            // ��ȡ��ǰ�ؿ���Level����Ϊ�������ո��ߣ�Nightmareaper�����趨�ĵ���ʱʱ��
            var timeout = entity.Level.GetNightmareaperTimeout();
            // ������ʱֵ���浽ʵ���������
            SetTimeout(entity, timeout);
        }

        // ÿ֡���µ��ã�����ˢ�¼�ʱ���߼�
        public override void Update(Entity entity)
        {
            base.Update(entity);
            // ���µ���ʱ�������Ӿ�Ч��
            UpdateTimer(entity);
        }

        // ��ʱ�����º��������Ƶ���ʱ�ݼ�������ʱ���������Ϊ
        private void UpdateTimer(Entity entity)
        {
            // ȡ����ǰ����ʱ��ֵ
            var timeout = GetTimeout(entity);
            if (timeout > 0)
            {
                // ÿ֡�ݼ�����ʱ
                timeout--;
                // �������ʱ������<=0����������ŭ�ո��ߵ�Ч��
                if (timeout <= 0)
                {
                    EnrageReapers(entity);
                }
                // ���浹��ʱ���º��ֵ
                SetTimeout(entity, timeout);
            }

            // ����ʣ��ʱ�����ʵ��ģ����ɫ���Ӻ�ɫ����ɫ���䣩
            // timeout/900f ��ζ����󵹼�ʱΪ900֡������ʱԽ��Խƫ�죬Խ��Խƫ��
            Color tint = Color.Lerp(Color.red, Color.white, timeout / 900f);
            // ��ʵ��ģ��������ɫ���ԣ������Ӿ���ʾ
            entity.SetModelProperty("Color", tint);
            // ͬʱ����ǰ����ʱ����ģ�����ԣ�����ǰ�˻򶯻�ʹ��
            entity.SetModelProperty("Timeout", timeout);
        }

        // ��ŭ�����ո��ߵĴ�����
        private void EnrageReapers(Entity entity)
        {
            // ��ȡ��ǰ�ؿ�����
            var level = entity.Level;
            // ֹͣ��ǰ���ŵ�����
            level.StopMusic();
            // ������ǰ�ؿ������� Nightmareaper ���͵�ʵ��
            foreach (Entity nightmareaper in level.FindEntities(VanillaBossID.nightmareaper))
            {
                // ���� Nightmareaper �ļ�ŭ״̬
                Nightmareaper.Enrage(nightmareaper);
                // ����ʵ����Ӽ�ŭ״̬�� Buff������Ч����
                nightmareaper.AddBuff<NightmareaperEnragedBuff>();
            }
        }

        // ��ȡ��ʵ���ʱ���ĳ�ʱ���ԣ�����ʱ��ֵ��
        public static int GetTimeout(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_TIMEOUT);
        // ���ø�ʵ���ʱ���ĳ�ʱ���ԣ�����ʱ��ֵ��
        public static void SetTimeout(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_TIMEOUT, value);

        // ����һ������Ԫ���ݣ���ʾ����ʱ����
        public static readonly VanillaEntityPropertyMeta<int> PROP_TIMEOUT = new VanillaEntityPropertyMeta<int>("Timeout");
        // �����Ч����Ψһ��ʶ ID
        public static readonly NamespaceID ID = VanillaEffectID.nightmareaperTimer;
    }
}
