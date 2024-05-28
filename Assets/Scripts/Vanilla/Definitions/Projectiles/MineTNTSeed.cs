using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(ProjectileNames.mineTNTSeed)]
    public class MineTNTSeed : VanillaProjectile
    {
        public MineTNTSeed(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SIZE, new Vector3(24, 24, 24));
            SetProperty(EntityProperties.GRAVITY, 1f);
            SetProperty(ProjectileProps.NO_HIT_ENTITIES, true);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMask = 0;
        }
        public override void PostContactGround(Entity entity)
        {
            base.PostContactGround(entity);
            var column = entity.GetColumn();
            var lane = entity.GetLane();
            var grid = entity.Level.GetGrid(column, lane);
            if (grid.CanPlace(ContraptionID.mineTNT))
            {
                var x = entity.Level.GetEntityColumnX(column);
                var z = entity.Level.GetEntityLaneZ(lane);
                var y = entity.Level.GetGroundY(x, z);
                var mine = entity.Level.Spawn(ContraptionID.mineTNT, new Vector3(x, y, z), entity);
                var riseTimer = MineTNT.GetRiseTimer(mine);
                riseTimer.Frame = 31;
            }
            entity.Remove();
        }
    }
}
