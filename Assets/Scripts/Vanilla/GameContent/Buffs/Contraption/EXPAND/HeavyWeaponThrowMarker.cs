#nullable enable

using MVZ2.Vanilla.Projectiles;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponThrowMarker)]
    public class HeavyWeaponThrowMarkerBuff : BuffDefinition
    {
        public HeavyWeaponThrowMarkerBuff(string nsp, string name) : base(nsp, name) { }

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            // 初始化倒计时（防止外部未设置时第一帧就引爆）  
            if (buff.GetProperty<int>(PROP_TIMEOUT) <= 0)
                buff.SetProperty(PROP_TIMEOUT, COUNTDOWN_FRAMES);

            // 兜底：给标识本身设一个略大于倒计时的 Timeout，防止引用丢失时残留  
            var marker = buff.GetEntity();
            if (marker != null && !marker.IsDead)
                marker.Timeout = buff.GetProperty<int>(PROP_TIMEOUT) + TIMEOUT_MARGIN;
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var marker = buff.GetEntity();
            if (marker == null || marker.IsDead)
                return;
            var level = marker.Level;

            // 矿车（父实体）消失则立即移除标识，避免孤儿残留  
            var cart = marker.Parent;
            if (!cart.ExistsAndAlive())
            {
                marker.Remove();
                return;
            }

            // 标识飞向鼠标  
            if (Global.Input.TryGetPointerScreenPosition(out var screenPos))
            {
                var mouse = level.ScreenToLawnPositionByY(screenPos, 0);
                mouse.y = level.GetGroundY(mouse.x, mouse.z);
                marker.Position = Vector3.Lerp(marker.Position, mouse, FOLLOW_DAMP);
            }

            // 倒计时  
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                Throw(buff, marker, level);
                marker.Remove();
            }
        }

        private void Throw(Buff buff, Entity marker, LevelEngine level)
        {
            var cart = marker.Parent;
            var origin = cart.ExistsAndAlive() ? cart!.GetCenter() : marker.Position;
            var target = marker.Position;
            int faction = buff.GetProperty<int>(PROP_FACTION);

            // —— 差异点全部由属性驱动 ——  
            var soundID = buff.GetProperty<NamespaceID>(PROP_SOUND_ID);
            var projectileID = buff.GetProperty<NamespaceID>(PROP_PROJECTILE_ID);
            if (!NamespaceID.IsValid(projectileID))
                return;

            if (NamespaceID.IsValid(soundID))
                marker.PlaySound(soundID);

            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, faction);
            var proj = level.Spawn(projectileID, origin, cart, param);
            if (proj == null)
                return;
            float maxY = Mathf.Max(origin.y, target.y) + THROW_ARC;
            proj.Velocity = VanillaProjectileExt.GetLobVelocity(origin, target, maxY, proj.GetGravity());
        }

        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("Faction");
        // 差异点：音效与弹射物均为 NamespaceID，由蓝图注入  
        public static readonly VanillaBuffPropertyMeta<NamespaceID> PROP_SOUND_ID = new VanillaBuffPropertyMeta<NamespaceID>("SoundID");
        public static readonly VanillaBuffPropertyMeta<NamespaceID> PROP_PROJECTILE_ID = new VanillaBuffPropertyMeta<NamespaceID>("ProjectileID");

        public const int COUNTDOWN_FRAMES = 30;
        public const int TIMEOUT_MARGIN = 15;
        public const float THROW_ARC = 200f;
        public const float FOLLOW_DAMP = 0.35f;
    }
}
