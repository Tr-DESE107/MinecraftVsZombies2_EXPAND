using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.faction)]
    public class FactionBuff : BuffDefinition
    {
        public FactionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_SIZE_MULTIPLIER, Vector3.one);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            int targetFaction = entity.GetFaction();
            var faceRight = targetFaction == buff.Level.Option.LeftFaction;
            var xScale = entity.FaceLeftAtDefault() == faceRight ? -1 : 1;
            buff.SetProperty(PROP_SIZE_MULTIPLIER, new Vector3(xScale, 1, 1));
        }

        public static readonly VanillaBuffPropertyMeta PROP_SIZE_MULTIPLIER = new VanillaBuffPropertyMeta("SizeMultiplier");
    }
}
