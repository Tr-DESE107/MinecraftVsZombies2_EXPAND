using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.spawn)]
    public class Spawn : CommandDefinition
    {
        public Spawn(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            var id = NamespaceID.Parse(parameters[0], VanillaMod.spaceName);

            var x = level.GetEntityColumnX(Mathf.FloorToInt(level.GetMaxColumnCount() * 0.5f));
            var z = level.GetEntityLaneZ(Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f));
            var y = level.GetGroundY(x, z);

            if (parameters.Length >= 2)
            {
                x = ParseHelper.ParseFloat(parameters[1]);
            }
            if (parameters.Length >= 3)
            {
                y = ParseHelper.ParseFloat(parameters[2]);
            }
            if (parameters.Length >= 4)
            {
                z = ParseHelper.ParseFloat(parameters[3]);
            }

            level.Spawn(id, new Vector3(x, y, z), null);
        }
    }
}