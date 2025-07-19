using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using PVZEngine;
using MVZ2.Vanilla.Level;
using MVZ2.GameContent.Damages;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.hellfire)]
    public class Hellfire : ContraptionBehaviour
    {
        // 存储“上次触发时的血量”的字段名
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // 当前实体的ID（用于字段访问）
        private static readonly NamespaceID ID = VanillaContraptionID.hellfire;

        public Hellfire(string nsp, string name) : base(nsp, name)
        {
            detector = new HellfireIgniteDetector(32)
            {
                factionTarget = FactionTarget.Friendly,
                mask = EntityCollisionHelper.MASK_PROJECTILE,
            };
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            SetLastTriggerHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            UpdateIgnite(entity);

            CheckHealthLossTrigger(entity);

            entity.SetAnimationBool("Evoked", IsCursed(entity));
        }

        private void CheckHealthLossTrigger(Entity entity)
        {
            float lastHP = GetLastTriggerHealth(entity);
            float currHP = entity.Health;

            // 每掉落一定血量（例如 100 点）触发一次事件
            int triggerCount = (int)((lastHP - currHP) / 100f);
            triggerCount = Mathf.Min(triggerCount, 2);
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                {
                    FireAOE(entity, 50f, entity.GetFaction());
                }

                // 更新记录的血量
                SetLastTriggerHealth(entity, currHP);
            }
        }

        public static void FireAOE(Entity entity, float damage, int faction)
        {

            var range = 40;
            entity.Explode(
                entity.GetCenter(),
                range,
                faction,
                damage,
                new DamageEffectList(VanillaDamageEffects.FIRE, VanillaDamageEffects.MUTE)
            );


        }

        public override bool CanEvoke(Entity entity)
        {
            if (IsCursed(entity))
                return false;
            var meteor = GetMeteor(entity);
            if (meteor != null && meteor.Exists(entity.Level))
                return false;
            return base.CanEvoke(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var pos = entity.Position + new Vector3(0, 1280, 0);
            var meteor = entity.SpawnWithParams(VanillaEffectID.cursedMeteor, pos);
            meteor.SetParent(entity);
            SetMeteor(entity, new EntityID(meteor));
            meteor.PlaySound(VanillaSoundID.bombFalling);
        }
        private void UpdateIgnite(Entity hellfire)
        {
            bool cursed = IsCursed(hellfire);
            igniteBuffer.Clear();
            detector.DetectEntities(hellfire, igniteBuffer);
            foreach (Entity target in igniteBuffer)
            {
                var behaviour = target.Definition?.GetBehaviour<IHellfireIgniteBehaviour>();
                if (behaviour == null)
                    return;
                behaviour.Ignite(target, hellfire, cursed);
            }
        }
        public static void Curse(Entity entity)
        {
            SetCursed(entity, true);
            entity.AddBuff<HellfireCursedBuff>();
        }
        public static void SetCursed(Entity entity, bool value) => entity.SetProperty(PROP_CURSED, value);
        public static bool IsCursed(Entity entity) => entity.GetProperty<bool>(PROP_CURSED);
        public static void SetMeteor(Entity entity, EntityID value) => entity.SetProperty(PROP_METEOR, value);
        public static EntityID GetMeteor(Entity entity) => entity.GetProperty<EntityID>(PROP_METEOR);
        public static readonly VanillaBuffPropertyMeta<bool> PROP_CURSED = new VanillaBuffPropertyMeta<bool>("cursed");
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_METEOR = new VanillaBuffPropertyMeta<EntityID>("meteor");
        private Detector detector;
        private List<Entity> igniteBuffer = new List<Entity>();
    }
    public interface IHellfireIgniteBehaviour
    {
        void Ignite(Entity entity, Entity hellfire, bool cursed);
    }

}
