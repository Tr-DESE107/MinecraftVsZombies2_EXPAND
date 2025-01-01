
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.pistenserExtend)]
    public class PistenserExtendBuff : BuffDefinition
    {
        public PistenserExtendBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHOT_OFFSET, NumberOperator.Add, PROP_SHOT_OFFSET));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var extend = Pistenser.GetExtend(entity);
            buff.SetProperty(PROP_SHOT_OFFSET, Vector3.up * extend);
        }
        public const string PROP_SHOT_OFFSET = "ShotOffset";
    }
}
