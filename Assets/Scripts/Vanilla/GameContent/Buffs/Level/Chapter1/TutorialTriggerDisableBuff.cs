﻿using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.tutorialTriggerDisable)]
    public class TutorialTriggerDisableBuff : BuffDefinition
    {
        public TutorialTriggerDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(VanillaLevelProps.TRIGGER_DISABLE_ID, VanillaBlueprintErrors.tutorial));
        }
    }
}
