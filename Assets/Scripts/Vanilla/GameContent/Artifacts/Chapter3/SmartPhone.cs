﻿using MVZ2.GameContent.Effects;
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
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            var level = artifact.Level;
            if (level.IsEndless())
            {
                artifact.SetInactive(true);
                artifact.SetNumber(-1);
                return;
            }
            artifact.SetInactive(false);
            if (level.CurrentWave < level.GetTotalWaveCount())
            {
                artifact.SetNumber(GetMoney((int)level.Energy));
                artifact.SetGlowing(false);
            }
            else
            {
                artifact.SetGlowing(true);
            }
        }
        private void PostLevelClearCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition != this)
                    continue;
                var money = artifact.GetNumber();
                if (money <= 0)
                    continue;
                GemEffect.SpawnGemEffects(level, money, level.GetEnergySlotEntityPosition(), null, false);
                artifact.Highlight();
                artifact.SetNumber(0);
            }
        }
        private int GetMoney(int energy)
        {
            if (energy <= 0)
                return 0;
            var unit = (int)(energy / ENERGY_UNIT);
            return unit * MONEY_UNIT;
        }
        public const int ENERGY_UNIT = 10;
        public const int MONEY_UNIT = 10;
    }
}
