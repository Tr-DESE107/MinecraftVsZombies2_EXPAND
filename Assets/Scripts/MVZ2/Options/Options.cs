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
                keyBindings = keyBindings?.ToSerializable()
            };
        }
        public void LoadFromSerializable(SerializableOptions options)
        {
            if (options == null)
                return;
            keyBindings.LoadFromSerializable(options.keyBindings);
        }
        public bool swapTrigger;
        public bool vibration;
        public NamespaceID difficulty;
        public bool bloodAndGore;
        public bool pauseOnFocusLost;
        public bool skipAllTalks;

        public float musicVolume;
        public float soundVolume;
        public float fastForwardMultiplier;
        public float particleAmount;
        public float shakeAmount;

        public string language;
        public bool showSponsorNames;
        public KeyBindingOptions keyBindings;
    }
    [Serializable]
    public class SerializableOptions
    {
        public SerializableKeyBindingOptions keyBindings;
    }
}
