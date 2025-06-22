using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class IZombieImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, 0, EntityTypes.ENEMY);
        }
        private void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (!level.IsIZombie())
                return;
            if (entity.Definition.HasBehaviour<MeleeEnemy>())
            {
                entity.AddBuff<IZombieAttackBoosterBuff>();
            }
            foreach (var buff in entity.GetBuffs<RandomEnemySpeedBuff>())
            {
                RandomEnemySpeedBuff.SetSpeed(buff, ZOMBIE_RANDOM_SPEED);
            }
        }
        public const float ZOMBIE_RANDOM_SPEED = 1.5f;
    }
}
