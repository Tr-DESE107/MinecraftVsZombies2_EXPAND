﻿using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.faction)]
    public class FactionBuff : BuffDefinition
    {
        public FactionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.FLIP_X, PROP_FLIP_X));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            int targetFaction = entity.GetFaction();
            var faceRight = targetFaction == buff.Level.Option.LeftFaction;
            buff.SetProperty(PROP_FLIP_X, entity.FaceLeftAtDefault() == faceRight);
        }

        public static readonly VanillaBuffPropertyMeta<bool> PROP_FLIP_X = new VanillaBuffPropertyMeta<bool>("FlipX");
    }
}
