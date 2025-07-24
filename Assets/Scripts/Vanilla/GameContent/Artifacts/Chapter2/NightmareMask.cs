using System.Collections.Generic;
using MVZ2.GameContent.Buffs;       // ��Ϸ�Զ��� Buff ϵͳ�����ռ�
using MVZ2Logic;                    // MVZ2 ��Ϸ�����߼�
using MVZ2Logic.Artifacts;         // MVZ2 �� Artifact�����������߼�����
using MVZ2Logic.Level;             // ��Ϸ�ؿ��߼�
using PVZEngine.Auras;             // �⻷��Aura��ϵͳ
using PVZEngine.Buffs;             // Buff ϵͳ
using PVZEngine.Callbacks;         // �ص�����ϵͳ
using PVZEngine.Entities;          // ʵ��ϵͳ������ֲ���ʬ����е�ȣ�

namespace MVZ2.GameContent.Artifacts
{
    // ������Ϊ NightmareMask ��������Artifact��������Ϊ������֮�桱
    [ArtifactDefinition(VanillaArtifactNames.NightmareMask)]
    public class NightmareMask : ArtifactDefinition
    {
        // ���캯����ע��⻷Ч���ͳ�ʼ����Ļص�
        public NightmareMask(string nsp, string name) : base(nsp, name)
        {
            // ��Ӹ������Ĺ⻷Ч��
            AddAura(new NightmareMaskAura());

            // ��ʵ���ʼ����ɺ󣬴����ص������� EntityTypes.ENEMY ������Ч��
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.ENEMY);
        }

        // ÿ֡�����У����������з�����Ч�����Ӿ�Ч����
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true); // ������������
        }

        // �ص�����������е��ENEMY��ʵ���ʼ��֮�󣬸��¹⻷Ч��
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;              // ��ȡ�ճ�ʼ����ʵ��
            var level = contraption.Level;               // ��ȡʵ�����ڵĹؿ�

            // ������������
            foreach (var artifact in level.GetArtifacts())
            {
                // �������Ϊ�ջ��ǡ������������
                if (artifact == null || artifact.Definition != this)
                    continue;

                // ��ȡ������Ӧ�Ĺ⻷Ч��
                AuraEffect aura = artifact.GetAuraEffect<NightmareMaskAura>();
                if (aura == null)
                    continue;

                // ǿ�Ƹ��¹⻷��ʹ������Ӧ�õ�Ŀ����
                aura.UpdateAura();
            }
        }

        // �⻷�����ࣺNightmareMaskAura
        public class NightmareMaskAura : AuraEffectDefinition
        {
            public NightmareMaskAura()
            {
                BuffID = VanillaBuffID.NightmareMaskEvade; // ���ù⻷��Ӧ�� Buff ID���߼�Ч���� buff ������
            }

            // ����⻷Ӱ���Ŀ��ʵ��
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();  // ��ȡ��ǰ�ؿ�
                results.AddRange(level.GetEntities(EntityTypes.ENEMY)); // �����й������⻷����Ŀ��
            }
        }
    }
}
