﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Effects
{
    [BuffDefinition(VanillaBuffNames.Effect.breakoutBoardUpgrade)]
    public class BreakoutBoardUpgradeBuff : BuffDefinition
    {
        public BreakoutBoardUpgradeBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SIZE, NumberOperator.Add, new Vector3(0, 0, 80)));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_SCALE, NumberOperator.Add, new Vector3(0, 1, 0)));
        }
    }
}
