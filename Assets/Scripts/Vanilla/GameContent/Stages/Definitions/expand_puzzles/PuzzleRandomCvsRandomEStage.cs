using MVZ2.GameContent.Contraptions;  
using MVZ2.Vanilla.Entities;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Callbacks;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Stages  
{  
    [StageDefinition(VanillaStageNames.puzzleRandomCvsRandomE)]  
    public class PuzzleRandomCvsRandomEStage : IZombiePuzzleStage  
    {  
        public PuzzleRandomCvsRandomEStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleRandomCvsRandomE)  
        {  
            // 添加实体初始化回调,过滤器设置为 CONTRAPTION 类型  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.PLANT);  
        }  
  
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)  
        {  
            var entity = param.entity;  
            var level = entity.Level;  
              
            // 检查是否是当前关卡  
            if (level.StageDefinition != this)  
                return;  
              
            // 检查是否是 random_china  
            if (entity.Definition.GetID() == VanillaContraptionID.randomChina)  
            {  
                // 方案1: 直接设置血量为 0(推荐)  
                //entity.Health = 0;  
                  
                // 方案2: 或者调用 Die 方法  
                entity.Die();  
            }  
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);

            
            level.SetEnergy(4000);
        }
    }  
}