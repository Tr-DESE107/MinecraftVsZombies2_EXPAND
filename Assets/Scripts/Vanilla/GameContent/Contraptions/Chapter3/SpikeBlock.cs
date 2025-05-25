using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.spikeBlock)]
    public class SpikeBlock : SpikesBehaviour
    {
        public SpikeBlock(string nsp, string name) : base(nsp, name)
        {
        }
        public override int AttackInterval => ATTACK_INTERVAL;
        public const int ATTACK_INTERVAL = 30;
    }
}
