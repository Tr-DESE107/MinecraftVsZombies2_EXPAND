using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Damages;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class ObstacleBehaviour : EntityBehaviourDefinition
    {
        protected ObstacleBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null && bodyResult.Amount > 0 && !bodyResult.HasEffect(VanillaDamageEffects.NO_DAMAGE_BLINK))
            {
                var entity = bodyResult.Entity;
                entity.DamageBlink();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.PlayCrySound(entity.GetDeathSound());
        }
        protected void KillConflictContraptions(Entity entity)
        {
            var grids = entity.GetGridsToTake();
            foreach (var grid in grids)
            {
                statueTakenLayersBuffer.Clear();
                entity.GetTakingGridLayersNonAlloc(grid, statueTakenLayersBuffer);
                foreach (var contraption in entity.Level.FindEntities(e => e.Type == EntityTypes.PLANT && e.GetGridsToTake().Contains(grid)))
                {
                    entityTakenLayersBuffer.Clear();
                    contraption.GetTakingGridLayersNonAlloc(grid, entityTakenLayersBuffer);
                    if (!entityTakenLayersBuffer.Any(l => statueTakenLayersBuffer.Contains(l)))
                        continue;
                    contraption.Die();
                }
            }
        }
        private List<NamespaceID> statueTakenLayersBuffer = new List<NamespaceID>();
        private List<NamespaceID> entityTakenLayersBuffer = new List<NamespaceID>();
    }
}
