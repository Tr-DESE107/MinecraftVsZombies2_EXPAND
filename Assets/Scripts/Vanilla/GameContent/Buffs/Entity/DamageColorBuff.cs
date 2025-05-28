﻿using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.damageColor)]
    public class DamageColorBuff : BuffDefinition
    {
        public DamageColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, new Color(1, 0, 0, 0.5f)));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, 2);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout > 0)
                return;
            var entity = buff.GetEntity();
            if (entity == null || !entity.IsDead || entity.Type == EntityTypes.BOSS)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
    }
}
