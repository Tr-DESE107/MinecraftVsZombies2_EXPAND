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
            // ���ʵ���ʼ���ص�,����������Ϊ CONTRAPTION ����  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.PLANT);  
        }  
  
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)  
        {  
            var entity = param.entity;  
            var level = entity.Level;  
              
            // ����Ƿ��ǵ�ǰ�ؿ�  
            if (level.StageDefinition != this)  
                return;  
              
            // ����Ƿ��� random_china  
            if (entity.Definition.GetID() == VanillaContraptionID.randomChina)  
            {  
                // ����1: ֱ������Ѫ��Ϊ 0(�Ƽ�)  
                //entity.Health = 0;  
                  
                // ����2: ���ߵ��� Die ����  
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