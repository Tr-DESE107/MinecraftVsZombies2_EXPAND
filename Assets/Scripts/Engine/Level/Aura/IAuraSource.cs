using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace PVZEngine.Auras
{
    public interface IAuraSource
    {
        Entity GetEntity();
        LevelEngine GetLevel();
    }
}
