using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Enemies;
using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.RaiderSkull)]
    public class RaiderSkull : MeleeEnemy
    {
        public RaiderSkull(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var fly = entity.AddBuff<FlyBuff>();
            fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 20f);
        }

        protected virtual bool CanFly(Entity enemy)
        {
            return (enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1) && enemy.Position.x > enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 5));
        }

        protected override void UpdateAI(Entity enemy)
        {
            if (CanFly(enemy))
            {
                var fly = enemy.AddBuff<FlyBuff>();
                fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 500f);
            }
            else
            {
                var fly = enemy.AddBuff<FlyBuff>();
                fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 20f);
            }


            base.UpdateAI(enemy);
        }
    }
}
