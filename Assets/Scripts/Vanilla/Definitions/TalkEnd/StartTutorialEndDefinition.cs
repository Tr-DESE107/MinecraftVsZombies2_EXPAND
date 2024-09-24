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
        public override void Execute()
        {
            base.Execute();
            var game = Global.Game;
            if (!game.IsInLevel())
                return;
            var level = game.GetLevel();
            level.ChangeStage(StageID.tutorial);
            level.BeginLevel(LevelTransitions.TO_LAWN);
        }
    }
}
