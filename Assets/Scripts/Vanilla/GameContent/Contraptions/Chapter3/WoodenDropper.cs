using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.woodenDropper)]
    public class WoodenDropper : DispenserFamily
    {
        public WoodenDropper(string nsp, string name) : base(nsp, name)
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
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var rng = entity.RNG;
            for (int i = 0; i < 30; i++)
            {
                var xspeed = entity.GetFacingX() * rng.Next(10f, 18f);
                var yspeed = rng.Next(30f);
                var zspeed = rng.Next(-1.5f, 1.5f);
                var param = entity.GetShootParams();
                param.velocity = new Vector3(xspeed, yspeed, zspeed);
                entity.ShootProjectile(param);
            }
            entity.PlaySound(VanillaSoundID.launch);
        }
    }
}
