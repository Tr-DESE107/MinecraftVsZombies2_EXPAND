#nullable enable  
  
using MVZ2Logic.Level;  
using PVZEngine.Definitions;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Stages  
{  
    [AutoStageDefinition(VanillaStageNames.InfinitySeija)] // 需在 VanillaStageNames 中新增该常量  
    public partial class InfinitySeija : StageDefinition  
    {  
        public InfinitySeija(string nsp, string name) : base(nsp, name)  
        {  
            var waveStageBehaviour = new WaveStageBehaviour(this);  
            waveStageBehaviour.SpawnFlagZombie = false;   // 不出旗帜波，全程 Boss 战  
            AddBehaviour(waveStageBehaviour);  
            AddBehaviour(new InfinitySeijaBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new StarshardStageBehaviour(this));  
            AddBehaviour(new RedstoneDropStageBehaviour(this));  
        }  
    }  
}
