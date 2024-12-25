using MVZ2.Talk;
using MVZ2.Talks;
using PVZEngine.Level;

namespace MVZ2.Map
{
    public class NoteTalkSystem : MVZ2TalkSystem
    {
        public NoteTalkSystem(TalkController talk) : base(talk)
        {
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
            return null;
        }
    }
}
