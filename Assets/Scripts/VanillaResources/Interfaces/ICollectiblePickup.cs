using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public interface ICollectiblePickup
    {
        void PostCollect(Entity pickup);
    }
}
