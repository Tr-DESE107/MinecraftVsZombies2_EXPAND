using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.fireBreath)]
    public class FireBreath : EffectBehaviour
    {
        #region 公有方法
        public FireBreath(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Multiply, PROP_LIGHT_RANGE_MULTIPLIER));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT | EntityCollisionHelper.MASK_ENEMY | EntityCollisionHelper.MASK_OBSTACLE | EntityCollisionHelper.MASK_BOSS;
            entity.Level.AddLoopSoundEntity(VanillaSoundID.fireBreath, entity.ID);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            bool existing = parent != null && parent.Exists();
            if (existing)
            {
                entity.Timeout = MAX_TIMEOUT;
                var cooldown = GetDamageCooldown(entity);
                cooldown--;
                if (cooldown <= 0)
                {
                    foreach (var collision in entity.GetCurrentCollisions())
                    {
                        collision.OtherCollider.TakeDamage(entity.GetDamage(), new DamageEffectList(VanillaDamageEffects.FIRE), entity);
                    }
                    cooldown = DAMAGE_COOLDOWN;
                }
                SetDamageCooldown(entity, cooldown);
            }
            entity.SetAnimationBool("Burning", existing);
            var lightPercentage = Mathf.Max(0, (entity.Timeout / (float)MAX_TIMEOUT) * 3 - 2);
            entity.SetProperty(PROP_LIGHT_RANGE_MULTIPLIER, Vector3.one * lightPercentage);
        }
        public static int GetDamageCooldown(Entity entity) => entity.GetProperty<int>("DamageCooldown");
        public static void SetDamageCooldown(Entity entity, int value) => entity.SetProperty("DamageCooldown", value);
        #endregion
        public const int DAMAGE_COOLDOWN = 15;
        public const int MAX_TIMEOUT = 30;
        public const string PROP_LIGHT_RANGE_MULTIPLIER = "LightRangeMultiplier";
    }
}