using System.Linq;
using MVZ2.GameContent.Buffs.Carts;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.cartCommon)]
    public class CartCommonBehaviour : EntityBehaviourDefinition
    {
        public CartCommonBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetCanUpdateBeforeGameStart(true);
            entity.AddBuff<CartFadeInBuff>();
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            StateUpdate(entity);
            TriggerChargeUpdate(entity);
            TurnToMoneyUpdate(entity);
        }
        private void StateUpdate(Entity entity)
        {
            var velocity = entity.Velocity;
            switch (entity.State)
            {
                default:
                    if (entity.Position.x < VanillaLevelExt.CART_TARGET_X)
                    {
                        velocity.x = 10;
                    }
                    else
                    {
                        var pos = entity.Position;
                        pos.x = VanillaLevelExt.CART_TARGET_X;
                        entity.Position = pos;
                        velocity.x = 0;
                    }

                    bool triggered = entity.Level.GetEntities(EntityTypes.ENEMY)
                        .Any(e => !e.IsDead && !e.IsHarmless() && entity.IsHostile(e) && e.GetLane() == entity.GetLane() && e.Position.x <= entity.Position.x + TRIGGER_DISTANCE);
                    if (triggered)
                    {
                        entity.TriggerCart();
                    }
                    break;
                case VanillaEntityStates.CART_TRIGGERED:
                    velocity.x = 10;
                    // 获取所有接触到的僵尸。
                    foreach (Entity ent in entity.Level.FindEntities(e => entity.CanCartCrush(e)))
                    {
                        // 碰到小车的僵尸受到伤害。
                        ent.TakeDamage(58115310, new DamageEffectList(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY, VanillaDamageEffects.MUTE), entity);
                    }
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= VanillaLevelExt.GetBorderX(true))
                    {
                        entity.Remove();
                    }
                    break;
            }
            entity.Velocity = velocity;
        }
        private void TriggerChargeUpdate(Entity entity)
        {
            if (!IsCartTriggerCharging(entity))
            {
                SetCartTriggerCharge(entity, 0);
            }
            SetCartTriggerCharging(entity, false);
            entity.SetModelProperty("Charge", GetCartTriggerCharge(entity) / (float)MAX_TRIGGER_CHARGE);
        }
        private void TurnToMoneyUpdate(Entity entity)
        {
            FrameTimer timer = entity.GetTurnToMoneyTimer();
            if (timer == null)
                return;
            timer.Run();
            if (timer.Expired)
            {
                var level = entity.Level;
                var difficultyMeta = Global.Game.GetDifficultyMeta(level.Difficulty);
                var money = 50;
                if (difficultyMeta != null)
                {
                    money = difficultyMeta.CartConvertMoney;
                }
                var gemEffects = GemEffect.SpawnGemEffects(level, money, entity.Position, entity, true, 0);
                foreach (var effect in gemEffects)
                {
                    effect.PlaySound(VanillaSoundID.points, 1 + (level.GetMaxLaneCount() - entity.GetLane() - 1) * 0.1f);
                }
                level.ShowMoney();
                entity.Remove();
            }
        }
        public static void ChargeUpTrigger(Entity entity)
        {
            SetCartTriggerCharging(entity, true);
            var charge = GetCartTriggerCharge(entity);
            charge++;
            if (charge >= MAX_TRIGGER_CHARGE)
            {
                charge = 0;
                entity.TriggerCart();
            }
            SetCartTriggerCharge(entity, charge);
        }
        public override void PostDeath(Entity entity, DeathInfo deathInfo)
        {
            base.PostDeath(entity, deathInfo);
            if (!deathInfo.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                var fragment = entity.GetOrCreateFragment();
                Fragment.AddEmitSpeed(fragment, 500);
            }
            entity.Remove();
        }
        public static void SetCartTriggerCharge(Entity entity, int value) => entity.SetBehaviourField<int>(FIELD_TRIGGER_CHARGE, value);
        public static int GetCartTriggerCharge(Entity entity) => entity.GetBehaviourField<int>(FIELD_TRIGGER_CHARGE);
        public static void SetCartTriggerCharging(Entity entity, bool value) => entity.SetBehaviourField<bool>(FIELD_TRIGGER_CHARGING, value);
        public static bool IsCartTriggerCharging(Entity entity) => entity.GetBehaviourField<bool>(FIELD_TRIGGER_CHARGING);

        public static readonly VanillaEntityPropertyMeta<int> FIELD_TRIGGER_CHARGE = new VanillaEntityPropertyMeta<int>("TriggerCharge");
        public static readonly VanillaEntityPropertyMeta<bool> FIELD_TRIGGER_CHARGING = new VanillaEntityPropertyMeta<bool>("TriggerCharging");
        public const float TRIGGER_DISTANCE = 28;
        public const int MAX_TRIGGER_CHARGE = 30;
    }
}
