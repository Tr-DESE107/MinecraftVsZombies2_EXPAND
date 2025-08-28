﻿using System;
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
            var level = Global.Level.GetLevel();

            string msg;
            string cheatNameKey;
            BuffDefinition buffDefinition = null;

            var cheatCode = parameters[0];
            switch (cheatCode)
            {
                case "godmode":
                    buffDefinition = game.GetBuffDefinition<DebugGodmodeBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_GODMODE;
                    break;
                case "energy":
                    buffDefinition = game.GetBuffDefinition<DebugEnergyBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_ENERGY;
                    break;
                case "recharge":
                    buffDefinition = game.GetBuffDefinition<DebugNoRechargeBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_RECHARGE;
                    break;
                case "starshard":
                    buffDefinition = game.GetBuffDefinition<DebugStarshardBuff>();
                    cheatNameKey = VanillaStrings.CHEAT_NAME_STARSHARD;
                    break;
                default:
                    throw new ArgumentException(Global.Localization.GetTextParticular(VanillaStrings.COMMAND_CHEAT_NOT_FOUND, VanillaStrings.CONTEXT_COMMAND_OUTPUT, cheatCode));
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
            var cheatName = Global.Localization.GetTextParticular(cheatNameKey, VanillaStrings.CONTEXT_COMMAND_CHEAT_NAME);
            PrintLine(Global.Localization.GetTextParticular(msg, VanillaStrings.CONTEXT_COMMAND_OUTPUT, cheatName));
        }
    }
}