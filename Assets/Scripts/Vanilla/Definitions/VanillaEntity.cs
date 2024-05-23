using System.Collections.Generic;
using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEntity : EntityDefinition
    {
        protected VanillaEntity() : base()
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.grass);
        }
        protected void UpdateTakenGrids(Entity entity)
        {
            if (entity.GetRelativeY() > leaveGridHeight || entity.Removed)
            {
                entity.ClearTakenGrids();
            }
            else
            {
                foreach (var grid in GetGridsToTake(entity))
                {
                    entity.TakeGrid(grid);
                }
            }
        }
        protected virtual IEnumerable<int> GetGridsToTake(Entity entity)
        {
            yield return entity.Game.GetGridIndex(entity.GetColumn(), entity.GetLane());
        }
        private const float leaveGridHeight = 64;
    }
}
