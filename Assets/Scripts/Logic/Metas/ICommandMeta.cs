namespace MVZ2Logic.Commands
{
    public interface ICommandMeta
    {
        string GetDescription();
        ICommandVariantMeta[] GetVariants();
    }
    public interface ICommandVariantMeta
    {
        string GetGrammarText(string commandName);
        string GetDescription();
        ICommandParameterMeta[] GetParameters();
    }
    public interface ICommandParameterMeta
    {
        string GetName();
        string GetTypeString();
        string GetDescription();
    }
}
