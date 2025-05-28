﻿using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2Logic.Archives;
using MVZ2Logic.Maps;
using PVZEngine.Level;

namespace MVZ2.Archives
{
    public class ArchiveTalkSystem : MVZ2TalkSystem
    {
        public ArchiveTalkSystem(IArchiveInterface archive, TalkController talk) : base(talk)
        {
            this.archive = archive;
        }

        public override IArchiveInterface GetArchive()
        {
            return archive;
        }
        public override IMapInterface GetMap()
        {
            return null;
        }
        public override LevelEngine GetLevel()
        {
            return null;
        }
        private IArchiveInterface archive;
    }
}
