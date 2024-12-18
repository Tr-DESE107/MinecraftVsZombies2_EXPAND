using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.frankensteinStage)]
    public class FrankensteinStageBuff : BuffDefinition
    {
        public FrankensteinStageBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.DARKNESS_VALUE, NumberOperator.Add, PROP_DARKNESS_ADDITION));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_DARKNESS_ADDITION, 0);
            buff.SetProperty(PROP_THUNDER_TIMEOUT, MAX_THUNDER_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_THUNDER_TIMEOUT);
            timeout--;
            if (timeout <= 0)
            {
                timeout = MAX_THUNDER_TIMEOUT;
                buff.Level.Thunder();
            }
            buff.SetProperty(PROP_THUNDER_TIMEOUT, timeout);


            var time = buff.GetProperty<int>(PROP_TIME);
            if (time < MAX_TIME)
            {
                time++;
                buff.SetProperty(PROP_TIME, time);
            }
            buff.SetProperty(PROP_DARKNESS_ADDITION, (time / (float)MAX_TIME) * 0.5f);
        }
        public const string PROP_DARKNESS_ADDITION = "DarknessAddition";
        public const string PROP_TIME = "Time";
        public const string PROP_THUNDER_TIMEOUT = "ThunderTimeout";
        public const int MAX_THUNDER_TIMEOUT = 150;
        public const int MAX_TIME = 30;
    }
}
