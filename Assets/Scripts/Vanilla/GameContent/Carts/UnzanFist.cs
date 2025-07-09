using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Carts
{
    [EntityBehaviourDefinition(VanillaCartNames.unzanFist)]
    public class UnzanFist : CartBehaviour
    {
        public UnzanFist(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostCrush(Entity entity, Entity other)
        {
            base.PostCrush(entity, other);
            entity.PlaySound(VanillaSoundID.punch);
            other.Velocity += entity.GetFacingDirection() * 40f + Vector3.up * 20f;
        }
    }
}