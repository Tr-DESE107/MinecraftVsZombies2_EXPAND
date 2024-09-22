using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2.Vanilla.TalkEnd
{
    [Definition(TalkEndNames.startTutorial)]
    public class StartTutorialEndDefinition : TalkEndDefinition
    {
        public StartTutorialEndDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Execute(LevelEngine level)
        {
            base.Execute(level);
            level.ChangeStage(StageID.tutorial);
            level.BeginLevel(LevelTransitions.TO_LAWN);
        }
    }
}
