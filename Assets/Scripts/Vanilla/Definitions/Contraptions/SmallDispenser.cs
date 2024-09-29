using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.smallDispenser)]
    [EntitySeedDefinition(0, VanillaMod.spaceName, RechargeNames.shortTime)]
    public class SmallDispenser : DispenserFamily
    {
        public SmallDispenser(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaEntityProps.RANGE, 255f);
            SetProperty(VanillaEntityProps.SHOT_OFFSET, new Vector3(25, 15, 0));
            SetProperty(VanillaEntityProps.SHOOT_SOUND, SoundID.bow);
            SetProperty(VanillaEntityProps.PROJECTILE_ID, ProjectileID.snowball);
            SetProperty(EntityProperties.SIZE, new Vector3(32, 32, 32));
            SetProperty(ContraptionProps.FRAGMENT, ContraptionID.furnace);
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
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

        public override void Evoke(Entity entity)
        {
            base.Evoke(entity);
        }
    }
}
