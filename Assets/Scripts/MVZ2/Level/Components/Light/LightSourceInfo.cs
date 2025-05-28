﻿using System;
using System.Linq;
using PVZEngine.Base;

namespace MVZ2.Level.Components
{
    public class LightSourceInfo
    {
        public UpdateList<long> illuminatingEntities = new UpdateList<long>();
    }
    [Serializable]
    public class SerializableLightSourceInfo
    {
        public SerializableLightSourceInfo(long id, LightSourceInfo info)
        {
            this.id = id;
            illuminatingEntities = info.illuminatingEntities.ToArray();
        }
        public LightSourceInfo ToDeserialized()
        {
            return new LightSourceInfo()
            {
                illuminatingEntities = new UpdateList<long>(illuminatingEntities)
            };
        }
        public long id;
        public long[] illuminatingEntities;
    }
}
