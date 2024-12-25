using MVZ2.Talk;
using MVZ2.Talks;
using PVZEngine.Level;

namespace MVZ2.Map
{
    public class MapTalkSystem : MVZ2TalkSystem
    {
        public MapTalkSystem(TalkController talk) : base(talk)
        {
        }

        public override bool IsInArchive()
        {
            return false;
        }
        public override bool IsInMap()
        {
            return true;
        }
        public override LevelEngine GetLevel()
        {
            return null;
        }
    }
}
