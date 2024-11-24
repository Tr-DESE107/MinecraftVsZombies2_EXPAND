using PVZEngine;

namespace MVZ2Logic.Level
{
    public interface IAreaMeta
    {
        public string ID { get; }
        public NamespaceID ModelID { get; set; }
        public NamespaceID MusicID { get; set; }
    }
}
