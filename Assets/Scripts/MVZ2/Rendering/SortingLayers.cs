using UnityEngine;

namespace MVZ2
{
    public static class SortingLayers
    {
        public static readonly int background = SortingLayer.NameToID("Background");
        public static readonly int shadow = SortingLayer.NameToID("Shadow");
        public static readonly int places = SortingLayer.NameToID("Places");
        public static readonly int entities = SortingLayer.NameToID("Entities");
        public static readonly int collectedDrops = SortingLayer.NameToID("CollectedDrops");
        public static readonly int drops = SortingLayer.NameToID("Drops");
        public static readonly int uis = SortingLayer.NameToID("UIs");
        public static readonly int screens = SortingLayer.NameToID("Screens");
        public static readonly int gui = SortingLayer.NameToID("GUI");
    }
}