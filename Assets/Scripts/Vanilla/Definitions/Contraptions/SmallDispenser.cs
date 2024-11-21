using MVZ2Logic.Level;
using MVZ2.Vanilla;
using MVZ2Logic.Audios;
using MVZ2Logic.Entities;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.smallDispenser)]
    [EntitySeedDefinition(0, VanillaMod.spaceName, VanillaRechargeNames.shortTime)]
    public class SmallDispenser : DispenserFamily
    {
        public SmallDispenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                return;
            }
        }
        public override Entity Shoot(Entity entity)
        {
            var projectile = base.Shoot(entity);
            projectile.Timeout = Mathf.CeilToInt(entity.GetRange() / entity.GetShotVelocity().magnitude);
            return projectile;
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.ShootProjectile(VanillaProjectileID.largeSnowball, new Vector3(3, 0, 0));
            entity.PlaySound(SoundID.odd);
        }
    }
}
