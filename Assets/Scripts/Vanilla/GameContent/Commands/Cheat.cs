using System;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using PVZEngine.Buffs;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.cheat)]
    public class Cheat : CommandDefinition
    {
        public Cheat(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            string msg;
            string cheatNameKey;
            BuffDefinition buffDefinition = null;

            var cheatCode = parameters[0];
            switch (cheatCode)
            {
                case "recharge":
                    buffDefinition = game.GetBuffDefinition<DebugNoRechargeBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_RECHARGE;
                    break;
                case "starshard":
                    buffDefinition = game.GetBuffDefinition<DebugStarshardBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_STARSHARD;
                    break;
                default:
                    throw new ArgumentException(Global.Game.GetTextParticular(VanillaStrings.COMMAND_CHEAT_NOT_FOUND, VanillaStrings.CONTEXT_COMMAND_OUTPUT, cheatCode));
            }

            if (level.HasBuff(buffDefinition))
            {
                level.RemoveBuffs(buffDefinition);
                msg = VanillaStrings.COMMAND_CHEAT_DISABLED;
            }
            else
            {
                level.AddBuff(buffDefinition);
                msg = VanillaStrings.COMMAND_CHEAT_ENABLED;
            }
            var cheatName = Global.Game.GetTextParticular(cheatNameKey, VanillaStrings.CONTEXT_COMMAND_CHEAT_NAME);
            PrintLine(Global.Game.GetTextParticular(msg, VanillaStrings.CONTEXT_COMMAND_OUTPUT, cheatName));
        }
    }
}