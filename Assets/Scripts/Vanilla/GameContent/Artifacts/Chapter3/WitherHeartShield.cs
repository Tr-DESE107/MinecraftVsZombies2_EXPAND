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
    // ������Ϊ WitherHeartShield ��������Artifact��������Ϊ�����黤�Ķܡ�
    [ArtifactDefinition(VanillaArtifactNames.WitherHeartShield)]
    public class WitherHeartShield : ArtifactDefinition
    {
        // ���캯����ע��⻷Ч���ͳ�ʼ����Ļص�
        public WitherHeartShield(string nsp, string name) : base(nsp, name)
        {
            // ��Ӹ������Ĺ⻷Ч��
            AddAura(new WitherHeartShieldAura());

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
                AuraEffect aura = artifact.GetAuraEffect<WitherHeartShieldAura>();
                if (aura == null)
                    continue;

                // ǿ�Ƹ��¹⻷��ʹ������Ӧ�õ�Ŀ����
                aura.UpdateAura();
            }
        }

        // �⻷�����ࣺWitherHeartShieldAura
        public class WitherHeartShieldAura : AuraEffectDefinition
        {
            public WitherHeartShieldAura()
            {
                BuffID = VanillaBuffID.WitherHeartShieldResistanceBuff; // ���ù⻷��Ӧ�� Buff ID���߼�Ч���� buff ������
            }

            // ����⻷Ӱ���Ŀ��ʵ�壨������еֲ�
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();  // ��ȡ��ǰ�ؿ�
                results.AddRange(level.GetEntities(EntityTypes.ENEMY)); // ��������е����⻷����Ŀ��
            }
        }
    }
}
