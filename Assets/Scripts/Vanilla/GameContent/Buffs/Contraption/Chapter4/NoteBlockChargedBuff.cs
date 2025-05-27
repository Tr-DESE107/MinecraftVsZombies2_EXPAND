using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.noteBlockCharged)]
    public class NoteBlockChargedBuff : BuffDefinition
    {
        public NoteBlockChargedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));
        }

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateColorOffset(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            buff.SetProperty(PROP_TIME, time);
            UpdateColorOffset(buff);
            if (time >= MAX_TIME)
            {
                buff.Remove();
            }
        }
        private void UpdateColorOffset(Buff buff)
        {
            var time = buff.GetProperty<int>(PROP_TIME);
            var alpha = 1 - (time / (float)MAX_TIME);
            buff.SetProperty(PROP_COLOR_OFFSET, new Color(1, 1, 1, alpha));
        }
        public const int MAX_TIME = 90;
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIME = new VanillaBuffPropertyMeta<int>("Time");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR_OFFSET = new VanillaBuffPropertyMeta<Color>("ColorOffset");
    }
}
