#nullable enable  
  
using MVZ2Logic.Level;  
using PVZEngine.Definitions;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.InfinityLockedChest)] // TODO: 需在 VanillaStageNames 中新增该常量  
    public partial class InfinityLockedChest : StageDefinition  
    {  
        public InfinityLockedChest(string nsp, string name) : base(nsp, name)  
        {  
            var waveStageBehaviour = new WaveStageBehaviour(this);  
            waveStageBehaviour.SpawnFlagZombie = false;  
            AddBehaviour(waveStageBehaviour);  
            AddBehaviour(new InfinityLockedChestBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new StarshardStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
        }  
    }  
}
