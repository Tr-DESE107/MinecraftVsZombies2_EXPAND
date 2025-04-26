using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostHead)]
    public class HostHead : MeleeEnemy
    {
        public HostHead(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            // 添加飞行Buff
            var fly = entity.AddBuff<FlyBuff>();
            if (fly != null)
            {
                fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 1);
            }

            // 添加回血Buff
            var regen = entity.Level.CreateBuff<RegenerationBuff>(60000L); // 显式指定泛型类型
            if (regen != null)
            {
                entity.AddBuff(regen);
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 2f); // 每次回血2点
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000);  // 持续60000帧
            }
        }
    }
}
