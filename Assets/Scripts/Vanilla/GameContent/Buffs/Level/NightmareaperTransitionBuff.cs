using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [Definition(VanillaBuffNames.Level.nightmareaperTransition)]
    public class NightmareaperTransitionBuff : BuffDefinition
    {
        public NightmareaperTransitionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(LogicLevelProps.BLACKSCREEN, NumberOperator.Add, PROP_BLACK_SCREEN));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
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

                buff.Remove();
            }
        }
        public const string PROP_TIMEOUT = "Timeout";
        public const string PROP_BLACK_SCREEN = "BlackScreen";
        public const float BLACK_SCREEN_SPEED = 1 / 180f;
        public const int MAX_TIMEOUT = 270;
        public const int CREATE_GLASS_TIMEOUT = 60;
    }
}
