using MVZ2.GameContent.Pickups;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using UnityEngine;

namespace MVZ2.GameContent.Commands
{
    [CommandDefinition(VanillaCommandNames.clearLevel)]
    public class ClearLevel : CommandDefinition
    {
        public ClearLevel(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Invoke(string[] parameters)
        {
            var level = Global.Game.GetLevel();

            var id = VanillaPickupID.clearPickup;

            float x = level.GetEntityColumnX(Mathf.FloorToInt(level.GetMaxColumnCount() * 0.5f));
            float z = level.GetEntityLaneZ(Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f));
            float y = level.GetGroundY(x, z);

            level.Spawn(id, new Vector3(x, y, z), null);
        }
    }
}