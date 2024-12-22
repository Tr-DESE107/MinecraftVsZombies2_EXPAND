using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;

namespace PVZEngine.Buffs
{
    public interface IBuffTarget
    {
        Buff CreateBuff(NamespaceID buffID);
        bool AddBuff(Buff buff);
        bool RemoveBuff(Buff buff);
        Entity GetEntity();
        IEnumerable<Buff> GetBuffs();
        BuffReference GetBuffReference(Buff buff);
        Buff GetBuff(long id)
        {
            return GetBuffs().FirstOrDefault(b => b.ID == id);
        }
        bool Exists();
    }
}
