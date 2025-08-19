using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Command;
using MVZ2Logic.IZombie;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.spawnBlueprint)]
    public class SpawnBlueprint : CommandDefinition
    {
        public SpawnBlueprint(string nsp, string name) : base(nsp, name)
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
                    x = CommandUtility.ParseOptionalFloat(parameters[2], x);
                }
                if (parameters.Length >= 4)
                {
                    y = CommandUtility.ParseOptionalFloat(parameters[3], y);
                }
                if (parameters.Length >= 5)
                {
                    z = CommandUtility.ParseOptionalFloat(parameters[4], z);
                }
            }
            else if (parameters[0] == "grid")
            {
                var column = Mathf.Floor(level.GetMaxColumnCount() * 0.5f);
                var lane = Mathf.Floor(level.GetMaxLaneCount() * 0.5f);

                if (parameters.Length >= 3)
                {
                    column = CommandUtility.ParseOptionalFloat(parameters[2], column);
                }
                if (parameters.Length >= 4)
                {
                    lane = CommandUtility.ParseOptionalFloat(parameters[3], lane);
                }

                x = level.GetEntityColumnXFloat(column);
                z = level.GetEntityLaneZFloat(lane);
                y = level.GetGroundY(x, z);
            }

            var blueprintID = VanillaBlueprintID.FromEntity(id);
            var spawnParams = new SpawnParams();
            spawnParams.SetProperty(VanillaPickupProps.CONTENT_ID, blueprintID);
            level.Spawn(VanillaPickupID.blueprintPickup, new Vector3(x, y, z), null, spawnParams);
        }
    }
}