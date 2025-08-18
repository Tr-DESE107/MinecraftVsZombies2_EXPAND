using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.spawngrid)]
    public class SpawnGrid : CommandDefinition
    {
        public SpawnGrid(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var game = Global.Game;
            var level = Global.Game.GetLevel();

            var id = NamespaceID.Parse(parameters[0], VanillaMod.spaceName);

            var column = Mathf.FloorToInt(level.GetMaxColumnCount() * 0.5f);
            var lane = Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f);

            if (parameters.Length >= 2)
            {
                column = ParseHelper.ParseInt(parameters[1]);
            }
            if (parameters.Length >= 3)
            {
                lane = ParseHelper.ParseInt(parameters[2]);
            }

            var x = level.GetEntityColumnX(column);
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundY(x, z);
            level.Spawn(id, new Vector3(x, y, z), null);
        }
    }
}