#nullable enable

using MVZ2Logic.Options;
using PVZEngine.Level;
using PVZEngine.Definitions;

namespace MVZ2.Options
{
    public class OptionContextLevel : OptionContext, IOptionContextLevel
    {
        public OptionContextLevel(LevelEngine level)
        {
            this.level = level;
        }
        public LevelEngine GetLevel()
        {
            return level;
        }
        public LevelEngine level;

    }
}