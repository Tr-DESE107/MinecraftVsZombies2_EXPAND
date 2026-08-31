#nullable enable

using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [AutoStageDefinition(VanillaStageNames.BalloonParty)]
    public partial class BalloonParty : StageDefinition
    {
        public BalloonParty(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));

            // 敌人初始化时触发，只处理敌方怪物  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);
        }

        private void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;

            // 只在本关卡实例生效  
            if (level.StageDefinition != this)
                return;

            // 气球本身也是敌人实体，跳过以避免递归  
            if (entity.IsEntityOf(VanillaEnemyID.lockedChestBalloon))
                return;

            // 已经在飞的怪物（幽灵、飞碟、恶魂等）不再挂气球  
            if (entity.Definition.IsFlyingEnemy())
                return;

            // 已经被别的气球吊住了就不重复挂  
            if (entity.HasBuff(VanillaBuffID.Entity.draggedByBalloon))
                return;

            AttachBalloon(entity);
        }

        // 需要额外 using：  
        // using MVZ2.GameContent.Buffs.Enemies;   // FlyBuff  
        // using PVZEngine.Buffs;  

        private const float BALLOON_TARGET_HEIGHT = 120f; // 想要的更低高度，原版是 120f  

        private void AttachBalloon(Entity enemy)
        {
            var level = enemy.Level;
            level.Spawn(VanillaEnemyID.lockedChestBalloon, enemy.GetCenter(), null)?.Let(balloon =>
            {
                balloon.SetParent(enemy);
                balloon.PlaySound(VanillaSoundID.balloonInflate);

                // 覆盖这只气球的目标飞行高度，只影响本关，不动 Balloon.cs  
                foreach (var fly in balloon.GetBuffs<FlyBuff>())
                {
                    fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, BALLOON_TARGET_HEIGHT);
                }
            });
        }
    }
}
