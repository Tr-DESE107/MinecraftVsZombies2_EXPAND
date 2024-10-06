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
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            foreach (var enemy in entity.Level.GetEntities(EntityTypes.ENEMY))
            {
                enemy.Stun();
            }
        }
    }
}
