using MVZ2.Talk;
using MVZ2.Talks;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public class LevelTalkSystem : MVZ2TalkSystem
    {
        public LevelTalkSystem(LevelEngine level, TalkController talk) : base(talk)
        {
            this.level = level;
        }
        public override bool IsInArchive()
        {
            return false;
        }
        public override bool IsInMap()
        {
            return false;
        }
        public override LevelEngine GetLevel()
        {
            return level;
        }
        private LevelEngine level;
    }
}
