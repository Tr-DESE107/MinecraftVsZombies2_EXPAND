using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.shikaisenZombie)]
    public class ShikaisenZombie : MeleeEnemy
    {
        public ShikaisenZombie(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStaff(entity, true);
        }
        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);
            if (HasStaff(enemy) && enemy.Health <= enemy.GetMaxHealth() * 0.5f && enemy.IsOnGround)
            {
                SpawnStaff(enemy);
                SetStaff(enemy, false);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
            entity.SetAnimationBool("HasStaff", HasStaff(entity));
        }
        public static Entity SpawnStaff(Entity entity)
        {
            var pos = entity.Position + entity.GetFacingDirection() * 30;
            var staff = entity.Spawn(VanillaEnemyID.shikaisenStaff, pos);
            staff.PlaySound(VanillaSoundID.wood);
            return staff;
        }
        public static bool HasStaff(Entity enemy) => enemy.GetBehaviourField<bool>(PROP_HAS_STAFF);
        public static void SetStaff(Entity enemy, bool value) => enemy.SetBehaviourField(PROP_HAS_STAFF, value);
        public static readonly VanillaEntityPropertyMeta PROP_HAS_STAFF = new VanillaEntityPropertyMeta("HasStaff");
    }
}
