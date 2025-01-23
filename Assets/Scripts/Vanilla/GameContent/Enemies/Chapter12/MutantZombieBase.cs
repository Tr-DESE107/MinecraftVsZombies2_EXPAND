using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
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
            entity.SetAnimationInt("Weapon", GetWeapon(entity));
            entity.SetAnimationInt("HealthState", entity.GetHealthState(3));
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            stateMachine.StartState(entity, STATE_DEATH);
        }
        public static bool HasImp(Entity entity) => entity.GetBehaviourField<bool>(ID, FIELD_HAS_IMP);
        public static void SetHasImp(Entity entity, bool value) => entity.SetBehaviourField(ID, FIELD_HAS_IMP, value);
        public static int GetWeapon(Entity entity) => entity.GetBehaviourField<int>(ID, FIELD_WEAPON);
        public static void SetWeapon(Entity entity, int value) => entity.SetBehaviourField(ID, FIELD_WEAPON, value);

        public const string FIELD_HAS_IMP = "HasImp";
        public const string FIELD_WEAPON = "Weapon";

        private static readonly NamespaceID ID = new NamespaceID("mvz2", "mutant_zombie_base");
        private static EntityStateMachine stateMachine = new MutantZombieStateMachine();
        private static Detector detector = new MutantZombieDetector();
    }

}