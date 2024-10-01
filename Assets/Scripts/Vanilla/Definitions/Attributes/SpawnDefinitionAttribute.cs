using System;

namespace MVZ2.Vanilla
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SpawnDefinitionAttribute : Attribute
    {
        public SpawnDefinitionAttribute(int spawnCost, int previewCount = 1)
        {
            this.SpawnCost = spawnCost;
            this.PreviewCount = previewCount;
        }
        public int SpawnCost { get; }
        public int PreviewCount { get; }
    }
}
