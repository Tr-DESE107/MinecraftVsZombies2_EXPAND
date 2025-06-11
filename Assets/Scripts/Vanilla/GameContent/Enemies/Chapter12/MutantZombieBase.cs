using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Enemies
{
    public abstract partial class MutantZombieBase : EnemyBehaviour
    {
        protected MutantZombieBase(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            stateMachine.Init(entity);
            stateMachine.StartState(entity, STATE_IDLE);

            SetHasImp(entity, true);
            SetWeapon(entity, entity.RNG.Next(3));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            stateMachine.UpdateAI(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);
            entity.SetAnimationBool("HasImp", HasImp(entity));
            entity.SetModelDamagePercent();
            entity.SetModelProperty("Weapon", entity.State == STATE_DEATH ? -1 : GetWeapon(entity));
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            stateMachine.StartState(entity, STATE_DEATH);
        }
        public static bool HasImp(Entity entity) => entity.GetBehaviourField<bool>(FIELD_HAS_IMP);
        public static void SetHasImp(Entity entity, bool value) => entity.SetBehaviourField(FIELD_HAS_IMP, value);
        public static int GetWeapon(Entity entity) => entity.GetBehaviourField<int>(FIELD_WEAPON);
        public static void SetWeapon(Entity entity, int value) => entity.SetBehaviourField(FIELD_WEAPON, value);

        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<bool> FIELD_HAS_IMP = new VanillaEntityPropertyMeta<bool>("HasImp");
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<int> FIELD_WEAPON = new VanillaEntityPropertyMeta<int>("Weapon");

        private const string PROP_REGION = "mutant_zombie_base";
        private static EntityStateMachine stateMachine = new MutantZombieStateMachine();
        private static Detector attackDetector = new MutantZombieDetector(0);
        private static Detector hammerDetector = new MutantZombieDetector(40);
    }

}