using PVZEngine.Armors;
using UnityEngine;

namespace PVZEngine.Damages
{
    public class ArmorDamageResult : DamageResult
    {
        public Armor Armor { get; set; }

        public override Vector3 GetPosition()
        {
            return Entity.Position;
        }
    }
}
