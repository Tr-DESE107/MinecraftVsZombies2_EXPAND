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
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponFlashbangMarkerBuff)]
    public class HeavyWeaponFlashbangMarkerBuff : BuffDefinition
    {
        public HeavyWeaponFlashbangMarkerBuff(string nsp, string name) : base(nsp, name) { }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            // 初始化倒计时（防止外部未设置时 GetProperty 返回 0 导致第一帧就引爆）  
            if (buff.GetProperty<int>(PROP_TIMEOUT) <= 0)
                buff.SetProperty(PROP_TIMEOUT, COUNTDOWN_FRAMES);

            // 兜底：给标识实体本身设一个略大于倒计时的 Timeout。  
            // 即使 PostUpdate 因某种原因停摆（如引用丢失、暂停边界情况），  
            // 标识也会在这个时间后由引擎自动移除，绝不会永久残留。  
            var marker = buff.GetEntity();
            if (marker != null && !marker.IsDead)
                marker.Timeout = COUNTDOWN_FRAMES + TIMEOUT_MARGIN;
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var marker = buff.GetEntity();
            if (marker == null || marker.IsDead)
                return;
            var level = marker.Level;

            // —— 安全清理：矿车（父实体）或其上的器械已消失，则立即移除标识 ——  
            // 器械在倒计时中途死亡时，矿车会因无骑乘者而自毁，此时标识失去意义，  
            // 必须主动移除，否则会成为孤儿实体永久残留在场上。  
            var cart = marker.Parent;
            if (!cart.ExistsAndAlive())
            {
                marker.Remove();
                return;
            }

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

            marker.PlaySound(VanillaSoundID.HeavyWeaponFlashbang);

            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, faction); // 阵营跟随矿车上的器械  
            var tnt = level.Spawn(VanillaProjectileID.Flashbang, origin, cart, param);
            if (tnt == null)
                return;
            float maxY = Mathf.Max(origin.y, target.y) + THROW_ARC;
            tnt.Velocity = VanillaProjectileExt.GetLobVelocity(origin, target, maxY, tnt.GetGravity());
        }

        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("Faction");
        public const int COUNTDOWN_FRAMES = 45;  // 1.5s
        public const int TIMEOUT_MARGIN = 15;    // 标识兜底 Timeout 相对倒计时的余量帧数    
        public const float THROW_ARC = 200f;     // 抛物线弧高  
        public const float FOLLOW_DAMP = 0.35f;  // 越大标识越快贴上鼠标  
    }
}
