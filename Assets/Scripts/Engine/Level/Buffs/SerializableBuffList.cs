using System;
using System.Collections.Generic;

namespace PVZEngine.Buffs
{
    [Serializable]
    public class SerializableBuffList
    {
        public List<SerializableBuff> buffs;
        public long currentBuffID;
    }
}
