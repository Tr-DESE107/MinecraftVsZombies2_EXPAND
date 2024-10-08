using System;
using PVZEngine;
using PVZEngine.Level.Buffs;
using Tools;

namespace PVZEngine.Auras
{
    [Serializable]
    public class SerializableAuraEffect
    {
        public int id;
        public FrameTimer updateTimer;
        public BuffReference[] buffs;
    }
}
