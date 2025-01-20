using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using PVZEngine.Models;

namespace PVZEngine.Buffs
{
    public interface IBuffTarget
    {
        Buff CreateBuff(NamespaceID buffID);
        bool AddBuff(Buff buff);
        bool RemoveBuff(Buff buff);
        IModelInterface GetInsertedModel(NamespaceID key);
        Entity GetEntity();
        void GetBuffs(List<Buff> results);
        BuffReference GetBuffReference(Buff buff);
        Buff GetBuff(long id);
        bool Exists();
    }
}
