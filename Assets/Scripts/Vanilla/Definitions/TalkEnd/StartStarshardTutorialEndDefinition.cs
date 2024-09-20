using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla.TalkEnd
{
    [Definition(TalkEndNames.startStarshardTutorial)]
    public class StartStarshardTutorialEndDefinition : TalkEndDefinition
    {
        public StartStarshardTutorialEndDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Execute(LevelEngine level)
        {
            base.Execute(level);
            level.ChangeStage(StageID.starshard_tutorial);
            level.BeginLevel(LevelTransitions.TO_LAWN);
        }
    }
}
