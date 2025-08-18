using System.Linq;
using MVZ2.Vanilla;
using MVZ2Logic;
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

            if (parameters.Length <= 0)
            {
                var commands = game.GetAllCommandsID().OrderBy(i => game.GetCommandNameByID(i));
                foreach (var id in commands)
                {
                    var name = game.GetCommandNameByID(id);
                    var meta = game.GetCommandMeta(id);
                    var description = game.GetTextParticular(meta.GetDescription(), VanillaStrings.CONTEXT_COMMAND_DESCRIPTION);
                    var output = game.GetTextParticular(VanillaStrings.COMMAND_HELP_COMMAND_LIST_TEMPLATE, VanillaStrings.CONTEXT_COMMAND_OUTPUT, name, description);
                    PrintLine(output);
                }
                PrintLine();
                var details = game.GetTextParticular(VanillaStrings.COMMAND_HELP_DETAILS, VanillaStrings.CONTEXT_COMMAND_OUTPUT);
                PrintLine(details);
            }
            else
            {
                var commandName = parameters[0];
                var id = game.GetCommandIDByName(commandName);
                var meta = game.GetCommandMeta(id);

                PrintLine(commandName);
                foreach (var variant in meta.GetVariants())
                {
                    PrintLine();
                    PrintLine(variant.GetGrammarText(commandName));
                    PrintLine(game.GetTextParticular(variant.GetDescription(), VanillaStrings.CONTEXT_COMMAND_VARIANT_DESCRIPTION));
                    PrintLine();
                    foreach (var param in variant.GetParameters())
                    {
                        var paramName = param.GetName();
                        var desc = game.GetTextParticular(param.GetDescription(), VanillaStrings.CONTEXT_COMMAND_PARAMETER_DESCRIPTION);
                        var type = game.GetTextParticular(param.GetTypeString(), VanillaStrings.CONTEXT_COMMAND_PARAMETER_TYPE);
                        var msg = game.GetTextParticular(VanillaStrings.COMMAND_HELP_PARAMETER_TEMPLATE, VanillaStrings.CONTEXT_COMMAND_OUTPUT, paramName, type, desc);
                        PrintLine(msg);
                    }
                }
            }
        }
    }
}