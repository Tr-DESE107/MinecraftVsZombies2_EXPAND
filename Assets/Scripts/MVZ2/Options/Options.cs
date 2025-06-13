using System;
using PVZEngine;

namespace MVZ2.Options
{
    public class Options
    {
        public Options()
        {
            keyBindings = new KeyBindingOptions();
        }
        public SerializableOptions ToSerializable()
        {
            return new SerializableOptions()
            {
                keyBindings = keyBindings?.ToSerializable(),
                skipAllTalks = skipAllTalks,
                showSponsorNames = showSponsorNames,
                blueprintWarningsDisabled = blueprintWarningsDisabled,
                commandBlockMode = commandBlockMode,
                fpsMode = fpsMode,
                showHotkeyIndicators = showHotkeyIndicators,
            };
        }
        public void LoadFromSerializable(SerializableOptions options)
        {
            if (options == null)
                return;
            keyBindings.LoadFromSerializable(options.keyBindings);
            skipAllTalks = options.skipAllTalks;
            showSponsorNames = options.showSponsorNames;
            blueprintWarningsDisabled = options.blueprintWarningsDisabled;
            commandBlockMode = options.commandBlockMode;
            fpsMode = options.fpsMode;
            showHotkeyIndicators = options.showHotkeyIndicators;
        }
        public bool swapTrigger;
        public bool vibration;
        public NamespaceID difficulty;
        public bool bloodAndGore;
        public bool pauseOnFocusLost;

        public float musicVolume;
        public float soundVolume;
        public float fastForwardMultiplier;
        public float particleAmount;
        public float shakeAmount;

        public string language;

        public KeyBindingOptions keyBindings;
        public bool skipAllTalks;
        public bool showSponsorNames;
        public bool blueprintWarningsDisabled;
        public int commandBlockMode;
        public int fpsMode;
        public bool showHotkeyIndicators;
    }
    [Serializable]
    public class SerializableOptions
    {
        public SerializableKeyBindingOptions keyBindings;
        public bool skipAllTalks;
        public bool showSponsorNames;
        public bool blueprintWarningsDisabled;
        public int commandBlockMode;
        public int fpsMode;
        public bool showHotkeyIndicators;
    }
}
