using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Managers;
using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2Logic.Talk;
using PVZEngine.Level;
using UnityEditor.VersionControl;

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

        public override LevelEngine GetLevel()
        {
            return null;
        }
    }
}
