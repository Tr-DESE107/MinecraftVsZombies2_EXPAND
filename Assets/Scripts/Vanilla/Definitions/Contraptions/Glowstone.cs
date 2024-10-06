using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.glowstone)]
    [EntitySeedDefinition(25, VanillaMod.spaceName, VanillaRechargeNames.longTime)]
    public class Glowstone : VanillaContraption
    {
        public Glowstone(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.PlaySound(SoundID.glowstone);
            var ring = entity.Level.Spawn(VanillaEffectID.shineRing, entity.Position, entity);
            ring.SetParent(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            foreach (var enemy in entity.Level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsEnemy(entity)))
            {
                enemy.Stun(150);
            }
        }
    }
}
