using System;

namespace MVZ2.Vanilla
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SpawnDefinitionAttribute : Attribute
    {
        public SpawnDefinitionAttribute(int spawnCost)
        {
            this.SpawnCost = spawnCost;
        }
        public int SpawnCost { get; }
    }
}
