using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.izObserver)]
    public class IZObserver : EffectBehaviour
    {

        #region 公有方法
        public IZObserver(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (!IsPass(entity))
            {
                var lane = entity.GetLane();
                var rightX = entity.GetBounds().max.x;
                if (entity.Level.EntityExists(e => e.IsHostileEntity() && e.GetLane() == lane && e.Position.x < rightX))
                {
                    SetPass(entity, true);
                    Global.AddSaveStat(VanillaStats.CATEGORY_IZ_OBSERVER_TRIGGER, entity.Level.StageID, 1);
                    entity.Produce(VanillaPickupID.emerald);
                    entity.PlaySound(VanillaSoundID.gulp);
                }
            }
            entity.SetAnimationBool("Pass", IsPass(entity));
        }
        public static bool IsPass(Entity entity) => entity.GetBehaviourField<bool>(PROP_PASS);
        public static void SetPass(Entity entity, bool value) => entity.SetBehaviourField(PROP_PASS, value);
        private static readonly VanillaEntityPropertyMeta PROP_PASS = new VanillaEntityPropertyMeta("Pass");
    }
}