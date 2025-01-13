using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.poisonJavelin)]
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
                var gas = projectile.Level.Spawn(VanillaEffectID.weaknessGas, projectile.Position, projectile);
                gas.SetFaction(projectile.GetFaction());
                cooldown = MAX_COOLDOWN;
            }
            SetGasCooldown(projectile, cooldown);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Remove();
        }
        public static int GetGasCooldown(Entity entity) => entity.GetBehaviourField<int>(ID, "GasCooldown");
        public static void SetGasCooldown(Entity entity, int value) => entity.SetBehaviourField(ID, "GasCooldown", value);
        private static readonly NamespaceID ID = VanillaProjectileID.poisonJavelin;
        public const int MAX_COOLDOWN = 3;
    }
}
