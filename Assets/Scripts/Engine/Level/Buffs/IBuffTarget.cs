using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Level.Buffs;

namespace PVZEngine.Level
{
    public interface IBuffTarget
    {
        Buff CreateBuff(NamespaceID buffID);
        bool AddBuff(Buff buff);
        bool RemoveBuff(Buff buff);
        Entity GetEntity();
        IEnumerable<Buff> GetBuffs();
        BuffReference GetBuffReference(Buff buff);
        Buff GetBuff(int id)
        {
            return GetBuffs().FirstOrDefault(b => b.ID == id);
        }
        bool Exists();
    }
}
