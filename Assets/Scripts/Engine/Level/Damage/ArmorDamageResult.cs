using PVZEngine.Armors;
using UnityEngine;

namespace PVZEngine.Damages
{
    public class ArmorDamageResult : DamageResult
    {
        public ArmorDamageResult(DamageInput input) : base(input)
        {
        }

        public Armor Armor { get; set; }

        public override Vector3 GetPosition()
        {
            return Entity.Position;
        }
    }
}
