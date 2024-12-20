using MVZ2.Cursors;
using MVZ2.Level;

namespace MVZ2.HeldItems
{
    public class HeldItemCursorSource : CursorSource
    {
        public HeldItemCursorSource(LevelController level)
        {
            levelController = level;
        }
        public override CursorType CursorType => CursorType.Empty;

        public override int Priority => -100;

        public override bool IsValid()
        {
            return levelController;
        }
        private LevelController levelController;
    }
}
