using System;

namespace MVZ2.Vanilla
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefinitionAttribute : Attribute
    {
        public DefinitionAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; }
    }
}
