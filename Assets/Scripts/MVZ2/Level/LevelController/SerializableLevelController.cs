using System;
using MongoDB.Bson.Serialization.Attributes;
using MVZ2.Entities;
using MVZ2.Level.UI;
using MVZ2.Models;
using PVZEngine;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Level
{
    [BsonIgnoreExtraElements]
    public class SerializableLevelController
    {
        public SerializableRNG rng;

        public float levelProgress;
        public float[] bannerProgresses;
        public float bossProgress;
        public NamespaceID bossProgressBarStyle;
        public bool progressBarMode;

        public NamespaceID musicID;
        public float musicTime;
        public float musicVolume;
        public float musicTrackWeight;

        public bool energyActive;
        public bool blueprintsActive;
        public bool pickaxeActive;
        public bool starshardActive;
        public bool triggerActive;

        public SerializableLevelControllerPart[] parts;

        public int maxCryTime;
        public FrameTimer cryTimer;
        public float twinkleTime;

        public SerializableEntityController[] entities;
        public SerializableModelData model;
        [Obsolete]
        public SerializableAreaModelData areaModel;

        public SerializableLevel level;

        public SerializableLevelUIPreset uiPreset;
    }
}
