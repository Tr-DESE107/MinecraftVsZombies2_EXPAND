using MVZ2.Talk;
using MVZ2.Talks;
using PVZEngine.Level;

namespace MVZ2.Archives
{
    public class ArchiveTalkSystem : MVZ2TalkSystem
    {
        public ArchiveTalkSystem(TalkController talk) : base(talk)
        {
        }

        public override bool IsInArchive()
        {
            return true;
        }

        public override LevelEngine GetLevel()
        {
            return null;
        }
    }
}
