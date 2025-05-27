using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Shells;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.motherTerror)]
    public class MotherTerror : MeleeEnemy
    {
        public MotherTerror(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskFriendly |= EntityCollisionHelper.MASK_ENEMY;
            SetRestoreEggTimer(entity, new FrameTimer(600));
        }
        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);
            var timer = GetRestoreEggTimer(enemy);
            if (!HasEggs(enemy))
            {
                timer.Run();
                if (timer.Expired)
                {
                    enemy.RemoveBuffs<MotherTerrorLaidBuff>();
                    timer.Reset();
                }
            }
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (!collision.Collider.IsForMain() || !collision.OtherCollider.IsForMain())
                return;
            var spider = collision.Entity;
            if (!HasEggs(spider))
                return;
            var other = collision.Other;
            if (state != EntityCollisionHelper.STATE_EXIT)
            {
                if (CanLayEgg(spider, other))
                {
                    LayEgg(spider, other);
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            if (!HasEggs(entity))
                return;
            var level = entity.Level;
            int count = 1 + level.GetEnemyAILevel();
            for (int i = 0; i < count; i++)
            {
                entity.SpawnWithParams(VanillaEnemyID.parasiteTerror, entity.GetCenter());
            }
            entity.PlaySound(VanillaSoundID.bloody);
            entity.EmitBlood();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            // 设置血量状态。
            entity.SetAnimationInt("EggState", GetEggState(entity));
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public static bool HasEggs(Entity spider)
        {
            return !spider.HasBuff<MotherTerrorLaidBuff>();
        }
        public static bool CanLayEgg(Entity self, Entity target)
        {
            if (target == self)
                return false;
            if (target.IsDead)
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (!self.IsFriendly(target))
                return false;
            if (target.IsEntityOf(VanillaEnemyID.motherTerror) || target.IsEntityOf(VanillaEnemyID.parasiteTerror))
                return false;
            if (target.GetShellID() != VanillaShellID.flesh)
                return false;
            if (target.HasBuff<TerrorParasitizedBuff>())
                return false;
            return true;
        }
        public static void LayEgg(Entity spider, Entity target)
        {
            target.AddBuff<TerrorParasitizedBuff>();
            target.PlaySound(VanillaSoundID.parasitize);
            spider.AddBuff<MotherTerrorLaidBuff>();
        }
        public static void SetRestoreEggTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_RESTORE_EGG_TIMER, timer);
        }
        public static FrameTimer GetRestoreEggTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_RESTORE_EGG_TIMER);
        }
        private static int GetEggState(Entity entity)
        {
            return HasEggs(entity) ? 1 : -1;
        }
        public static readonly NamespaceID ID = VanillaEnemyID.motherTerror;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_RESTORE_EGG_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("RestoreEggTimer");
    }
}
