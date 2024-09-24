using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2.Vanilla.TalkEnd
{
    [Definition(TalkEndNames.startStarshardTutorial)]
    public class StartStarshardTutorialEndDefinition : TalkEndDefinition
    {
        public StartStarshardTutorialEndDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Execute()
        {
            base.Execute();
            var game = Global.Game;
            if (!game.IsInLevel())
                return;
            var level = game.GetLevel();
            level.ChangeStage(StageID.starshard_tutorial);
            level.BeginLevel(LevelTransitions.TO_LAWN);
        }
    }
}
