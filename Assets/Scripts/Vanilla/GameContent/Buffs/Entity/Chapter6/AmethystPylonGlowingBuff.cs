#nullable enable

using System.Collections.Generic;

using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Modifiers;
using MVZ2.Vanilla.Properties;

using MVZ2Logic.Entities;
using MVZ2Logic.Models;

using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

using Tools;

using UnityEngine;

namespace MVZ2.GameContent.Buffs.Entities
{
    // 紫晶塔激光命中"幽灵/怨灵"实体时赋予的发光 buff。  
    // 参照 TransfenserGlowingBuff：让实体自身成为光源 + 模型发光 + 定时移除。  
    // 区别：无易伤(不注册 PRE_ENTITY_TAKE_DAMAGE)，且光色/光等级可由紫晶塔当前输出等级配置。  
    [AutoBuffDefinition(VanillaBuffNames.Enemy.AmethystPylonGlowing)]
    public class AmethystPylonGlowingBuff : BuffDefinition
    {
        public AmethystPylonGlowingBuff(string nsp, string name) : base(nsp, name)
        {
            // 让被照实体自己成为一个光源  
            AddModifier(new BooleanModifier(LogicEntityProps.IS_LIGHT_SOURCE, true));
            // 光色由 buff 属性 PROP_LIGHT_COLOR 决定(写法同 HellfireIgnitedBuff)  
            AddModifier(ColorModifier.Override(LogicEntityProps.LIGHT_COLOR, PROP_LIGHT_COLOR, priority: VanillaModifierPriorities.FORCE));
            // 光照范围固定  
            AddModifier(new Vector3Modifier(LogicEntityProps.LIGHT_RANGE, NumberOperator.Set, LIGHT_RANGE, priority: VanillaModifierPriorities.FORCE));
            // 光照等级由 buff 属性 PROP_LIGHT_LEVEL 决定(写法同 HellfireIgnitedBuff)，  
            // 供 WraithBuff/八色系统判定"是否被匹配等级照亮"  
            AddModifier(new IntModifier(LogicEntityProps.LIGHT_LEVEL, IntegerOperator.Set, PROP_LIGHT_LEVEL , priority: VanillaModifierPriorities.FORCE));
            // 注意：这里刻意不加任何伤害倍率触发，即"无易伤"  
        }
        public override void OnCreate(Buff buff)
        {
            base.OnCreate(buff);
            SetTimer(buff, TimerHelper.NewSecondTimer(GLOW_SECONDS));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = GetTimer(buff);
            if (timer.RunToExpiredOrNull())
            {
                buff.Remove();  // 计时结束自动移除，实体恢复非光源  
            }
        }

        // ===== 供激光调用：给实体挂/刷新发光，并设置光色与等级 =====  
        // 若实体已有该 buff 则复用(避免重复叠加 Override)，否则新建  
        public static void GiveGlow(Entity entity, int level)
        {
            buffBuffer.Clear();
            entity.GetBuffs<AmethystPylonGlowingBuff>(buffBuffer);
            var buff = buffBuffer.Count > 0 ? buffBuffer[0] : entity.AddBuff<AmethystPylonGlowingBuff>();
            SetLightLevel(buff, level);
            SetLightColor(buff, GetSpectrumColor(level));
            SetTimer(buff, TimerHelper.NewSecondTimer(GLOW_SECONDS)); // 每次命中刷新持续时间  
        }

        // 光等级 -> 八色(0白 1红 2橙 3黄 4绿 5青 6蓝 7紫)  
        public static Color GetSpectrumColor(int level)
        {
            switch (((level % 8) + 8) % 8)
            {
                case 0: return Color.white;
                case 1: return new Color(1f, 0f, 0f);
                case 2: return new Color(1f, 0.5f, 0f);
                case 3: return new Color(1f, 1f, 0f);
                case 4: return new Color(0f, 1f, 0f);
                case 5: return new Color(0f, 1f, 1f);
                case 6: return new Color(0f, 0f, 1f);
                case 7: return new Color(0.545f, 0f, 1f);
                default: return Color.white;
            }
        }

        public static FrameTimer? GetTimer(Buff buff) => buff.GetProperty<FrameTimer>(PROP_TIMER);
        public static void SetTimer(Buff buff, FrameTimer? value) => buff.SetProperty(PROP_TIMER, value);
        public static void SetLightColor(Buff buff, Color value) => buff.SetProperty(PROP_LIGHT_COLOR, value);
        public static void SetLightLevel(Buff buff, int value) => buff.SetProperty(PROP_LIGHT_LEVEL, value);

        public const float GLOW_SECONDS = 6f; // 发光持续时间，可按需调整  
        public static readonly Vector3 LIGHT_RANGE = Vector3.one * 64f;
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_LIGHT_COLOR = new VanillaBuffPropertyMeta<Color>("lightColor");
        public static readonly VanillaBuffPropertyMeta<int> PROP_LIGHT_LEVEL = new VanillaBuffPropertyMeta<int>("lightLevel");
        private static List<Buff> buffBuffer = new List<Buff>();
    }
}
