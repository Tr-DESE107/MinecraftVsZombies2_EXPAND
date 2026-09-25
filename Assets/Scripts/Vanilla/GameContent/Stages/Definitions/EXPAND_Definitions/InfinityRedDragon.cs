#nullable enable  
  
using MVZ2Logic.Level;  
using PVZEngine.Definitions;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.InfinityRedDragon)] // TODO: 需在 VanillaStageNames 中新增该常量  
    public partial class InfinityRedDragon : StageDefinition  
    {  
        public InfinityRedDragon(string nsp, string name) : base(nsp, name)  
        {  
            var waveStageBehaviour = new WaveStageBehaviour(this);  
            waveStageBehaviour.SpawnFlagZombie = false;  
            AddBehaviour(waveStageBehaviour);  
            AddBehaviour(new InfinityRedDragonBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new StarshardStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
        }  
    }  
}
