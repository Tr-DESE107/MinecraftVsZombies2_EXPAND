using PVZEngine.Entities;

namespace MVZ2Logic.HeldItems
{
    public enum HeldHighlightMode
    {
        None,
        Entity,
        Grid
    }
    public struct HeldHighlight
    {
        public HeldHighlightMode mode;
        public Entity entity;
        public bool gridValid;
        public float gridRangeStart;
        public float gridRangeEnd;
        public static readonly HeldHighlight None = new HeldHighlight()
        {
            mode = HeldHighlightMode.None
        };
        public static HeldHighlight Entity(Entity entity)
        {
            return new HeldHighlight()
            {
                mode = HeldHighlightMode.Entity,
                entity = entity,
            };
        }
        public static HeldHighlight Green(float start = 0, float end = 1)
        {
            return new HeldHighlight()
            {
                mode = HeldHighlightMode.Grid,
                gridValid = true,
                gridRangeStart = start,
                gridRangeEnd = end,
            };
        }
        public static HeldHighlight Red()
        {
            return new HeldHighlight()
            {
                mode = HeldHighlightMode.Grid,
                gridValid = false,
                gridRangeStart = 0,
                gridRangeEnd = 1,
            };
        }
    }
}
