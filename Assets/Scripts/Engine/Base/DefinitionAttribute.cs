using System;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class DefinitionAttribute : Attribute
    {
        public DefinitionAttribute(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; }
        public string Type { get; }
    }
}
