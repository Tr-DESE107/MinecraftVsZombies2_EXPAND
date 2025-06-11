﻿using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.parasiteTerror)]
    public class ParasiteTerror : MeleeEnemy
    {
        public ParasiteTerror(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            // 设置血量状态。
            entity.SetModelDamagePercent();
        }
    }
}
