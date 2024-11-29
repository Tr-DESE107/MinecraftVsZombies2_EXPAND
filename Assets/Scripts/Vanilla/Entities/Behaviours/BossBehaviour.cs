using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class BossBehaviour : VanillaEntityBehaviour
    {
        protected BossBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.RightFaction);
        }
    }
}
