﻿using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.nightmareaperTransition)]
    public class NightmareaperTransitionBuff : BuffDefinition
    {
        public NightmareaperTransitionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(LogicLevelProps.SCREEN_COVER, PROP_SCREEN_COVER));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_ALL, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            var level = buff.Level;

            if (timeout == CREATE_GLASS_TIMEOUT)
            {
                // 创建梦魇玻璃
                level.Spawn(VanillaEffectID.nightmareGlass, Vector3.zero, null);
                // Play Sound.
                level.PlaySound(VanillaSoundID.dialogJudge);
            }
            else if (timeout <= 0)
            {
                // 梦魇收割者出现
                level.PlayMusic(VanillaMusicID.nightmareBoss2);
                // Create Boss
                Vector3 pos = new Vector3(level.GetEntityColumnX(4), 0, level.GetEntityLaneZ(2));
                var boss = level.Spawn(VanillaBossID.nightmareaper, pos, null);
                Nightmareaper.Appear(boss);

                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_CLICK_TO_DRAG_CRUSHING_WALLS, 100, 120);

                buff.Remove();
            }
            var blackScreen = buff.GetProperty<Color>(PROP_SCREEN_COVER);
            blackScreen.a = Mathf.Clamp01(blackScreen.a + BLACK_SCREEN_SPEED);
            buff.SetProperty(PROP_SCREEN_COVER, blackScreen);
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_SCREEN_COVER = new VanillaBuffPropertyMeta<Color>("ScreenCover");
        public const float BLACK_SCREEN_SPEED = 1 / 180f;
        public const int MAX_TIMEOUT = 270;
        public const int CREATE_GLASS_TIMEOUT = 60;
    }
}
