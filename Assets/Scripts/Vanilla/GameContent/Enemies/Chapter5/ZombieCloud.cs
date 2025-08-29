using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.zombieCloud)]
    public class ZombieCloud : StateEnemy
    {
        public ZombieCloud(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 80f);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.TINT, DEATH_SMOKE_COLOR);
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter(), param);
            entity.Remove();
        }
        public static readonly Color DEATH_SMOKE_COLOR = new Color32(241, 191, 227, 255);
    }
}
