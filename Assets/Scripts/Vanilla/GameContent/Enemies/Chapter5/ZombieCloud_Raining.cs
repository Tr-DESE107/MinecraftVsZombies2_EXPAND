using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.zombieCloudRaining)]
    public class ZombieCloud_Raining : AIEntityBehaviour
    {
        public ZombieCloud_Raining(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (entity.IsSecondsInterval(0.25f))
            {
                var x = entity.RNG.NextFloat() * 32f - 16f;
                var pos = entity.Position + new Vector3(x, 0, 0);
                entity.Spawn(VanillaEffectID.zombieCloudRaindrop, pos);
            }
        }
    }
}
