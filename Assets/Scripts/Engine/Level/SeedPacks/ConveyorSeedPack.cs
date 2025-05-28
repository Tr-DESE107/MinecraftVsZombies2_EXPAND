using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace PVZEngine.SeedPacks
{
    public class ConveyorSeedPack : SeedPack
    {
        public ConveyorSeedPack(LevelEngine level, SeedDefinition definition, long id) : base(level, definition, id)
        {
        }
        public int GetIndex()
        {
            return Level.GetConveyorSeedPackIndex(this);
        }
        public override BuffReference GetBuffReference(Buff buff)
        {
            return new BuffReferenceConveyorSeedPack(ID, buff.ID);
        }
        #region 序列化
        public SerializableConveyorSeedPack Serialize()
        {
            var seri = new SerializableConveyorSeedPack();
            ApplySerializableProperties(seri);
            return seri;
        }
        public static ConveyorSeedPack Deserialize(SerializableConveyorSeedPack seri, LevelEngine level)
        {
            var definition = level.Content.GetSeedDefinition(seri.seedID);
            var seedPack = new ConveyorSeedPack(level, definition, seri.id);
            seedPack.ApplyDeserializedProperties(level, seri);
            return seedPack;
        }
        #endregion
    }
}
