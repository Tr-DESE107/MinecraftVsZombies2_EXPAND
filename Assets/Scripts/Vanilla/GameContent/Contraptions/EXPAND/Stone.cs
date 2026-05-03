#nullable enable  
  
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Entities;  
using PVZEngine.Entities;  
using PVZEngine.Level;
using PVZEngine.Definitions;  
  
namespace MVZ2.GameContent.Contraptions  
{  
    [AutoEntityBehaviourDefinition(VanillaContraptionNames.Stone)]  
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
            // ����Ѫ���������𶯻�  
            contraption.SetModelDamagePercent();  
        }  
  
        // ���ܱ�����ǿ��  
        public override bool CanEvoke(Entity entity)  
        {  
            return false;  
        }  
    }  
}