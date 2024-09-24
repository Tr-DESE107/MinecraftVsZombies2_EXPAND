using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Games;
using PVZEngine.Level;

namespace MVZ2.Vanilla.TalkEnd
{
    [Definition(TalkEndNames.startTriggerTutorial)]
    public class StartTriggerEndDefinition : TalkEndDefinition
    {
        public StartTriggerEndDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Execute()
        {
            base.Execute();
            var game = Global.Game;
            if (!game.IsInLevel())
                return;
            var level = game.GetLevel();
            level.ChangeStage(StageID.trigger_tutorial);
            level.BeginLevel(LevelTransitions.TO_LAWN);
        }
    }
}
