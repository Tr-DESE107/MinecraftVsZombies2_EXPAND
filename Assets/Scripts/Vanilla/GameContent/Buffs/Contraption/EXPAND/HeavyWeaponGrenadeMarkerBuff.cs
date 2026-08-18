#nullable enable

using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Projectiles;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponGrenadeMarker)]
    public class HeavyWeaponGrenadeMarkerBuff : BuffDefinition
    {
        public HeavyWeaponGrenadeMarkerBuff(string nsp, string name) : base(nsp, name) { }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var marker = buff.GetEntity();
            if (marker == null || marker.IsDead)
                return;
            var level = marker.Level;

            // —— 动画：标识坐标快速飞向鼠标 ——  
            if (Global.Input.TryGetPointerScreenPosition(out var screenPos))
            {
                var mouse = level.ScreenToLawnPositionByY(screenPos, 0);
                mouse.y = level.GetGroundY(mouse.x, mouse.z);
                marker.Position = Vector3.Lerp(marker.Position, mouse, FOLLOW_DAMP);
            }

            // —— 倒计时 ——  
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                ThrowTNT(buff, marker, level);
                marker.Remove();
            }
        }

        private void ThrowTNT(Buff buff, Entity marker, LevelEngine level)
        {
            // 从矿车处生成，抛向标识（此时标识≈鼠标位置）  
            var cart = marker.Parent;
            var origin = cart.ExistsAndAlive() ? cart!.GetCenter() : marker.Position;
            var target = marker.Position;
            int faction = buff.GetProperty<int>(PROP_FACTION);

            marker.PlaySound(VanillaSoundID.HeavyWeaponGrenade);

            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, faction); // 阵营跟随矿车上的器械  
            var tnt = level.Spawn(VanillaProjectileID.Grenade, origin, cart, param);
            if (tnt == null)
                return;
            float maxY = Mathf.Max(origin.y, target.y) + THROW_ARC;
            tnt.Velocity = VanillaProjectileExt.GetLobVelocity(origin, target, maxY, tnt.GetGravity());
        }

        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("Faction");
        public const int COUNTDOWN_FRAMES = 90;  // ≈3秒，按你的 TPS 调整（若1秒=30帧则=3秒）  
        public const float THROW_ARC = 200f;     // 抛物线弧高  
        public const float FOLLOW_DAMP = 0.35f;  // 越大标识越快贴上鼠标  
    }
}
