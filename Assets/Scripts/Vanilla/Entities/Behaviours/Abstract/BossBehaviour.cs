using MVZ2.GameContent.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class BossBehaviour : AIEntityBehaviour
    {
        protected BossBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
