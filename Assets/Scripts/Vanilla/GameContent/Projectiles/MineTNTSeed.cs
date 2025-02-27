using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.mineTNTSeed)]
    public class MineTNTSeed : ProjectileBehaviour
    {
        public MineTNTSeed(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile = 0;
            entity.CollisionMaskFriendly = 0;
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var column = entity.GetColumn();
            var lane = entity.GetLane();
            var grid = entity.Level.GetGrid(column, lane);
            if (grid != null && grid.CanPlaceOrStackEntity(VanillaContraptionID.mineTNT))
            {
                var x = entity.Level.GetEntityColumnX(column);
                var z = entity.Level.GetEntityLaneZ(lane);
                var y = entity.Level.GetGroundY(x, z);
                var mine = entity.Level.Spawn(VanillaContraptionID.mineTNT, new Vector3(x, y, z), entity);
                var riseTimer = MineTNT.GetRiseTimer(mine);
                riseTimer.Frame = 31;
            }
            entity.Remove();
        }
    }
}
