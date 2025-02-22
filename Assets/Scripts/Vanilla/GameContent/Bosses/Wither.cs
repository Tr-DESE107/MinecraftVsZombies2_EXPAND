using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    [EntityBehaviourDefinition(VanillaBossNames.wither)]
    public partial class Wither : BossBehaviour
    {
        public Wither(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            stateMachine.Init(boss);
            stateMachine.StartState(boss, STATE_IDLE);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            stateMachine.StartState(boss, STATE_DEATH);
        }
        public override void PreTakeDamage(DamageInput damageInfo)
        {
            base.PreTakeDamage(damageInfo);
            if (damageInfo.Amount > 600)
            {
                damageInfo.SetAmount(600);
            }
        }
        #endregion 事件

        #region 常量
        private static readonly VanillaEntityPropertyMeta PROP_PHASE = new VanillaEntityPropertyMeta("Phase");

        private const int STATE_IDLE = VanillaEntityStates.WITHER_IDLE;
        private const int STATE_APPEAR = VanillaEntityStates.WITHER_APPEAR;
        private const int STATE_CHARGE = VanillaEntityStates.WITHER_CHARGE;
        private const int STATE_EAT = VanillaEntityStates.WITHER_EAT;
        private const int STATE_SUMMON = VanillaEntityStates.WITHER_SUMMON;
        private const int STATE_DEATH = VanillaEntityStates.WITHER_DEATH;
        #endregion 常量

        private static WitherStateMachine stateMachine = new WitherStateMachine();
    }
}
