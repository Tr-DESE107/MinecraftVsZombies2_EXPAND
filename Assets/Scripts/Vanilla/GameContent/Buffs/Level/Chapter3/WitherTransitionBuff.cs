using MVZ2.GameContent.Effects;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.witherTransition)]
    public class WitherTransitionBuff : BuffDefinition
    {
        public WitherTransitionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(LogicLevelProps.SCREEN_COVER, PROP_SCREEN_COVER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            timer.Run();

            var level = buff.Level;
            var state = buff.GetProperty<int>(PROP_STATE);
            switch (state)
            {
                case STATE_START:
                    // 音乐放缓。
                    level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));
                    if (timer.Expired)
                    {
                        var column = level.GetMaxColumnCount() / 2;
                        var lane = level.GetMaxLaneCount() / 2;
                        var x = level.GetEntityColumnX(column);
                        var z = level.GetEntityLaneZ(lane);
                        var y = level.GetGroundY(x, z) + 80;
                        level.Spawn(VanillaEffectID.witherSummoners, new Vector3(x, y, z), null);
                        level.SetMusicVolume(1);
                        level.PlayMusic(VanillaMusicID.witherBoss);

                        buff.SetProperty(PROP_STATE, STATE_SUMMONING);
                    }
                    break;
                case STATE_SUMMONING:
                    if (level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && !e.IsDead))
                    {
                        // 凋灵出现
                        level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;
                        level.SetProgressBarToBoss(VanillaProgressBarID.wither);
                        level.ShakeScreen(30, 0, 30);

                        buff.SetProperty(PROP_STATE, STATE_SUMMONED);
                        buff.SetProperty(PROP_SCREEN_COVER, Color.white);
                        timer.ResetTime(2);
                    }
                    break;
                case STATE_SUMMONED:
                    if (timer.Expired)
                    {
                        buff.Remove();
                    }
                    break;
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
        public static readonly VanillaBuffPropertyMeta PROP_STATE = new VanillaBuffPropertyMeta("State");
        public static readonly VanillaBuffPropertyMeta PROP_SCREEN_COVER = new VanillaBuffPropertyMeta("ScreenCover");
        public const int STATE_START = 0;
        public const int STATE_SUMMONING = 1;
        public const int STATE_SUMMONED = 2;
        public const int MAX_TIMEOUT = 90;
    }
}
