using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.ghost)]
    public class Ghost : MeleeEnemy
    {
        public Ghost(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
        }
        public override void PostDeath(Entity entity, DamageInput info)
        {
            base.PostDeath(entity, info);
            if (!IsEverIlluminated(entity) && !info.Effects.HasEffect(VanillaDamageEffects.WHACK))
            {
                Global.Game.Unlock(VanillaUnlockID.ghostBuster);
            }
        }
        public static void SetEverIlluminated(Entity entity, bool value)
        {
            entity.SetBehaviourProperty(ID, PROP_EVER_ILLUMINATED, value);
        }
        public static bool IsEverIlluminated(Entity entity)
        {
            return entity.GetBehaviourProperty<bool>(ID, PROP_EVER_ILLUMINATED);
        }
        public const string PROP_EVER_ILLUMINATED = "EverIlluminated";
        public static readonly NamespaceID ID = VanillaEnemyID.ghost;
    }
}
