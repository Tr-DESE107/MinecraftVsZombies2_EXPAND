using System;

namespace PVZEngine.Level.Buffs
{
    [Serializable]
    public abstract class BuffReference
    {
        public abstract IBuffTarget GetTarget(LevelEngine level);
        public Buff GetBuff(LevelEngine level)
        {
            var target = GetTarget(level);
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
    public class BuffReferenceSeedPack : BuffReference
    {
        public BuffReferenceSeedPack(int seedId)
        {
            seedID = seedId;
        }
        public override IBuffTarget GetTarget(LevelEngine level)
        {
            return level.GetSeedPackByID(seedID);
        }
        public int seedID;
    }
}
