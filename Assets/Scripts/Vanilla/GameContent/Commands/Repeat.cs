using System;
using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

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
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            var count = ParseHelper.ParseInt(parameters[0]);
            var last = Global.GetCommandHistory().FirstOrDefault(h =>
            {
                var parts = Global.SplitCommand(h);
                return parts.Length < 1 || Global.Game.GetCommandIDByName(parts[0]) != GetID();
            });
            if (string.IsNullOrEmpty(last))
                return;
            for (int i = 0; i < count; i++)
            {
                Global.ExecuteCommand(last);
            }
        }
    }
}