using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2Logic.Archives;
using MVZ2Logic.Maps;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public class LevelTalkSystem : MVZ2TalkSystem
    {
        public LevelTalkSystem(LevelEngine level, TalkController talk) : base(talk)
        {
            this.level = level;
        }
        public override IArchiveInterface GetArchive()
        {
            return null;
        }
        public override IMapInterface GetMap()
        {
            return null;
        }
        public override LevelEngine GetLevel()
        {
            return level;
        }
        private LevelEngine level;
    }
}
