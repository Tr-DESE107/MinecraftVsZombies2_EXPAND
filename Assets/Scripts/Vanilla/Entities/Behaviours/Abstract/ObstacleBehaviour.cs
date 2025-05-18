using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs;
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
            if (bodyResult != null && bodyResult.Amount > 0)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.PlaySound(entity.GetDeathSound(), entity.GetCryPitch());
        }
        protected void KillConflictContraptions(Entity entity)
        {
            var grid = entity.GetGrid();
            var statueTakenLayers = new List<NamespaceID>();
            entity.GetTakingGridLayersNonAlloc(grid, statueTakenLayers);
            var entityTakenLayers = new List<NamespaceID>();
            foreach (var contraption in entity.Level.FindEntities(e => e.Type == EntityTypes.PLANT && e.GetGrid() == grid))
            {
                entityTakenLayers.Clear();
                contraption.GetTakingGridLayersNonAlloc(grid, entityTakenLayers);
                if (!entityTakenLayers.Any(l => statueTakenLayers.Contains(l)))
                    continue;
                contraption.Die();
            }
        }
    }
}
