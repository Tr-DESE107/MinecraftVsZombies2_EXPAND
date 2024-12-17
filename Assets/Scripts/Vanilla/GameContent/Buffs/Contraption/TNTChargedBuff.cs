using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.tntCharged)]
    public class TNTChargedBuff : BuffDefinition
    {
        public TNTChargedBuff(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
