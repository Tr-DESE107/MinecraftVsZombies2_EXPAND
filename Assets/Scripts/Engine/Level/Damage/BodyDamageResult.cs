using UnityEngine;

namespace PVZEngine.Damages
{
    public class BodyDamageResult : DamageResult
    {
        public override Vector3 GetPosition()
        {
            return Entity.Position;
        }
    }
}
