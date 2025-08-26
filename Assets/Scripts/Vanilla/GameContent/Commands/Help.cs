using System.Linq;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Debugs;
using MVZ2Logic.Games;
using MVZ2Logic.IZombie;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.help)]
    public class Help : CommandDefinition
    {
        public Help(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var localization = Global.Localization;
            var debugs = Global.Debugs;

            if (parameters.Length <= 0)
            {
                var commands = debugs.GetAllCommandsID().OrderBy(i => debugs.GetCommandNameByID(i));
                foreach (var id in commands)
                {
                    var name = debugs.GetCommandNameByID(id);
                    var def = game.GetCommandDefinition(id);
                    var description = localization.GetTextParticular(def.GetDescription(), VanillaStrings.CONTEXT_COMMAND_DESCRIPTION);
                    var output = localization.GetTextParticular(VanillaStrings.COMMAND_HELP_COMMAND_LIST_TEMPLATE, VanillaStrings.CONTEXT_COMMAND_OUTPUT, name, description);
                    PrintLine(output);
                }
                PrintLine();
                var details = localization.GetTextParticular(VanillaStrings.COMMAND_HELP_DETAILS, VanillaStrings.CONTEXT_COMMAND_OUTPUT);
                PrintLine(details);
            }
            else
            {
                var commandName = parameters[0];
                var id = debugs.GetCommandIDByName(commandName);
                var def = game.GetCommandDefinition(id);

                PrintLine(commandName);
                foreach (var variant in def.GetVariants())
                {
                    PrintLine();
                    PrintLine(variant.GetGrammarText(commandName));
                    PrintLine(localization.GetTextParticular(variant.Description, VanillaStrings.CONTEXT_COMMAND_VARIANT_DESCRIPTION));
                    PrintLine();
                    foreach (var param in variant.Parameters)
                    {
                        var paramName = param.Name;
                        var desc = localization.GetTextParticular(param.Description, VanillaStrings.CONTEXT_COMMAND_PARAMETER_DESCRIPTION);
                        var type = localization.GetTextParticular(param.GetTypeName(), VanillaStrings.CONTEXT_COMMAND_PARAMETER_TYPE);
                        var msg = localization.GetTextParticular(VanillaStrings.COMMAND_HELP_PARAMETER_TEMPLATE, VanillaStrings.CONTEXT_COMMAND_OUTPUT, paramName, type, desc);
                        PrintLine(msg);
                    }
                }
            }
        }
    }
}