using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using MVZ2.GameContent.Armors;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.ShrunkenHead)]
    public class ShrunkenHead : ArtifactDefinition
    {
        private float timer = 0f;  // 计时器当前值
        private float interval = 5f;  // 计时间隔，单位秒，可以根据需求修改

        public ShrunkenHead(string nsp, string name) : base(nsp, name)
        {
        }

        // 假设ArtifactDefinition有一个类似Update的方法
        // 如果没有，你需要找到游戏主循环里调用的地方，比如OnTick、OnUpdate、OnArtifactUpdate等
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);

            // 如果有artifact提供deltaTime就用，没有则用固定时间模拟
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
                
                bool hostile = enemy.IsHostile(0);
                if (!hostile)
                    {
                    enemy.EquipMainArmor(VanillaArmorID.bone_helmet);
                    enemy.PlaySound(VanillaSoundID.armorUp);
                }
            }
        }
    }
}
