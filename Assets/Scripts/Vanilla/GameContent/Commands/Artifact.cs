using System;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.IZombie;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.artifact)]
    public class Artifact : CommandDefinition
    {
        public Artifact(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            var mode = parameters[0];
            var slot = ParseHelper.ParseInt(parameters[1]);
            if (slot < 0 || slot >= level.GetSeedSlotCount())
            {
                var msg = Global.Game.GetTextParticular(VanillaStrings.COMMAND_BLUEPRINT_SLOT_OUT_OF_RANGE, VanillaStrings.CONTEXT_COMMAND_OUTPUT, slot.ToString());
                throw new ArgumentException(msg);
            }
            if (mode == "set")
            {
                var idParam = parameters[2];
                var id = NamespaceID.Parse(idParam, VanillaMod.spaceName);
                var definition = game.GetArtifactDefinition(id);
                level.ReplaceArtifact(slot, definition);
            }
            else if (mode == "remove")
            {
                level.SetArtifact(slot, null);
            }
        }
    }
}