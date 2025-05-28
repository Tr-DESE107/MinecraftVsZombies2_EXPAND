using MVZ2.Vanilla.Level;
using MVZ2Logic.IZombie;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class IZombiePuzzleBehaviour : IZombieBehaviour
    {
        public IZombiePuzzleBehaviour(StageDefinition stageDef, NamespaceID layout) : base(stageDef)
        {
            this.layout = layout;
        }
        protected override NamespaceID GetNewLayout(int round, RandomGenerator rng)
        {
            return layout;
        }

        protected override int GetMaxRounds()
        {
            return 1;
        }
        protected override void ReplaceBlueprints(LevelEngine level, IZombieLayoutDefinition layout)
        {
            if (layout == null)
                return;
            level.FillSeedPacks(layout.Blueprints);
        }
        private NamespaceID layout;
    }
}
