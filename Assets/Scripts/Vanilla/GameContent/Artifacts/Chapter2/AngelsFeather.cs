using MVZ2.GameContent.Buffs.Enemies;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.AngelsFeather)]
    public class AngelsFeather : ArtifactDefinition
    {
        private float timer = 0f;  // ��ʱ����ǰֵ
        private float interval = 5f;  // ��ʱ�������λ�룬���Ը��������޸�

        public AngelsFeather(string nsp, string name) : base(nsp, name)
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

            foreach (var entity in level.FindEntities(e => e.ExistsAndAlive() && (e.Type == EntityTypes.ENEMY || e.Type == EntityTypes.PLANT)))
            {
                bool hostile = entity.IsHostile(0);
                if (!hostile)
                {
                    Buff buff = entity.GetFirstBuff<DivineShieldBuff>();
                    if (buff == null)
                    {
                        entity.AddBuff<DivineShieldBuff>();

                    }


                }
            }
        }
    }
}
