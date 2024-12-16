using System;
using PVZEngine.Level;

namespace PVZEngine.Buffs
{
    [Serializable]
    public abstract class BuffReference
    {
        public abstract IBuffTarget GetTarget(LevelEngine level);
        public Buff GetBuff(LevelEngine level)
        {
            var target = GetTarget(level);
            if (target == null)
                return null;
            return target.GetBuff(buffId);
        }
        public int buffId;
    }
    [Serializable]
    public class BuffReferenceEntity : BuffReference
    {
        public BuffReferenceEntity(long id)
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
        public BuffReferenceArmor(long id)
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
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level;
        }
    }
    [Serializable]
    public abstract class BuffReferenceSeedPack : BuffReference
    {
        public BuffReferenceSeedPack(long seedId)
        {
            seedID = seedId;
        }
        public long seedID;
    }
    [Serializable]
    public class BuffReferenceClassicSeedPack : BuffReferenceSeedPack
    {
        public BuffReferenceClassicSeedPack(long seedId) : base(seedId)
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
        public BuffReferenceConveyorSeedPack(long seedId) : base(seedId)
        {
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.GetConveyorSeedPackByID(seedID);
        }
    }
}
