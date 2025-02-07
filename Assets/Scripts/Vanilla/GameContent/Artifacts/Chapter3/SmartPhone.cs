using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.smartPhone)]
    public class SmartPhone : ArtifactDefinition
    {
        public SmartPhone(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_CLEAR, PostLevelClearCallback);
        }
        private void PostLevelClearCallback(LevelEngine level)
        {
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition != this)
                    continue;
                var energy = level.Energy;
                if (energy <= 0)
                    continue;
                level.SetEnergy(0);
                var unit = (int)(energy / ENERGY_UNIT);
                var money = unit * MONEY_UNIT;
                GemEffect.SpawnGemEffects(level, money, level.GetEnergySlotEntityPosition(), null, false);
                artifact.Highlight();
            }
        }
        public const int ENERGY_UNIT = 50;
        public const int MONEY_UNIT = 10;
    }
}
