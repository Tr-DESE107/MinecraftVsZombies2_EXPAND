#nullable enable

using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Projectiles;
using MVZ2Logic.Entities;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using UnityEngine;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Contraptions
{
    /// <summary>
    /// 万象天枢_三头（三头发射器 × 万象天枢融合体）。
    /// 普攻：一轮 4 拍，每拍中路 1 箭 + 两侧路各 1 箭（场内侧箭飞行变道，越界侧补 1 箭），
    ///       即中路 4 箭、场内边路 4 箭、越界边路 8 箭。
    /// 触发：装填导弹盒（可对空），与万象天枢本体一致（继承基类）。
    /// 大招：发射三颗混沌异界星，以牛顿引力相互加速，做不可预测的三体运动。
    /// </summary>
    [AutoEntityBehaviourDefinition(VanillaContraptionNames.EXPANDispenser_Triplenser)]
    public class EXPANDispenser_Triplenser : EXPANDispenser
    {
        public EXPANDispenser_Triplenser(string nsp, string name) : base(nsp, name)
        {
        }

        // 参照基类：大招升级（HFPDUpgradedBuff）后模型进入升级形态并发光
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("Upgraded", IsUpgraded(entity));
            entity.SetModelProperty("GatlinAlt", IsGatlinAlt(entity));
            entity.SetModelProperty("MissileBox", HasMissileBox(entity));
        }

        // 普攻连发：一轮 4 拍（每拍三向齐射）
        public override void OnShootTick(Entity entity)
        {
            SetRepeatCount(entity, REPEAT_VOLLEY);
            var repeatTimer = GetRepeatTimer(entity);
            if (repeatTimer != null)
            {
                repeatTimer.ResetTime(Mathf.FloorToInt(15f / REPEAT_VOLLEY));
                repeatTimer.Frame = 0;
            }
        }

        // 每拍三向齐射：中路 1 箭 + 两侧路各 1 箭；越界侧补 1 箭（一轮累计 8 箭）
        public override Entity? Shoot(Entity entity)
        {
            var center = base.Shoot(entity);
            var lane = entity.GetLane();
            var maxLane = entity.Level.GetMaxLaneCount();
            for (int offset = -1; offset <= 1; offset += 2)
            {
                var targetLane = lane + offset;
                var side = base.Shoot(entity);
                if (side == null)
                    continue;
                if (targetLane < 0 || targetLane >= maxLane)
                {
                    // 越界行：数量补偿，再补 1 箭
                    base.Shoot(entity);
                }
                else
                {
                    side.StartChangingLane(targetLane);
                }
            }
            return center;
        }

        // 大招：三颗混沌异界星做三体运动（绕过基类扫射状态），但保留基类的升级光效
        protected override void OnEvoke(Entity entity)
        {
            // 与基类一致：大招后永久升级并发光
            if (!entity.HasBuff<HFPDUpgradedBuff>())
                entity.AddBuff<HFPDUpgradedBuff>();
            entity.PlaySound(VanillaSoundID.motor);

            var mates = new List<Entity>();
            for (int i = 0; i < CHAOS_PLANET_COUNT; i++)
            {
                var param = entity.GetShootParams();
                param.damage = entity.GetDamage() * EVOKE_PLANET_DAMAGE_MULT;
                param.projectileID = VanillaProjectileID.chaosHellPlanet;

                // 初速：前进为主 + 随机侧向扰动（三体运动对初值敏感，每次大招轨迹不同）
                var forward = param.velocity.normalized;
                var side = Vector3.Cross(forward, Vector3.up).normalized;
                var lateral = side * (entity.RNG.Next(-10, 10) / 10f) * CHAOS_PLANET_LATERAL_SPEED;
                param.velocity = forward * CHAOS_PLANET_FORWARD_SPEED + lateral;

                var star = entity.ShootProjectile(param);
                if (star != null)
                    mates.Add(star);
            }

            // 互写组员引用，建立三体引力组
            for (int i = 0; i < mates.Count; i++)
            {
                var others = mates.Where((_, j) => j != i).Select(e => new EntityID(e)).ToArray();
                ChaosPlanet.SetMates(mates[i], others);
            }

            entity.PlaySound(VanillaSoundID.odd);
            entity.PlaySound(VanillaSoundID.boon);
        }

        public const int REPEAT_VOLLEY = 4;               // 普攻一轮拍数（每拍三向齐射）
        public const int CHAOS_PLANET_COUNT = 3;          // 大招混沌星数量
        public const int EVOKE_PLANET_DAMAGE_MULT = 50;   // 大招混沌星伤害倍率
        public const float CHAOS_PLANET_FORWARD_SPEED = 8f;  // 混沌星前进初速
        public const float CHAOS_PLANET_LATERAL_SPEED = 3f;  // 混沌星侧向扰动幅度
    }
}
