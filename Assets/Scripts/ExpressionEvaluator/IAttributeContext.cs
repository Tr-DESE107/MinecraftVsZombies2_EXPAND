#nullable enable


namespace ExpressionEvaluator
{
    public interface IAttributeContext
    {
        object? EvaluateVariable(string name);
        object? EvaluateFunction(string name, object?[] args);
    }
}