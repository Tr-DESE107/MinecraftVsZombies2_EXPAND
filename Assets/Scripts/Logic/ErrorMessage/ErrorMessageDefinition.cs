using PVZEngine.Base;

namespace MVZ2Logic.Errors
{
    public class ErrorMessageDefinition : Definition
    {
        public ErrorMessageDefinition(string nsp, string name, string definitionType) : base(nsp, name)
        {
            DefinitionType = definitionType;
        }
        public sealed override string GetDefinitionType() => DefinitionType;
        public string Message { get; set; }
        public string DefinitionType { get; }
    }
}
