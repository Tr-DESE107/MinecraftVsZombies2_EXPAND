using System;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace PVZEngine.Buffs
{
    [Serializable]
    public abstract class BuffReference
    {
        public BuffReference(long buffId)
        {
            this.buffId = buffId;
        }
        public abstract IBuffTarget GetTarget(LevelEngine level);
        public Buff GetBuff(LevelEngine level)
        {
            var target = GetTarget(level);
            if (target == null)
                return null;
            return target.GetBuff(buffId);
        }
        public long buffId;
    }
    [Serializable]
    public class BuffReferenceEntity : BuffReference
    {
        public BuffReferenceEntity(long id, long buffId) : base(buffId)
        {
            entityID = id;
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.FindEntityByID(entityID);
        }
        public long entityID;
    }
    [Serializable]
    public class BuffReferenceArmor : BuffReference
    {
        public BuffReferenceArmor(long id, long buffId) : base(buffId)
        {
            entityID = id;
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.FindEntityByID(entityID)?.EquipedArmor;
        }
        public long entityID;
    }
    [Serializable]
    public class BuffReferenceLevel : BuffReference
    {
        public BuffReferenceLevel(long buffId) : base(buffId)
        {
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level;
        }
    }
    [Serializable]
    public abstract class BuffReferenceSeedPack : BuffReference
    {
        public BuffReferenceSeedPack(long seedId, long buffId) : base(buffId)
        {
            seedID = seedId;
        }
        public long seedID;
    }
    [Serializable]
    public class BuffReferenceClassicSeedPack : BuffReferenceSeedPack
    {
        public BuffReferenceClassicSeedPack(long seedId, long buffId) : base(seedId, buffId)
        {
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.GetSeedPackByID(seedID);
        }
    }
    [Serializable]
    public class BuffReferenceConveyorSeedPack : BuffReferenceSeedPack
    {
        public BuffReferenceConveyorSeedPack(long seedId, long buffId) : base(seedId, buffId)
        {
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.GetConveyorSeedPackByID(seedID);
        }
    }
}
