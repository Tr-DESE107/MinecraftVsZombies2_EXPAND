#nullable enable  
  
using MVZ2.Vanilla.Entities;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Contraptions  
{  
    [EntityBehaviourDefinition(VanillaContraptionNames.Stone)]  
    public class Stone : ContraptionBehaviour  
    {  
        public Stone(string nsp, string name) : base(nsp, name) { }  
  
        public override void Init(Entity entity)  
        {  
            base.Init(entity);  
        }  
  
        protected override void UpdateLogic(Entity contraption)  
        {  
            base.UpdateLogic(contraption);  
            // 根据血量更新受损动画  
            contraption.SetModelDamagePercent();  
        }  
  
        // 不能被大招强化  
        public override bool CanEvoke(Entity entity)  
        {  
            return false;  
        }  
    }  
}