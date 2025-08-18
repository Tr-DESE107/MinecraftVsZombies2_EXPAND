using MVZ2Logic;
using MVZ2Logic.IZombie;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.energy)]
    public class Energy : CommandDefinition
    {
        public Energy(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            var mode = parameters[0];
            var amount = ParseHelper.ParseFloat(parameters[1]);
            if (mode == "set")
            {
                level.SetEnergy(amount);
            }
            else if (mode == "add")
            {
                level.AddEnergy(amount);
            }
        }
    }
}