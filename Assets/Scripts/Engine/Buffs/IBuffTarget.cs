using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine
{
    public interface IBuffTarget
    {
        ISerializeBuffTarget SerializeBuffTarget();
    }
    public interface ISerializeBuffTarget
    {
        IBuffTarget DeserializeBuffTarget(Level level);
    }
}
