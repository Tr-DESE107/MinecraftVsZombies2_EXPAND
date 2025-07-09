using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.ufoSpawn)]
    public class UFOSpawnBuff : BuffDefinition
    {
        public UFOSpawnBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public static void SetType(Buff buff, int type) => buff.SetProperty(PROP_TYPE, type);
        public static int GetType(Buff buff) => buff.GetProperty<int>(PROP_TYPE);
        public static readonly VanillaBuffPropertyMeta<int> PROP_TYPE = new VanillaBuffPropertyMeta<int>("type");
    }
}
