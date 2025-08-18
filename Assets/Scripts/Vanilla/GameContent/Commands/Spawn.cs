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

            var id = NamespaceID.Parse(parameters[1], VanillaMod.spaceName);

            float x = level.GetEntityColumnX(Mathf.FloorToInt(level.GetMaxColumnCount() * 0.5f));
            float z = level.GetEntityLaneZ(Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f));
            float y = level.GetGroundY(x, z);
            if (parameters[0] == "pos")
            {
                if (parameters.Length >= 3)
                {
                    x = ParseHelper.ParseFloat(parameters[2]);
                }
                if (parameters.Length >= 4)
                {
                    y = ParseHelper.ParseFloat(parameters[3]);
                }
                if (parameters.Length >= 5)
                {
                    z = ParseHelper.ParseFloat(parameters[4]);
                }
            }
            else if (parameters[0] == "grid")
            {
                var column = Mathf.FloorToInt(level.GetMaxColumnCount() * 0.5f);
                var lane = Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f);

                if (parameters.Length >= 3)
                {
                    column = ParseHelper.ParseInt(parameters[2]);
                }
                if (parameters.Length >= 4)
                {
                    lane = ParseHelper.ParseInt(parameters[3]);
                }

                x = level.GetEntityColumnX(column);
                z = level.GetEntityLaneZ(lane);
                y = level.GetGroundY(x, z);
            }

            level.Spawn(id, new Vector3(x, y, z), null);
        }
    }
}