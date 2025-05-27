using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.poisonJavelin)]
    public class PoisonJavelin : ProjectileBehaviour
    {
        public PoisonJavelin(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            var cooldown = GetGasCooldown(projectile);
            cooldown--;
            if (cooldown <= 0)
            {
                var gas = projectile.SpawnWithParams(VanillaEffectID.weaknessGas, projectile.Position);
                cooldown = MAX_COOLDOWN;
            }
            SetGasCooldown(projectile, cooldown);
        }
        public static int GetGasCooldown(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_GAS_COOLDOWN);
        public static void SetGasCooldown(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_GAS_COOLDOWN, value);
        private static readonly NamespaceID ID = VanillaProjectileID.poisonJavelin;
        public const int MAX_COOLDOWN = 3;
        public static readonly VanillaEntityPropertyMeta<int> PROP_GAS_COOLDOWN = new VanillaEntityPropertyMeta<int>("GasCooldown");
    }
}
