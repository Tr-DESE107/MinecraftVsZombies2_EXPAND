using System.Linq;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.obstacleDestroyContraption)]
    public class ObstacleDestroyContraptionBehaviour : EntityBehaviourDefinition
    {
        public ObstacleDestroyContraptionBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.UpdateTakenGrids();

            var grids = entity.GetGridsToTake();
            foreach (var grid in grids)
            {
                var statueTakenLayers = entity.GetTakingGridLayers(grid);
                foreach (var contraption in entity.Level.FindEntities(e => e.Type == EntityTypes.PLANT && e.GetGridsToTake().Contains(grid)))
                {
                    var contraptionTakenLayers = contraption.GetTakingGridLayers(grid);
                    if (contraptionTakenLayers.Intersect(statueTakenLayers).Count() <= 0)
                        continue;
                    contraption.Die();
                }
            }
        }
    }
}
