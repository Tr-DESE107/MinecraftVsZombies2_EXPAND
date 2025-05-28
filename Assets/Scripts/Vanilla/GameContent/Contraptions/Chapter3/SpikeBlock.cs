﻿using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.spikeBlock)]
    public class SpikeBlock : SpikesBehaviour
    {
        public SpikeBlock(string nsp, string name) : base(nsp, name)
        {
        }
        public override int AttackCooldown => ATTACK_COOLDOWN;
        public const int ATTACK_COOLDOWN = 30;
    }
}
