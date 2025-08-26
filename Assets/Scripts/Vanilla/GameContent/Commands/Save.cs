using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using MVZ2Logic.Level;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.save)]
    public class Save : CommandDefinition
    {
        public Save(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var level = Global.Level.GetLevel();
            level.SaveStateData();
            PrintLine(Global.Localization.GetTextParticular(VanillaStrings.COMMAND_SAVE_SUCCESS, VanillaStrings.CONTEXT_COMMAND_OUTPUT));
        }
    }
}