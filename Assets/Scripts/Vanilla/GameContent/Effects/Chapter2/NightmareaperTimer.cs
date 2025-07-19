using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    // 定义一个效果类 NightmareaperTimer，绑定到游戏中名为 "nightmareaperTimer" 的效果实体
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareaperTimer)]
    public class NightmareaperTimer : EffectBehaviour
    {
        // 构造函数，传入命名空间和名字，调用基类构造器
        public NightmareaperTimer(string nsp, string name) : base(nsp, name)
        {
        }

        // 初始化时调用，比如效果第一次生效时触发
        public override void Init(Entity entity)
        {
            base.Init(entity);
            // 获取当前关卡（Level）中为“梦魇收割者（Nightmareaper）”设定的倒计时时长
            var timeout = entity.Level.GetNightmareaperTimeout();
            // 将倒计时值保存到实体的属性中
            SetTimeout(entity, timeout);
        }

        // 每帧更新调用，用于刷新计时器逻辑
        public override void Update(Entity entity)
        {
            base.Update(entity);
            // 更新倒计时计数和视觉效果
            UpdateTimer(entity);
        }

        // 计时器更新函数，控制倒计时递减及倒计时结束后的行为
        private void UpdateTimer(Entity entity)
        {
            // 取出当前倒计时数值
            var timeout = GetTimeout(entity);
            if (timeout > 0)
            {
                // 每帧递减倒计时
                timeout--;
                // 如果倒计时结束（<=0），触发激怒收割者的效果
                if (timeout <= 0)
                {
                    EnrageReapers(entity);
                }
                // 保存倒计时更新后的值
                SetTimeout(entity, timeout);
            }

            // 根据剩余时间计算实体模型颜色（从红色到白色渐变）
            // timeout/900f 意味着最大倒计时为900帧，倒计时越长越偏红，越少越偏白
            Color tint = Color.Lerp(Color.red, Color.white, timeout / 900f);
            // 给实体模型设置颜色属性，用于视觉提示
            entity.SetModelProperty("Color", tint);
            // 同时将当前倒计时传给模型属性，方便前端或动画使用
            entity.SetModelProperty("Timeout", timeout);
        }

        // 激怒梦魇收割者的处理函数
        private void EnrageReapers(Entity entity)
        {
            // 获取当前关卡对象
            var level = entity.Level;
            // 停止当前播放的音乐
            level.StopMusic();
            // 遍历当前关卡中所有 Nightmareaper 类型的实体
            foreach (Entity nightmareaper in level.FindEntities(VanillaBossID.nightmareaper))
            {
                // 触发 Nightmareaper 的激怒状态
                Nightmareaper.Enrage(nightmareaper);
                // 给该实体添加激怒状态的 Buff（持续效果）
                nightmareaper.AddBuff<NightmareaperEnragedBuff>();
            }
        }

        // 获取该实体计时器的超时属性（倒计时数值）
        public static int GetTimeout(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_TIMEOUT);
        // 设置该实体计时器的超时属性（倒计时数值）
        public static void SetTimeout(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_TIMEOUT, value);

        // 定义一个属性元数据，表示倒计时属性
        public static readonly VanillaEntityPropertyMeta<int> PROP_TIMEOUT = new VanillaEntityPropertyMeta<int>("Timeout");
        // 定义该效果的唯一标识 ID
        public static readonly NamespaceID ID = VanillaEffectID.nightmareaperTimer;
    }
}
