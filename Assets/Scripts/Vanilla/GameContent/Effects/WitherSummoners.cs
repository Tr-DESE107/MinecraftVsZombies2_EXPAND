using MVZ2.GameContent.Bosses;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.witherSummoners)]
    public class WitherSummoners : EffectBehaviour
    {

        #region 公有方法
        public WitherSummoners(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.PlaySound(VanillaSoundID.reverseVampire);
            entity.PlaySound(VanillaSoundID.odd);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.Timeout <= 0)
            {
                entity.Spawn(VanillaBossID.wither, entity.Position);
            }
        }
        #endregion
    }
}