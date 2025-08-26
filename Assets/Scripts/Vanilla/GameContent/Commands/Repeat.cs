using System.Linq;
using MVZ2Logic;
using MVZ2Logic.Command;
using MVZ2Logic.IZombie;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.repeat)]
    public class Repeat : CommandDefinition
    {
        public Repeat(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var count = ParseHelper.ParseInt(parameters[0]);
            var last = Global.Debugs.GetCommandHistory().FirstOrDefault(h =>
            {
                var parts = CommandUtility.SplitCommand(h);
                return parts.Length < 1 || Global.Debugs.GetCommandIDByName(parts[0]) != GetID();
            });
            if (string.IsNullOrEmpty(last))
                return;
            for (int i = 0; i < count; i++)
            {
                Global.Debugs.ExecuteCommand(last);
            }
        }
    }
}