using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.reverseSatellite)]
    public class ReverseSatelliteBuff : BuffDefinition
    {
        public ReverseSatelliteBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(LogicLevelProps.CAMERA_ROTATION, NumberOperator.Add, PROP_CAMERA_ROTATION));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.Level.PlaySound(VanillaSoundID.boon);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            buff.SetProperty(PROP_TIME, time);

            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            if (buff.Level.EntityExists(VanillaEnemyID.reverseSatellite))
            {
                timeout = MAX_TIMEOUT;
            }
            else
            {
                timeout--;
            }
            buff.SetProperty(PROP_TIMEOUT, timeout);


            var rotation = Mathf.Min(time, timeout) / (float)MAX_TIMEOUT;
            rotation = MathTool.EaseInAndOut(rotation);
            buff.SetProperty(PROP_CAMERA_ROTATION, rotation * MAX_ROTATION);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public const string PROP_CAMERA_ROTATION = "CameraRotation";
        public const string PROP_TIME = "Time";
        public const string PROP_TIMEOUT = "Timeout";
        public const int MAX_TIMEOUT = 30;
        public const int MAX_ROTATION = 180;
    }
}
