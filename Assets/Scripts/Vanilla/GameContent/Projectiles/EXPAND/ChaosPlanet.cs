#nullable enable

using MVZ2.Vanilla.Projectiles;
using MVZ2.Vanilla.Properties;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Projectiles
{
    /// <summary>
    /// 混沌异界星：三体运动投射物行为。
    /// 组内星体每帧按牛顿引力相互加速（欧拉积分），配合质心束缚与限速，
    /// 呈现不可预测的混沌三体轨道；运动轨迹对初值敏感，每次发射都会随机扰动初速。
    /// </summary>
    [AutoEntityBehaviourDefinition(VanillaEntityBehaviourNames.chaosHellPlanet)]
    public class ChaosPlanet : EntityBehaviourDefinition
    {
        public ChaosPlanet(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SCALE));
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, PROP_SCALE));
            AddModifier(new BooleanModifier(VanillaProjectileProps.NO_DESTROY_OUTSIDE_LAWN, PROP_NO_DESTROY_OUTSIDE_LAWN));
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);

            // 缩放反馈（与地狱行星一致的视觉衰减）
            var scaleMultiplier = GetScaleMultiplier(entity);
            scaleMultiplier = scaleMultiplier * 0.9f + Vector3.one * 0.1f;
            SetScaleMultiplier(entity, scaleMultiplier);

            // 三体引力积分：对所有存活组员求引力和
            var mates = GetMates(entity);
            Vector3 accel = Vector3.zero;
            Vector3 groupCenter = entity.GetCenter();
            int aliveCount = 1;
            if (mates != null)
            {
                foreach (var mateId in mates)
                {
                    var mate = mateId?.GetEntity(entity.Level);
                    if (mate == null || !mate.ExistsAndAlive())
                        continue;
                    var toMate = mate.GetCenter() - entity.GetCenter();
                    var dist = Mathf.Max(toMate.magnitude, MIN_ORBIT_DISTANCE);
                    accel += toMate.normalized * (GRAVITY / (dist * dist));
                    groupCenter += mate.GetCenter();
                    aliveCount++;
                }
            }
            groupCenter /= aliveCount;

            // 束缚：离组质心过远时向心拉回，防止星体飞散
            var toCenter = groupCenter - entity.GetCenter();
            var centerDist = toCenter.magnitude;
            if (centerDist > BOUND_RADIUS)
            {
                accel += toCenter.normalized * (BOUND_PULL * (centerDist - BOUND_RADIUS) / BOUND_RADIUS);
            }

            // 欧拉积分推进速度并限速，防止数值爆炸
            entity.Velocity += accel;
            var speed = entity.Velocity.magnitude;
            if (speed > MAX_SPEED)
                entity.Velocity = entity.Velocity.normalized * MAX_SPEED;

            // 组内仍有存活星体时不出场地销毁（星系必须整体存在）
            SetNoDestroyOutsideLawn(entity, aliveCount > 1);
        }

        // 组员引用（发射时由 OnEvoke 互写）
        public static EntityID[]? GetMates(Entity entity) => entity.GetBehaviourField<EntityID[]>(PROP_MATES);
        public static void SetMates(Entity entity, EntityID[] value) => entity.SetBehaviourField(PROP_MATES, value);

        // 缩放反馈
        public static Vector3 GetScaleMultiplier(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_SCALE);
        public static void SetScaleMultiplier(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_SCALE, value);

        public static bool NoDestroyOutsideLawn(Entity entity) => entity.GetBehaviourField<bool>(PROP_NO_DESTROY_OUTSIDE_LAWN);
        public static void SetNoDestroyOutsideLawn(Entity entity, bool value) => entity.SetBehaviourField(PROP_NO_DESTROY_OUTSIDE_LAWN, value);

        public static readonly VanillaEntityPropertyMeta<EntityID[]> PROP_MATES = new VanillaEntityPropertyMeta<EntityID[]>("mates");
        public static readonly VanillaEntityPropertyMeta<Vector3> PROP_SCALE = new VanillaEntityPropertyMeta<Vector3>("scale");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_NO_DESTROY_OUTSIDE_LAWN = new VanillaEntityPropertyMeta<bool>("no_destroy_outside_lawn");

        public const float GRAVITY = 6000f;           // 引力常数：加速度 = G / 距离²（每帧）
        public const float MIN_ORBIT_DISTANCE = 40f;  // 最小作用距离（防奇点弹射）
        public const float BOUND_RADIUS = 240f;       // 质心束缚半径：超过后向心拉回
        public const float BOUND_PULL = 0.6f;         // 束缚最大拉力
        public const float MAX_SPEED = 15f;           // 星体速度上限
    }
}
