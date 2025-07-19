using System;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.SeedPacks;
using MukioI18n;
using MVZ2.GameContent.Armors;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.ShrunkenHead)]
    public class ShrunkenHead : ArtifactDefinition
    {
        private float timer = 0f;  // ��ʱ����ǰֵ
        private float interval = 5f;  // ��ʱ�������λ�룬���Ը��������޸�

        public ShrunkenHead(string nsp, string name) : base(nsp, name)
        {
        }

        // ����ArtifactDefinition��һ������Update�ķ���
        // ���û�У�����Ҫ�ҵ���Ϸ��ѭ������õĵط�������OnTick��OnUpdate��OnArtifactUpdate��
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);

            // �����artifact�ṩdeltaTime���ã�û�����ù̶�ʱ��ģ��
            float deltaTime = UnityEngine.Time.deltaTime; 

            timer += deltaTime;

            if (timer >= interval)
            {
                timer = 0f;
                DoEffect(artifact);
                artifact.SetGlowing(true);
            }
        }

        private void DoEffect(Artifact artifact)
        {
            var level = artifact.Level;
            
            foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.ExistsAndAlive()))
            {
                enemy.EquipMainArmor(VanillaArmorID.bone_helmet);
                enemy.PlaySound(VanillaSoundID.armorUp);
            }
        }
    }
}
