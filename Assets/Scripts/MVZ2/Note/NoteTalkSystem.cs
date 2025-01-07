using MVZ2.Archives;
using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2Logic.Archives;
using MVZ2Logic.Maps;
using PVZEngine.Level;

namespace MVZ2.Map
{
    public class NoteTalkSystem : MVZ2TalkSystem
    {
        public NoteTalkSystem(TalkController talk) : base(talk)
        {
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
            return null;
        }
    }
}
