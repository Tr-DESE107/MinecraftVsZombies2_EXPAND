using PVZEngine.Entities;
using UnityEngine;

namespace PVZEngine.Damages
{
    public class BodyDamageResult : DamageResult
    {
        public BodyDamageResult(DamageInput input) : base(input)
        {
        }
        public override Vector3 GetPosition()
        {
            return Entity.Position;
        }
    }
}
