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
        public static void SetVariant(Buff buff, int type) => buff.SetProperty(PROP_VARIANT, type);
        public static int GetVariant(Buff buff) => buff.GetProperty<int>(PROP_VARIANT);
        public static readonly VanillaBuffPropertyMeta<int> PROP_VARIANT = new VanillaBuffPropertyMeta<int>("variant");
    }
}
